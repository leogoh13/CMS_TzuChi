Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class API
    Public Function SendAPI(body As String, ByVal website As String)
        Dim httpWebRequest = CType(WebRequest.Create(website), HttpWebRequest)
        httpWebRequest.ContentType = "application/json"
        httpWebRequest.Method = "POST"

        Using streamWriter = New StreamWriter(httpWebRequest.GetRequestStream())
            Console.WriteLine(website)
            Console.WriteLine(body)
            streamWriter.Write(body)
        End Using

        Dim httpResponse = CType(httpWebRequest.GetResponse(), HttpWebResponse)

        If httpResponse.StatusCode = HttpStatusCode.OK Then
            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                Dim responseText As String = streamReader.ReadToEnd()
                'responseText contains the response received by the request            

                Console.WriteLine("responseText : " & responseText)

                If (Not responseText.Contains("id")) Then
                    Dim tempPos = New With {Key .itemName = ""}
                    Dim post = JsonConvert.DeserializeAnonymousType(body, tempPos)
                    Dim itemS As String = post.itemName
                Else
                End If
            End Using
        End If

        If website Is Nothing Then
            website = "NULL"
        End If
    End Function

    Public Function SendAPIAndGetProductID(body As String, website As String) As String
        Dim productID = ""
        Dim httpWebRequest = CType(WebRequest.Create(website), HttpWebRequest)
        httpWebRequest.ContentType = "application/json"
        httpWebRequest.Method = "POST"

        Using streamWriter = New StreamWriter(httpWebRequest.GetRequestStream())
            streamWriter.Write(body)
        End Using
        Dim httpResponse = CType(httpWebRequest.GetResponse(), HttpWebResponse)
        Dim responseText As String
        If httpResponse.StatusCode = HttpStatusCode.OK Then
            Console.WriteLine("Response OK")
            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                responseText = streamReader.ReadToEnd()
            End Using
            productID = JObject.Parse(responseText)("status").ToString
        End If
        Console.WriteLine("productID : " & productID)
        Return productID
    End Function

End Class