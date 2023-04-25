Imports System.Xml

Public Class XMLX

    Public Shared Function GetSingleValue(xpath) As String
        Dim doc As New XmlDocument()
        doc.Load($"{My.Application.Info.DirectoryPath}\Settings.xml")
        Return doc.SelectSingleNode(xpath).InnerText
    End Function

End Class
