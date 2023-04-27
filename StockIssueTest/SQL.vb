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

    Public Sub ExecuteAndReturnSTOJOURecords(ByRef obj As List(Of CMS_ISSUANCE))
        Dim query = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE 
                        FROM {GlobalDatabaseSchema}.STOJOU 
                        WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU)"
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim issue As New CMS_ISSUANCE

                    issue.siteTo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SITES"))
                    issue.invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVNUM"))
                    issue.itemNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM"))
                    issue.quantity = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY"))
                    issue.cost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AMT"))
                    issue.expirationDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPDAT"))
                    issue.updateDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UPDATEDDATE"))

                    obj.Add(issue)
                End While
            End Using

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
        End Try
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
