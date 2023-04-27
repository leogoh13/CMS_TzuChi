Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JSONGenerator

    Public Function GetProductID_EndpointA(item As String, site As String) As String
        Dim columns As New List(Of String)
        columns.Add("ITEM")
        columns.Add("DESC1")
        columns.Add("DESC2")

        Dim sql As New SQL()
        Dim website As String
        Dim query = $"SELECT ITMREF_0 ITEM, ITMDES1_0 DESC1, ITMDES2_0 DESC2 FROM {GlobalDatabaseSchema}.ITMMASTER WHERE ITMREF_0 = '{item}'"
        Dim retVal = sql.ExecuteQueryAndReturnValue(query, columns)
        Dim userID = Nothing
        Select Case site
            Case "31F01"
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
            Case "31F02"
                userID = XMLX.GetSingleValue("//UserID/SiteKlang")
                website = XMLX.GetSingleValue("//API/Site/Klang")
            Case "31F03"
                userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
                website = XMLX.GetSingleValue("//API/Site/Melaka")
            Case Else
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
        End Select

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
        Dim product As Product_EndpointA_Detail = api.SendAPIAndGetProductID(str, website).results.Item(0)
        Return product.id
    End Function

    Public Function GetIssuance_EndpointD() As String

        Dim issuance = GetIssuance_Invoice()

        If issuance Is Nothing Then
            Return ""
        End If

        Dim userID As String
        Dim website As String

        Select Case issuance.siteTo
            Case issuance.siteTo.Contains("F01")
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
            Case issuance.siteTo.Contains("F02")
                userID = XMLX.GetSingleValue("//UserID/SiteKlang")
                website = XMLX.GetSingleValue("//API/Site/Klang")
            Case issuance.siteTo.Contains("F03")
                userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
                website = XMLX.GetSingleValue("//API/Site/Melaka")
            Case Else
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
                website = XMLX.GetSingleValue("//API/Site/KLPudu")
        End Select

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

            Dim api As New API()
            Dim response = api.SendAPIReturnNull(str, website)
            Dim parsedJSON = JToken.Parse(response)
            Dim beautified = parsedJSON.ToString(Formatting.Indented)
            Dim minified = parsedJSON.ToString(Formatting.None)
            Logger.WriteLine(beautified)

            Dim sql As New SQL
            Dim query As String = $"DELETE FROM {GlobalDatabaseSchema}.TEMP_STOJOU WHERE VCRNUM_0 = '{issuance.invoiceNumber}'"
            sql.ExecuteCustomQueryAndReturnValue(query)
        Catch ex As Exception
            Logger.WriteLine(ex.ToString & " " & ex.Message)
        End Try
        Return ""
    End Function

    Public Function GetIssuance_Items(invoiceNumber) As String

        Dim sql As New SQL()
        Dim str As String = ""
        Dim issuanceRecords As New List(Of CMS_ISSUANCE)
        sql.ExecuteAndReturnSTOJOURecords(issuanceRecords)

        For Each issue In issuanceRecords
            Console.WriteLine(issue.itemNumber)
            Console.WriteLine(issue.siteTo)
            Console.WriteLine(issue.expirationDate)
            Console.WriteLine(issue.cost)
            str +=
           $"{{
        	""itemId"" : {GetProductID_EndpointA(issue.itemNumber, issue.siteTo)},
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
End Class

