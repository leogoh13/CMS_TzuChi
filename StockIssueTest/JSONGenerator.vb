Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JSONGenerator

    Public Function GetProductID_EndpointA(item As String, site As String) As String
        Dim columns As New List(Of String) From {
            "ITEM",
            "DESC1",
            "DESC2"
        }

        Dim sql As New SQL()
        Dim website As String
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

        Dim itemName = retVal(0).Trim & " " & retVal(1).Trim
        Dim hash = Hash_SHA1.HashSHA1($"{userID}_{itemName.Trim}_{GlobalHashKey}")

        Dim str =
        $"{{
            ""userId"" : ""{userID}"",
            ""hash"" : ""{hash}"",
            ""itemName"" : ""{itemName}""
        }}"

        Logger.WriteLine("EndpointA JSON : " & str)
        Logger.WriteLine("EndpointA URL : " & website)

        Dim api As New API
        Dim product As Product_EndpointA_ToGetProductIDDetail = Nothing

        Try
            product = api.SendAPIAndGetProductID(str, website).results.Item(0)
        Catch ex As Exception
            Logger.WriteLine(ex.ToString() & " | " & ex.Message)
        End Try

        Return product.id
    End Function

    Public Function GetIssuance_EndpointD() As String

        Dim issuance = GetIssuance_Invoice()

        If issuance IsNot Nothing Then

            Dim userID As String
            Dim website As String

            Logger.WriteLine("issuance.siteTo : " + issuance.siteTo)

            If issuance.siteTo.Contains("F01") Then
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
            ElseIf issuance.siteTo.Contains("F02") Then
                userID = XMLX.GetSingleValue("//UserID/SiteKlang")
                website = XMLX.GetSingleValue("//API/Site/Klang")
            ElseIf issuance.siteTo.Contains("F03") Then
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
        End If

        Logger.WriteLine("Nothing to issue")
        Return ""
    End Function

    Public Function GetIssuance_Items(invoiceNumber) As String

        Dim sql As New SQL()
        Dim str As String = ""
        Dim issuanceRecords As New List(Of CMS_ISSUANCE)
        sql.ExecuteAndReturnSTOJOURecords(issuanceRecords)

        For Each issue In issuanceRecords
            Dim itemID As String = GetProductID_EndpointA(issue.itemNumber, issue.siteTo)

            If itemID = "" Then
                SaveProduct_EndpointC(issue.itemNumber)
            End If


            str +=
           $"{{
        	""itemId"" : {itemID},
        	""qty"" : {issue.quantity},
        	""expiry"" : ""{Convert.ToDateTime(issue.expirationDate).ToString("yyyy-MM-dd")}"",
        	""cost"" : {issue.cost}
            }},"
        Next
        Console.WriteLine("str : " + str)
        Console.WriteLine("str.LastIndexOf("","").ToString : " + str.LastIndexOf(",").ToString)
        Console.WriteLine("str.Length.ToString : " + str.Length.ToString)

        str = str.Substring(0, str.LastIndexOf(","))
        Return str

    End Function

    Public Function GetIssuance_Invoice() As CMS_ISSUANCE
        Dim query As String = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE FROM {GlobalDatabaseSchema}.STOJOU WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU)"
        Dim sql As New SQL()
        Dim issuance As New List(Of CMS_ISSUANCE)


        sql.ExecuteAndReturnSTOJOURecords(issuance)
        If issuance.Count > 0 Then
            Return issuance.Item(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function SaveProduct_EndpointC(itemNumber As String) As String




        Return ""
    End Function

End Class

