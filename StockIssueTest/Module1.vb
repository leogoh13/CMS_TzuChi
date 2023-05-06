Imports System.IO
Imports System.Text
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.Logging

Module Module1

    Public GlobalHashKey As String = XMLX.GetSingleValue("//API/HashSumConstant")
    Public GlobalDatabaseSchema As String = XMLX.GetSingleValue("//database/databaseSchema")

    Sub Main()

        Dim impersonator As New clsAuthenticator
        Dim RDP_Directory As String = XMLX.GetSingleValue("//RDP/Directory")
        Dim RDP_Domain As String = XMLX.GetSingleValue("//RDP/Domain")
        Dim RDP_Username As String = XMLX.GetSingleValue("//RDP/Username")
        Dim RDP_Password As String = XMLX.GetSingleValue("//RDP/Password")
        Dim json As New JSONGenerator()

        Dim retVal = json.GetIssuance_EndpointD()
        Logger.WriteLine(retVal)
    End Sub

End Module


Public Class Logger
    Shared filepath As String = $"{My.Application.Info.DirectoryPath}\Logs\Log_{Date.UtcNow.ToString("yyyyMMdd")}.log"
    Shared file As New FileInfo(filepath)
    Public Shared Sub WriteLine(str As String)
        If Not file.Exists() Then
            file.Create().Close()
        End If

        Dim datetime = Date.Now.ToString("ddMMyyyy hh:mm:ss tt")
        Dim sw As StreamWriter = file.AppendText()
        sw.WriteLine(datetime & " : " & str)
        sw.Close()

    End Sub
End Class



Public Class CMS_ISSUANCE

    Public itemNumber As String
    Public itemIdNumber As String
    Public hash As String
    Public invoiceNumber As String
    Public vendorID As String
    Public itemDesc1 As String
    Public itemDesc2 As String
    Public siteTo As String
    Public quantity As Double
    Public cost As Double
    Public expirationDate As Date
    Public updateDate As Date

End Class

