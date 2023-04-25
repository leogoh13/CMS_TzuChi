Imports System.Security.Cryptography

Public Class Hash_SHA1
    Public Shared Function HashSHA1(str As String) As String

        Dim result As String = ""
        Dim OSha1 As New SHA1CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(str)
        bytesToHash = OSha1.ComputeHash(bytesToHash)
        For Each item As Byte In bytesToHash
            result &= item.ToString("x2")
        Next

        Return result
    End Function
End Class
