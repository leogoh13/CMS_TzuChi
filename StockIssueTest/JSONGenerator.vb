Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class JSONGenerator

    Public Function GetProductID_EndpointA(item As String, site As String) As ProductID_EndpointADetail
        Dim columns As New List(Of String)
        columns.Add("ITEM")
        columns.Add("DESC1")
        columns.Add("DESC2")

        Dim sql As New SQL()
        Dim website As String
        Dim query = $"SELECT ITMREF_0 ITEM, ITMDES1_0 DESC1, ITMDES2_0 DESC2 FROM YTCPROD.ITMMASTER WHERE ITMREF_0 = '{item}'"
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

        Dim itemName = retVal(0).Trim & " " & retVal(1).Trim & " " & retVal(2).Trim
        Dim hash = Hash_SHA1.HashSHA1($"{userID}_{itemName.Trim}_{GlobalHashKey}")

        Dim str =
        $"{{
            ""userId"" : ""{userID}"",
            ""hash"" : ""{hash}"",
            ""itemName"" : ""{itemName}""
        }}"

        Dim api As New API
        Return api.SendAPIAndGetProductID(str, website).results.Item(0)
    End Function

    Public Function GetIssuance_EndpointD() As String

        Dim list = GetIssuance_Invoice()
        Dim invoiceNumber = list(0)
        Dim site = list(1)
        Dim itemNumber = list(2)
        Dim quantity = list(3)
        Dim amount = list(4)
        Dim expirationDate = list(5)

        Dim userID = ""
        Select Case site
            Case "31F01"
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
            Case "31F02"
                userID = XMLX.GetSingleValue("//UserID/SiteKlang")
            Case "31F03"
                userID = XMLX.GetSingleValue("//UserID/SiteMelaka")
            Case Else
                userID = XMLX.GetSingleValue("//UserID/SiteKLPudu")
        End Select


        Hash_SHA1.HashSHA1($"{userID}_{invoiceNumber}_{GlobalHashKey}")
        Dim str As String =
            $"{{
	            ""userId"" : ""{userID}"",
	            ""hash"" : ""{Hash_SHA1.HashSHA1($"{userID}_{invoiceNumber}_{GlobalHashKey}")}"",
	            ""new_stock"" : {{
		            ""date"" : ""{expirationDate}"",
		            ""vendor_id"" : 31,
		            ""invoice_no"" : ""{invoiceNumber}"",
		            ""stocks"" : [{GetIssuance_Items(invoiceNumber, itemNumber, quantity, expirationDate, amount, site)}
	            }}
            }}"

        Dim parsedJSOn As JToken = JToken.Parse(str)
        Dim beautified = parsedJSOn.ToString(Formatting.Indented)
        Dim minified = parsedJSOn.ToString(Formatting.None)
        Return beautified
    End Function

    Public Function GetIssuance_Items(invoiceNumber, item, qty, expiryDate, cost, site) As String

        Dim sql As New SQL()
        Dim amountOfItem = sql.ExecuteCustomQueryAndReturnValue($"SELECT COUNT(*) COL1 FROM YTCPROD.STOJOU WHERE VCRNUM_0 = '{invoiceNumber}'")
        Dim str As String = ""

        Dim list As New List(Of String)
        list.Add("ITEM")
        list.Add("QTY")
        list.Add("AMT")
        list.Add("EXPDAT")

        Dim query = "SELECT ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT FROM YTCPROD.STOJOU WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM YTCPROD.TEMP_STOJOU )"
        Dim retVal = sql.ExecuteQueryAndReturnValue(query, list)

        For Each value In retVal
            Console.WriteLine("value : " & value)
            str =
           $"{{
        	""itemId"" : {GetProductID_EndpointA(item, site).id},
        	""qty"" : {qty},
        	""expiry"" : ""{expiryDate}"",
        	""cost"" : {cost}
        }}"
        Next
        str += "]"
        Return str
    End Function

    Public Function GetIssuance_Invoice() As List(Of String)
        'Dim query As String = "SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT FROM YTCPROD.STOJOU WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM YTCPROD.TEMP_STOJOU )"
        Dim query As String =
            $"SELECT 
	            A.ITMREF_0 ITEM, 
	            B.ITMDES1_0 DESCRIPTION1,
	            B.ITMDES2_0 DESCRIPTION2,
	            A.STOFCY_0 SITETO,
	            QTYSTU_0 * -1 QUANTITY, 
	            CPRAMT_0 * -1 AMOUNT, 
	            SHLDAT_0 EXPIRATIONDATE,
	            A.UPDDATTIM_0 UPDATEDATE,
	            A.VCRNUM_0 INVOICENUMBER
            FROM YTCPROD.STOJOU A
            LEFT JOIN YTCPROD.ITMMASTER B ON A.ITMREF_0 = B.ITMREF_0
            WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM YTCPROD.TEMP_STOJOU GROUP BY VCRNUM_0 )"
        Dim sql As New SQL()
        Dim issuance As CMS_ISSUANCE

        Return sql.ExecuteQueryAndReturnValue(query, issuance)
    End Function
End Class
