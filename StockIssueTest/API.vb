Imports System.CodeDom.Compiler
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class API
    Public Function SendAPI(body As String, ByVal website As String)
        Dim httpWebRequest = CType(WebRequest.Create(website), HttpWebRequest)
        httpWebRequest.ContentType = "application/json"
        httpWebRequest.Method = "POST"

        Using streamWriter = New StreamWriter(httpWebRequest.GetRequestStream())
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

    Public Function SendAPIAndGetProductID(body As String, website As String) As ProductID_EndpointAResult
        Dim jsonDataBytes = Encoding.UTF8.GetBytes(body)
        Dim response As String = ""
        Dim request As WebRequest

        Try


            request = WebRequest.Create(website)
            With request
                .ContentLength = jsonDataBytes.Length
                .ContentType = "application/json"
                .Method = "POST"
            End With

            Using requestStream = request.GetRequestStream
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                requestStream.Close()

                Using responseStream = request.GetResponse.GetResponseStream
                    Using reader As New StreamReader(responseStream)
                        response = reader.ReadToEnd()
                    End Using
                End Using

                Console.WriteLine("response : " & response)
            End Using

        Catch ex As Exception
            Console.WriteLine(ex.ToString & " | " & ex.Message)
        End Try

        Dim productIDEndpointA As ProductID_EndpointAResult = JsonConvert.DeserializeObject(Of ProductID_EndpointAResult)(response)

        For Each field In productIDEndpointA.results
            Console.WriteLine(field.ToString + " : " + field.id)
        Next
        Return productIDEndpointA
    End Function

End Class


Public Class ProductID_EndpointADetail
    Public id As String
    Public product As String
    Public status As String
End Class
Public Class ProductID_EndpointAResult
    Public itemName As String
    Public results As List(Of ProductID_EndpointADetail)
End Class

