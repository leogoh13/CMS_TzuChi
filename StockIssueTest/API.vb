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

    Public Function SendAPIAndGetProductID(body As String, website As String) As Product_EndpointA_Header
        Console.WriteLine("SendAPIAndGetProductID")
        Dim response As String
        Dim request As WebRequest
        Dim jsonDataBytes = Encoding.Default.GetBytes(body)
        Dim productID As String
        Dim product As Product_EndpointA_Header
        Logger.WriteLine("Product ID request : " + body)

        Try
            Console.WriteLine("Try")
            request = WebRequest.Create(website)
            request.ContentLength = jsonDataBytes.Length
            request.ContentType = "application/json"
            request.Method = "POST"

            Using requestStream = request.GetRequestStream
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                requestStream.Close()
                Console.WriteLine("responseStream")
                Using responseStream = request.GetResponse.GetResponseStream
                    Using reader As New StreamReader(responseStream)
                        response = reader.ReadToEnd()
                    End Using
                End Using

            End Using
            product = JsonConvert.DeserializeObject(Of Product_EndpointA_Header)(response)

            Logger.WriteLine("Product ID response : " + response)
        Catch ex As Exception
            Logger.WriteLine(ex.ToString & " " & ex.Message)
        End Try

        Return product
    End Function

End Class


Public Class Product_EndpointA_Detail
    Public id As String
    Public product As String
    Public status As String
End Class

Public Class Product_EndpointA_Header
    Public itemName As String
    Public results As List(Of Product_EndpointA_Detail)
End Class