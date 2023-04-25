Imports System.Data.SqlClient

Public Class SQL

    Dim QPRODUCTID As String = "SELECT top 1 ITMREF_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU WHERE ITMID is not null and CHECK_NUM = '0';"
    Dim QDATE As String = "SELECT top 1 UPDDATTIM_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QINVOICENO As String = "SELECT top 1 VCRNUM_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QITEMID As String = "SELECT top 1 ITMID FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QQTY As String = "SELECT top 1 QTYPCU_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QEXPDA As String = "SELECT top 1 SHLDAT_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QCOST As String = "SELECT top 1 CPRAMT_0 FROM x3erpv12.YTCPROD.TEMP_STOJOU ;"
    Dim QCOUNTQTY As String = "SELECT COUNT(*) FROM x3erpv12.YTCPROD.TEMP_STOJOU WHERE ITMID is not null and CHECK_NUM = '0' and ITMREF_0 ='"
    Dim QCOUNT As String = "SELECT COUNT(*) FROM x3erpv12.YTCPROD.TEMP_STOJOU WHERE CHECK_NUM = '0' and ITMREF_0 ='"

    Dim sqlConnStr As String
    Dim dataSource As String
    Dim sqlCommand As SqlCommand
    Dim sqlDataReader As SqlDataReader
    Dim sqlConnection As SqlConnection
    Dim initialCatalog As String
    Dim xmlSettings As XMLX


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

    Public Function ExecuteQueryAndReturnValue(query As String) As List(Of String)
        Dim columns As New List(Of String)
        Dim rowList As New List(Of String)

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Console.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read

                Dim index As Integer = 0
                For Each column In columns
                    rowList.Add(sqlDataReader(column))
                Next
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Console.WriteLine(ex.Message)
            Console.WriteLine(ex.ToString)
        End Try

        Return rowList
    End Function

    Public Function ExecuteQueryAndReturnValue(query As String, list As List(Of String)) As List(Of String)
        Dim rowList As New List(Of String)

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlDataReader = sqlCommand.ExecuteReader()
            Console.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read

                Dim index As Integer = 0
                For Each column In list
                    rowList.Add(sqlDataReader(column))
                Next
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Console.WriteLine(ex.Message)
            Console.WriteLine(ex.ToString)
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
            Console.WriteLine("SQL Query Executed..")

            While sqlDataReader.Read
                str = Convert.ToString(sqlDataReader.GetInt32(0))
            End While

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Console.WriteLine(ex.Message)
            Console.WriteLine(ex.ToString)
        End Try

        Return str
    End Function

    Public Function GetPaymentHeader() As String
        Dim temp As String = Nothing
        Return temp
    End Function

End Class
