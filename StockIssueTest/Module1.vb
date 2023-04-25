Imports System.IO
Imports System.Text
Imports System.Data.SqlClient

Module Module1

    Public Const GlobalHashKey As String = "cx5ysSagei"
    Public Const SQLURL As String = "data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const SQLURLSERVER As String = "server=10.7.111.106:50001,10.7.111.104:50018;data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const FILEPATH As String = "E:\Sage\CxSysTest\Process"
    Public Const USER As String = "X3fcpudu"

    Public myConn As SqlConnection
    Public myCmd As SqlCommand
    Public myReader As SqlDataReader

    Sub Main()

        Dim impersonator As New clsAuthenticator
        Dim RDP_Directory As String = XMLX.GetSingleValue("//RDP/Directory")
        Dim RDP_Domain As String = XMLX.GetSingleValue("//RDP/Domain")
        Dim RDP_Username As String = XMLX.GetSingleValue("//RDP/Username")
        Dim RDP_Password As String = XMLX.GetSingleValue("//RDP/Password")


        'impersonator.Impersonator(RDP_Domain, RDP_Username, RDP_Password)
        'Dim dir As New IO.DirectoryInfo(RDP_Directory)
        'For Each file In dir.GetFiles()
        '    Console.WriteLine(file.FullName)
        'Next

        Dim json As New JSONGenerator()
        Dim retVal = json.GetIssuance_EndpointD()
        Console.WriteLine(retVal)

    End Sub


    Public Function FileSend(filepath As String)

    End Function

    Public Function Write(filepath As String, text As String)
        Dim FILE_NAME As String = filepath

        If System.IO.File.Exists(FILE_NAME) = True Then

            Dim fileStream As FileStream = New FileStream(FILE_NAME, FileMode.OpenOrCreate)

            Dim objWriter As New System.IO.StreamWriter(fileStream, Encoding.UTF8)

            objWriter.Write(text)

        Else

            System.IO.File.Create(FILE_NAME).Dispose()

            Dim file As System.IO.StreamWriter
            file = My.Computer.FileSystem.OpenTextFileWriter(filepath, True)
            file.WriteLine(text)
            file.Close()
        End If
    End Function

    Public Function GetQuery(query As String) As String
        Dim conn As SqlConnection
        conn = New SqlConnection(SQLURLSERVER)
        conn.Open()
        Dim results As String
        Dim parameterValue As Integer = 0
        Dim cmdCom As SqlCommand = conn.CreateCommand()
        cmdCom.CommandText = query
        myReader = cmdCom.ExecuteReader()
        Do While myReader.Read()
            results = myReader.GetString(parameterValue)
            parameterValue = parameterValue + 1
        Loop
        myReader.Close()

        conn.Close()
        Return results
    End Function

    Public Function GetQueryInt(query As String) As String
        Dim conn As SqlConnection
        conn = New SqlConnection(SQLURLSERVER)
        conn.Open()
        Dim results As String
        'Dim tempInt As Integer
        Dim parameterValue As Integer = 0
        Dim cmdCom As SqlCommand = conn.CreateCommand()
        cmdCom.CommandText = query
        myReader = cmdCom.ExecuteReader()
        Do While myReader.Read()
            results = If(myReader.IsDBNull(0), "0", myReader.GetByte(0).ToString)
        Loop
        myReader.Close()

        conn.Close()
        Return results
    End Function

    Public Function GetQueryDec(query As String) As String
        Dim conn As SqlConnection
        conn = New SqlConnection(SQLURLSERVER)
        conn.Open()
        Dim results As String
        'Dim tempInt As Integer
        Dim parameterValue As Integer = 0
        Dim cmdCom As SqlCommand = conn.CreateCommand()
        cmdCom.CommandText = query
        myReader = cmdCom.ExecuteReader()
        Do While myReader.Read()
            results = If(myReader.IsDBNull(0), "0", myReader.GetDecimal(0).ToString)
            'results = CStr(tempInt)

        Loop
        myReader.Close()

        conn.Close()
        Return results
    End Function

    Public Function appendStr(query As String, itmref As String) As String
        Dim result As String = query + itmref + "';"
        Return result
    End Function
End Module
