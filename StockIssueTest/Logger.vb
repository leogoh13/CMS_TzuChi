Imports System.IO

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
        Console.WriteLine(datetime & " : " & str)
        sw.Close()

    End Sub
End Class

