Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JSONGenerator

    Public Function GetProductID_EndpointA(item As String, site As String) As String
        Dim columns As New List(Of String)
        columns.Add("ITEM")
        columns.Add("DESC1")
        columns.Add("DESC2")

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

        Dim itemName = retVal(0).Trim & " " & retVal(1).Trim '& " " & retVal(2).Trim
        Dim hash = Hash_SHA1.HashSHA1($"{userID}_{itemName.Trim}_{GlobalHashKey}")

        Dim str =
        $"{{
            ""userId"" : ""{userID}"",
            ""hash"" : ""{hash}"",
            ""itemName"" : ""{itemName}""
        }}"

        Console.WriteLine("EnpointA JSON : " & str)
        Console.WriteLine("EnpointA URL : " & website)

        Dim api As New API
        Dim product As Product_EndpointA_ToGetProductIDDetail = api.SendAPIAndGetProductID(str, website).results.Item(0)
        Return product.id
    End Function

    Public Function GetIssuance_EndpointD() As String

        Dim issuance = GetIssuance_Invoice()
        If issuance Is Nothing Then
            Logger.WriteLine("Nothing to issue")
            Return ""
        End If
        Logger.WriteLine("issuance.siteTo : " + issuance.siteTo)

        Dim userID As String = ""
        Dim website As String = ""
            If issuance.siteTo.Contains("F01") Then
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
            ElseIf issuance.siteTo.Contains("F02") Then
                userID = XMLX.GetSingleValue("//UserID/SiteKlang")
                website = XMLX.GetSingleValue("//API/Site/Klang")
            ElseIf issuance.siteTo.Contains("F03") Then
                Return ""
                userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
            website = XMLX.GetSingleValue("//API/Site/Melaka")
        End If
        Logger.WriteLine("userID : " + userID)
        Logger.WriteLine("website : " + website)

        Try
            Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")
            Dim str As String =
            $"{{
	            ""userId"" : ""{userID}"",
	            ""hash"" : ""{Hash_SHA1.HashSHA1($"{userID}_{issuance.invoiceNumber}_{GlobalHashKey}")}"",
	            ""new_stock"" : {{
		            ""date"" : ""{Convert.ToDateTime(issuance.updateDate).ToString("yyyy-MM-dd")}"",
		            ""vendor_id"" : 31,
		            ""invoice_no"" : ""{issuance.invoiceNumber}"",
		            ""stocks"" : [{GetIssuance_Items(issuance.invoiceNumber)}]
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
        Return ""
    End Function

    Public Function GetIssuance_Items(invoiceNumber) As String

        Dim sql As New SQL()
        Dim str As String = ""
        Dim issuanceRecords As New List(Of CxSYS_ISSUANCE)
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
        	""expiry"" : ""{Convert.ToDateTime(issue.expirationDate).ToString("yyyy-MM-dd")}"",
        	""cost"" : {issue.cost}
            }},"
        Next
        str = str.Substring(0, str.LastIndexOf(","))
        Return str

    End Function

    Public Function GetIssuance_Invoice() As CxSYS_ISSUANCE
        Dim query As String = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE FROM {GlobalDatabaseSchema}.STOJOU WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU)"
        Dim sql As New SQL()
        Dim issuance As New List(Of CxSYS_ISSUANCE)


        sql.ExecuteAndReturnSTOJOURecords(issuance)
        If issuance.Count > 0 Then
            Return issuance.Item(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function SaveProductID_EndpointC() As String
        Logger.WriteLine("SaveProductID_EndpointC Starts")
        ' Check the TEMP_ITMMASTER table for pending records to send to CxSYS
        Dim sql As New SQL
        Dim products As New List(Of CxSYS_PRODUCT)
        Dim PuduSite As String = "F01"
        Dim KlangSite As String = "F02"
        Dim MelakaSite As String = "F03"

        Dim fPuduItemExist As Boolean = False
        Dim fKlangItemExist As Boolean = False
        Dim fMelakaItemExist As Boolean = False
        Dim itemSyncedInCxSYS As Boolean = False
        sql.GetNewProductList(products)


        ' Double check in CxSYS whether this record has already created or not
        For Each item In products
            Logger.WriteLine("item.itemReference : " & item.itemReference)

            'If GetProductID_EndpointA(item.itemReference, PuduSite) <> "" Then
            '    fPuduItemExist = True
            'End If

            'If GetProductID_EndpointA(item.itemReference, KlangSite) <> "" Then
            '    fKlangItemExist = True
            'End If

            'If GetProductID_EndpointA(item.itemReference, MelakaSite) <> "" Then
            '    fMelakaItemExist = True
            'End If

            'If fPuduItemExist And fKlangItemExist And fMelakaItemExist Then
            'itemSyncedInCxSYS = True
            'End If

            ' If not, get the item's necessary details and send to all sites.
            If Not itemSyncedInCxSYS Then

                Dim userID As String
                Dim website As String
                Dim hash As String
                Dim api As New API
                Dim cxsysClass As String
                Dim json As String

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


                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
                hash = Hash_SHA1.HashSHA1($"{userID}_{item.trade_name}_{GlobalHashKey}")
                Logger.WriteLine($"{userID}_{item.trade_name}_{GlobalHashKey}")
                json = $"
                        {{
                            ""userId"" : ""{userID}"",
                            ""hash"" : ""{hash}"",
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
                    Dim response = api.SendAPIReturnNull(json, website)
                    Dim parsedJSON = JToken.Parse(response)
                    Dim beautified = parsedJSON.ToString(Formatting.Indented)
                    Dim minified = parsedJSON.ToString(Formatting.None)
                    Logger.WriteLine(beautified)
                    Dim query As String = $"DELETE FROM {GlobalDatabaseSchema}.TEMP_ITMMASTER WHERE ITMREF_0 = '{item.itemReference}'"
                    sql.ExecuteCustomQueryAndReturnValue(query)
                Catch ex As Exception
                    Logger.WriteLine(ex.ToString & " " & ex.Message)
                End Try



                'userID = XMLX.GetSingleValue("//UserID/SiteKlang")
                'website = XMLX.GetSingleValue("//API/Site/Klang")
                'hash = Hash_SHA1.HashSHA1($"{userID}_{item.trade_name}_{GlobalHashKey}")
                'json = $"
                '        {{
                '            ""userId"" : ""{userID}"",
                '            ""hash"" : ""{hash}"",
                '            ""new_item"" : {{
                '             ""type"" : ""{cxsysType}"",
                '             ""form"" : ""{item.form}"",
                '             ""trade_name"" : ""{item.trade_name}"",
                '             ""generic_name"" : ""{item.generic_name}"",
                '             ""display_name"" : ""{item.display_name}"",
                '             ""purpose"" : ""{item.purpose}"",
                '             ""measurement"" : ""{item.measurement}"",
                '             ""unit"" : {item.unit},
                '             ""class"" : ""{cxsysClass}"",
                '             ""sales_price"" : {item.sales_price},
                '             ""cost_price"" : {item.cost_price},
                '             ""age_limit"" : {item.age_limit},
                '             ""dosage"" : ""{item.dosage}"",
                '             ""default_qty"" : {item.default_qty},
                '             ""indic_guide"" : ""{item.indic_guide}"",
                '             ""dosage_guide"" : ""{item.dosage_guide}""
                '         }}
                '        }}"
                'api.SendAPIReturnNull(json, website)



                'userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
                'website = XMLX.GetSingleValue("//API/Site/Melaka")
                'hash = Hash_SHA1.HashSHA1($"{userID}_{item.trade_name}_{GlobalHashKey}")
                'json = $"
                '        {{
                '            ""userId"" : ""{userID}"",
                '            ""hash"" : ""{hash}"",
                '            ""new_item"" : {{
                '             ""type"" : ""{cxsysType}"",
                '             ""form"" : ""{item.form}"",
                '             ""trade_name"" : ""{item.trade_name}"",
                '             ""generic_name"" : ""{item.generic_name}"",
                '             ""display_name"" : ""{item.display_name}"",
                '             ""purpose"" : ""{item.purpose}"",
                '             ""measurement"" : ""{item.measurement}"",
                '             ""unit"" : {item.unit},
                '             ""class"" : ""{cxsysClass}"",
                '             ""sales_price"" : {item.sales_price},
                '             ""cost_price"" : {item.cost_price},
                '             ""age_limit"" : {item.age_limit},
                '             ""dosage"" : ""{item.dosage}"",
                '             ""default_qty"" : {item.default_qty},
                '             ""indic_guide"" : ""{item.indic_guide}"",
                '             ""dosage_guide"" : ""{item.dosage_guide}""
                '         }}
                '        }}"
                'api.SendAPIReturnNull(json, website)

            End If

        Next


        Return ""

    End Function
End Class



