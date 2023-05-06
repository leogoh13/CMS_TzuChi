Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

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

    Public Function SendAPIAndGetProductID(body As String, website As String) As Product_EndpointA_ToGetProductIDHeader
        Console.WriteLine("SendAPIAndGetProductID")
        Dim response As String = ""
        Dim request As WebRequest
        Dim jsonDataBytes = Encoding.Default.GetBytes(body)
        Dim product As Product_EndpointA_ToGetProductIDHeader
        Logger.WriteLine("Product ID request : " + body)

        Try
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

            End Using

            Logger.WriteLine("Product ID response : " + response)
        Catch ex As Exception
            Logger.WriteLine(ex.ToString & " " & ex.Message)
        End Try
        product = JsonConvert.DeserializeObject(Of Product_EndpointA_ToGetProductIDHeader)(response)
        Return product
    End Function

End Class


Public Class Product_EndpointA_ToGetProductIDDetail
    Public id As String
    Public product As String
    Public status As String
End Class

Public Class Product_EndpointA_ToGetProductIDHeader
    Public itemName As String
    Public results As List(Of Product_EndpointA_ToGetProductIDDetail)
End Class

Public Class Product_EndpointC_ToSaveNewProduct_Header
    Public userId As String
    Public hash As String
    Public new_item As List(Of Product_EndpointC_ToSaveNewProduct_Detail)
End Class

Public Class Product_EndpointC_ToSaveNewProduct_Detail
    Public type As String
    Public form As String
    Public trada_name As String
    Public generic_name As String
    Public display_name As String
    Public purpose As String
    Public measurement As String
    Public unit As String
    Public item_Class As String
    Public sales_price As String
    Public cost_price As String
    Public age_limit As String
    Public dosage As String
    Public default_qty As String
    Public indic_guide As String
    Public dosage_guide As String
End Class
