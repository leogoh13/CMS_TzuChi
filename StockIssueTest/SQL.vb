Imports System.Data.SqlClient

Public Class SQL

    Dim sqlConnStr As String
    Dim sqlCommand As SqlCommand
    Dim sqlDataReader As SqlDataReader
    Dim sqlConnection As SqlConnection


    Public Sub New()
        sqlConnStr =
            $"
                server={XMLX.GetSingleValue("//database/serverIP")};
                data source={XMLX.GetSingleValue("//database/dataSource")};
                initial catalog={XMLX.GetSingleValue("//database/initialCatalog")};
                user id={XMLX.GetSingleValue("//database/dbUserID")};
                password={XMLX.GetSingleValue("//database/dbPassword")};"
        sqlConnection = New SqlConnection(sqlConnStr)
    End Sub

    Public Sub ExecuteQueryReturnNull(query As String)
        Dim columns As New List(Of String)
        Dim rowList As New List(Of String)

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Logger.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read
                Dim index As Integer = 0
                For Each column In columns
                Next
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.Message)
            Logger.WriteLine(ex.ToString)
        End Try
    End Sub

    Public Function ExecuteQueryAndReturnValue(query As String) As List(Of String)
        Dim columns As New List(Of String)
        Dim rowList As New List(Of String)

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Logger.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read

                Dim index As Integer = 0
                For Each column In columns
                    rowList.Add(sqlDataReader(column))
                Next
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.Message)
            Logger.WriteLine(ex.ToString)
        End Try

        Return rowList
    End Function

    Public Function ExecuteQueryAndReturnValue(query As String, list As List(Of String)) As List(Of String)
        Dim rowList As New List(Of String)

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Logger.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read

                Dim index As Integer = 0
                For Each column In list
                    rowList.Add(sqlDataReader(column))
                Next
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.Message)
            Logger.WriteLine(ex.ToString)
        End Try

        Return rowList
    End Function

    Public Function ExecuteCustomQueryAndReturnValue(query) As String
        Dim str As String = Nothing
        Dim value
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Logger.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read
                str = Convert.ToString(sqlDataReader.GetInt32(0))
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.Message)
            Logger.WriteLine(ex.ToString)
        End Try

        Return str
    End Function

    Public Function GetPaymentHeader() As String
        Dim temp As String = Nothing
        Return temp
    End Function

End Class
