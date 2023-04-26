Imports System.IO
Imports System.Net
Imports System.Net.Mime
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class API
    Public Function SendAPIReturnNull(body As String, website As String) As String
        Dim response As String
        Dim request As WebRequest
        Dim jsonDataBytes = Encoding.Default.GetBytes(body)

        request = WebRequest.Create(website)
        request.ContentLength = jsonDataBytes.Length
        request.ContentType = "application/json"
        request.Method = "POST"

        Using requestStream = request.GetRequestStream
            requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
            requestStream.Close()

            Using responseStream = request.GetResponse.GetResponseStream
                Using reader As New StreamReader(responseStream)
                    response = reader.ReadToEnd()
                End Using
            End Using

            Logger.WriteLine("request : " + body)
            Logger.WriteLine("response : " + response)
        End Using
        Return response
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
            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                responseText = streamReader.ReadToEnd()
            End Using
            Console.
            productID = JObject.Parse(responseText)("status").ToString
        End If
        Logger.WriteLine("productID : " & productID)
        Return productID
    End Function

End Class