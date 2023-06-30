Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JSONGenerator


    Public Function GetProductID_EndpointA(item As String, site As String) As String
        Logger.WriteLine($"GetProductID_EndpointA : {item} - {site}")
        Dim columns As New List(Of String) From {
            "ITEM",
            "DESC1",
            "DESC2"
        }

        Dim sql As New SQL()
        Dim website As String = ""
        Dim query = $"SELECT ITMREF_0 ITEM, ITMDES1_0 DESC1, ITMDES2_0 DESC2 FROM {GlobalDatabaseSchema}.ITMMASTER WHERE ITMREF_0 = '{item}'"
        Dim retVal = sql.ExecuteQueryAndReturnValue(query, columns)
        Dim userID = Nothing

        If site.Contains("F01") Then
            userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
            website = XMLX.GetSingleValue("//API/Site/KLPudu")
        ElseIf site.Contains("F02") Then
            userID = XMLX.GetSingleValue("//UserID/SiteKlang")
            website = XMLX.GetSingleValue("//API/Site/Klang")
        ElseIf site.Contains("F03") Then
            userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
            website = XMLX.GetSingleValue("//API/Site/Melaka")
        End If

        Logger.WriteLine($"GetProductID_EndpointA userID : {userID}")
        Logger.WriteLine($"GetProductID_EndpointA website : {website}")

        Dim itemName = retVal(0).Trim & " " & retVal(1).Trim '& " " & retVal(2).Trim
        Dim hash = Hash_SHA1.HashSHA1($"{userID}_{itemName.Trim}_{GlobalHashKey}")

        Dim str =
        $"{{
            ""userId"" : ""{userID}"",
            ""hash"" : ""{hash}"",
            ""itemName"" : ""{itemName}""
        }}"

        Dim api As New API
        Dim productIDHeader = api.SendAPIAndGetProductID(str, website)
        If productIDHeader Is Nothing Then
            Return "0"
        Else
            Dim product As Product_EndpointA_ToGetProductIDDetail = productIDHeader.results.Item(0)
            Return product.id
        End If

    End Function

    Public Function GetIssuance_EndpointD() As String

        Logger.WriteLine("Issuance Process - START")

        Dim issuance = GetIssuance_Invoice()
        If issuance Is Nothing Then
            Logger.WriteLine("Nothing to issue")
            Return ""
        End If
        Logger.WriteLine("issuance.siteTo : " + issuance.siteTo)

        Dim userID As String = ""
        Dim website As String = ""
        GetUserID(issuance, userID, website)

        Try

            Dim str As String = ""

            'str =
            '$"{{
            ' ""userId"" : ""{userID}"",
            ' ""hash"" : ""{Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")}"",
            ' ""edit_stock"" : {{
            '  ""invoice_no"" : ""{issuance.invoiceNumber}"",
            '        ""vendor_id"" : {VendorID},
            '            ""to_edit"" : {{
            '                ""itemId"" : {GetProductID_EndpointA(issuance.itemNumber, issuance.siteTo)}
            '        }}
            '  ""date"" : ""{Convert.ToDateTime(issuance.updateDate):yyyy-MM-dd}"",
            '  ""stocks"" : [{GetIssuance_Items()}]
            ' }}
            '}}"
            Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")
            str =
                $"{{
	                ""userId"" : ""{userID}"",
	                ""hash"" : ""{Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")}"",
	                ""new_stock"" : {{
		                ""date"" : ""{Convert.ToDateTime(issuance.updateDate):yyyy-MM-dd}"",
		                ""vendor_id"" : {VendorID},
		                ""invoice_no"" : ""{issuance.invoiceNumber}"",
		                ""stocks"" : [{GetIssuance_Items()}]
	                }}
                }}"

            Dim parsedJSON
            Dim beautified
            Dim minified
            If XMLX.GetSingleValue("//API/SendOutAPI") = "1" Then
                Try
                    Dim api As New API()
                    Dim response = api.SendAPIReturnNull(str, website)
                    parsedJSON = JToken.Parse(response)
                    beautified = parsedJSON.ToString(Formatting.Indented)
                    minified = parsedJSON.ToString(Formatting.None)
                    Logger.WriteLine(beautified)
                    Dim sql As New SQL
                    Dim query As String = $"DELETE FROM {GlobalDatabaseSchema}.TEMP_STOJOU WHERE VCRNUM_0 = '{issuance.invoiceNumber}'"
                    sql.ExecuteCustomQueryAndReturnValue(query)
                Catch ex As Exception
                    Logger.WriteLine(ex.ToString & " | " & ex.Message)
                End Try
            Else
                parsedJSON = JToken.Parse(str)
                beautified = parsedJSON.ToString(Formatting.Indented)
                minified = parsedJSON.ToString(Formatting.None)
                Logger.WriteLine(beautified)
            End If
        Catch ex As Exception
            Logger.WriteLine(ex.ToString & " " & ex.Message)
        End Try

        Logger.WriteLine("Issuance Process - END")

        Return ""
    End Function

    Public Function GetIssuance_Items() As String

        Dim sql As New SQL()
        Dim str As String = ""
        Dim issuanceRecords As New List(Of CMS_ISSUANCE)
        sql.ExecuteAndReturnSTOJOURecords(issuanceRecords)

        For Each issue In issuanceRecords

            Logger.WriteLine("issue.itemNumber : " & issue.itemNumber)
            Logger.WriteLine("issue.siteTo : " & issue.siteTo)
            Logger.WriteLine("issue.expirationDate : " & issue.expirationDate)
            Logger.WriteLine("issue.cost : " & issue.cost)

            str +=
           $"{{
        	""itemId"" : {GetProductID_EndpointA(issue.itemNumber, issue.siteTo)},
        	""qty"" : {issue.quantity},
        	""expiry"" : ""{Convert.ToDateTime(issue.expirationDate):yyyy-MM-dd}"",
        	""cost"" : {issue.cost}
            }},"
        Next
        str = str.Substring(0, str.LastIndexOf(","))
        Return str

    End Function

    Public Function GetIssuance_Invoice() As CMS_ISSUANCE
        Dim sql As New SQL()
        Dim issuance As New List(Of CMS_ISSUANCE)


        sql.ExecuteAndReturnSTOJOURecords(issuance)
        If issuance.Count > 0 Then
            Return issuance.Item(0)
        Else
            Logger.WriteLine("No product to create")
            Return Nothing
        End If
    End Function

    Public Function SaveProductID_EndpointC() As String

        Logger.WriteLine("Product Creation Process - START")

        ' Check the TEMP_ITMMASTER table for pending records to send to CxSYS
        Dim sql As New SQL
        Dim products As New List(Of CMS_PRODUCT)

        'Purposely set to these to trigger to all sites
        Dim PuduSite As String = "F01"
        Dim KlangSite As String = "F02"
        Dim MelakaSite As String = "F03"

        Dim itemSyncedInCxSYS As Boolean = False
        sql.GetNewProductList(products)


        ' Double check in CxSYS whether this record has already created or not
        For Each item In products

            Logger.WriteLine("Pudu : " & item.itemReference)
            Dim puduItemID = GetProductID_EndpointA(item.itemReference, PuduSite)
            Logger.WriteLine("Klang : " & item.itemReference)
            Dim klangItemID = GetProductID_EndpointA(item.itemReference, KlangSite)
            Logger.WriteLine("Melaka : " & item.itemReference)
            Dim melakaItemID = GetProductID_EndpointA(item.itemReference, MelakaSite)

            If Not itemSyncedInCxSYS Then

                Dim api As New API
                Dim cxsysClass As String

                If item.itemClass = "1" Then
                    cxsysClass = "Normal"
                ElseIf item.itemClass = "2" Then
                    cxsysClass = "Poison"
                ElseIf item.itemClass = "3" Then
                    cxsysClass = "Psychotropic"
                Else
                    cxsysClass = "Unselected"
                End If

                Dim cxsysType As String
                If item.type = "1" Then
                    cxsysType = "med"
                ElseIf item.type = "2" Then
                    cxsysType = "inj"
                ElseIf item.type = "3" Then
                    cxsysType = "np"
                Else
                    cxsysType = "Unselected"
                End If

                Dim puduUserID As String = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                Dim puduWebsite As String = XMLX.GetSingleValue("//API/Site/KLPudu")

                Dim klangUserID As String = XMLX.GetSingleValue("//UserID/SiteKlang")
                Dim klangWebsite As String = XMLX.GetSingleValue("//API/Site/Klang")

                Dim melakaUserID As String = XMLX.GetSingleValue("//UserID/SiteMelaka")
                Dim melakaWebsite As String = XMLX.GetSingleValue("//API/Site/Melaka")

                Dim puduHash = Hash_SHA1.HashSHA1($"{puduUserID}_{item.trade_name}_{GlobalHashKey}")
                Dim klangHash = Hash_SHA1.HashSHA1($"{klangUserID}_{item.trade_name}_{GlobalHashKey}")
                Dim melakaHash = Hash_SHA1.HashSHA1($"{melakaUserID}_{item.trade_name}_{GlobalHashKey}")

                Logger.WriteLine($"{puduUserID}_{item.trade_name}_{GlobalHashKey}")
                Logger.WriteLine($"{klangUserID}_{item.trade_name}_{GlobalHashKey}")
                Logger.WriteLine($"{melakaUserID}_{item.trade_name}_{GlobalHashKey}")

                Dim puduJSON As String = $"
                        {{
                            ""userId"" : ""{puduUserID}"",
                            ""hash"" : ""{puduHash}"",
                            ""new_item"" : {{
                             ""type"" : ""{cxsysType}"",
                             ""form"" : ""{item.form}"",
                             ""trade_name"" : ""{item.trade_name}"",
                             ""generic_name"" : ""{item.generic_name}"",
                             ""display_name"" : ""{item.display_name}"",
                             ""purpose"" : ""{item.purpose}"",
                             ""measurement"" : ""{item.measurement}"",
                             ""unit"" : {item.unit},
                             ""class"" : ""{cxsysClass}"",
                             ""sales_price"" : {item.sales_price},
                             ""cost_price"" : {item.cost_price},
                             ""age_limit"" : {item.age_limit},
                             ""dosage"" : ""{item.dosage}"",
                             ""default_qty"" : {item.default_qty},
                             ""indic_guide"" : ""{item.indic_guide}"",
                             ""dosage_guide"" : ""{item.dosage_guide}""
                         }}
                        }}"
                Dim klangJSON As String = $"
                        {{
                            ""userId"" : ""{klangUserID}"",
                            ""hash"" : ""{klangHash}"",
                            ""new_item"" : {{
                             ""type"" : ""{cxsysType}"",
                             ""form"" : ""{item.form}"",
                             ""trade_name"" : ""{item.trade_name}"",
                             ""generic_name"" : ""{item.generic_name}"",
                             ""display_name"" : ""{item.display_name}"",
                             ""purpose"" : ""{item.purpose}"",
                             ""measurement"" : ""{item.measurement}"",
                             ""unit"" : {item.unit},
                             ""class"" : ""{cxsysClass}"",
                             ""sales_price"" : {item.sales_price},
                             ""cost_price"" : {item.cost_price},
                             ""age_limit"" : {item.age_limit},
                             ""dosage"" : ""{item.dosage}"",
                             ""default_qty"" : {item.default_qty},
                             ""indic_guide"" : ""{item.indic_guide}"",
                             ""dosage_guide"" : ""{item.dosage_guide}""
                         }}
                        }}"
                Dim melakaJSON As String = $"
                        {{
                            ""userId"" : ""{melakaUserID}"",
                            ""hash"" : ""{melakaHash}"",
                            ""new_item"" : {{
                             ""type"" : ""{cxsysType}"",
                             ""form"" : ""{item.form}"",
                             ""trade_name"" : ""{item.trade_name}"",
                             ""generic_name"" : ""{item.generic_name}"",
                             ""display_name"" : ""{item.display_name}"",
                             ""purpose"" : ""{item.purpose}"",
                             ""measurement"" : ""{item.measurement}"",
                             ""unit"" : {item.unit},
                             ""class"" : ""{cxsysClass}"",
                             ""sales_price"" : {item.sales_price},
                             ""cost_price"" : {item.cost_price},
                             ""age_limit"" : {item.age_limit},
                             ""dosage"" : ""{item.dosage}"",
                             ""default_qty"" : {item.default_qty},
                             ""indic_guide"" : ""{item.indic_guide}"",
                             ""dosage_guide"" : ""{item.dosage_guide}""
                         }}
                        }}"

                Try
                    ' PUDU
                    If puduItemID = "0" Then
                        Logger.WriteLine(JToken.Parse(api.SendAPIReturnNull(puduJSON, puduWebsite)).ToString(Formatting.Indented))
                    End If

                    ' KLANG
                    If klangItemID = "0" Then
                        Logger.WriteLine(JToken.Parse(api.SendAPIReturnNull(klangJSON, klangWebsite)).ToString(Formatting.Indented))
                    End If

                    ' MELAKA
                    If melakaItemID = "0" Then
                        Logger.WriteLine(JToken.Parse(api.SendAPIReturnNull(melakaJSON, melakaWebsite)).ToString(Formatting.Indented))
                    End If

                    ' Remove the item from the table
                    Dim query As String = $"DELETE FROM {GlobalDatabaseSchema}.TEMP_ITMMASTER WHERE ITMREF_0 = '{item.itemReference}'"
                    sql.ExecuteCustomQueryAndReturnValue(query)
                Catch ex As Exception
                    Logger.WriteLine(ex.ToString & " " & ex.Message)
                End Try

            End If
        Next

        Logger.WriteLine("Product Creation Process - END")

        Return ""

    End Function

    Public Function EditStock_EndpointF() As String

        Dim sql As New SQL()
        Dim str As String = ""
        Dim issuanceRecords As New List(Of CMS_ISSUANCE)
        sql.ExecuteAndReturnSTOJOURecords(issuanceRecords)

        For Each issuance In issuanceRecords

            Logger.WriteLine("issue.itemNumber : " & issuance.itemNumber)
            Logger.WriteLine("issue.siteTo : " & issuance.siteTo)
            Logger.WriteLine("issue.expirationDate : " & issuance.expirationDate)
            Logger.WriteLine("issue.cost : " & issuance.cost)

            Dim userID As String = Nothing
            Dim website As String = Nothing
            GetUserID(issuance, userID, website)


            str +=
            $"{{
                ""userId"" : ""{userID}"",
                ""hash"" : ""{Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")}"",
                ""edit_stock"" : {{
                    ""invoice_no"" : ""{issuance.invoiceNumber}"",
                    ""vendor_id"" : ""{VendorID}"",
                    ""to_edit"" : {{
                        ""itemId"" : {issuance.itemIdNumber},
                        ""qty"" : {issuance.quantity:.00},
                        ""expiry"" : ""{issuance.expirationDate}""
                        ""cost"" : {issuance.cost}
                    }}
                }}
            }}"

        Next

    End Function

    Public Sub GetUserID(issuance As CMS_ISSUANCE, ByRef userID As String, ByRef website As String)
        If issuance.siteTo.Contains("F01") Then
            userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
            website = XMLX.GetSingleValue("//API/Site/KLPudu")
        ElseIf issuance.siteTo.Contains("F02") Then
            userID = XMLX.GetSingleValue("//UserID/SiteKlang")
            website = XMLX.GetSingleValue("//API/Site/Klang")
        ElseIf issuance.siteTo.Contains("F03") Then
            'userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
            'website = XMLX.GetSingleValue("//API/Site/Melaka")
            userID = ""
            website = ""
        End If
        Logger.WriteLine("userID : " + userID)
        Logger.WriteLine("website : " + website)
    End Sub
End Class