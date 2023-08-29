Imports System.Data.SqlClient

Public Class SQL
    ReadOnly sqlConnStr As String
    Dim sqlCommand As SqlCommand
    Dim sqlDataReader As SqlDataReader
    ReadOnly sqlConnection As SqlConnection


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


    Public Sub ExcludeMelakaFomStojouRecord()
        Dim query = $"
                        SET IDENTITY_INSERT {GlobalDatabaseSchema}.TEMP_STOJOU_MELAKA ON;
                        INSERT INTO {GlobalDatabaseSchema}.TEMP_STOJOU_MELAKA ([BPRNUM_0]
                                   ,[VCRNUM_0]
                                   ,[QTYPCU_0]
                                   ,[UPDDATTIM_0]
                                   ,[SHLDAT_0]
                                   ,[CPRAMT_0]
                                   ,[CHECK_NUM]
                                   ,[ITMREF_0]
                                   ,[ITMDES1_0]
                                   ,[ITMDES2_0]
                                   ,[ITMID], [ROWID])
                        SELECT 
		                        [BPRNUM_0],
		                        [VCRNUM_0],
		                        [QTYPCU_0],
		                        [UPDDATTIM_0],
		                        [SHLDAT_0],
		                        [CPRAMT_0],
		                        [CHECK_NUM],
		                        [ITMREF_0],
		                        [ITMDES1_0],
		                        [ITMDES2_0],
		                        [ITMID],
		                        [ROWID]
                        FROM {GlobalDatabaseSchema}.TEMP_STOJOU
                        WHERE VCRNUM_0 LIKE '%F03%';
                        DELETE FROM {GlobalDatabaseSchema}.TEMP_STOJOU WHERE VCRNUM_0 LIKE '%F03%';
                    "

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)
            sqlCommand.ExecuteReader()
            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
        End Try
    End Sub

    Public Sub ExecuteAndReturnSTOJOURecords(ByRef obj As List(Of CxSYS_ISSUANCE))

        Dim query = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE, CREDATTIM_0 CREATEDATE
                        FROM {GlobalDatabaseSchema}.STOJOU 
                        WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU) AND TRSTYP_0 = 2"
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim issue As New CxSYS_ISSUANCE With {
                        .siteTo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SITES")),
                        .invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVNUM")),
                        .itemNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM")),
                        .quantity = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY")),
                        .cost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AMT")),
                        .expirationDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPDAT")),
                        .updateDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UPDATEDDATE")),
                        .createDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("CREATEDATE"))
                    }

                    obj.Add(issue)
                End While
            End Using

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
        End Try
    End Sub

    Public Sub ExecuteAndReturnEditedSTOJOURecords(ByRef obj As List(Of CxSYS_ISSUANCE))
        Dim query = $"  SELECT TOP 1 VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE, CREDATTIM_0 CREATEDATE
                        FROM {GlobalDatabaseSchema}.STOJOU 
                        WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU) AND TRSTYP_0 = 2
                        ORDER BY MVTSEQ_0"
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim issue As New CxSYS_ISSUANCE With {
                        .siteTo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SITES")),
                        .invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVNUM")),
                        .itemNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM")),
                        .quantity = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY")),
                        .cost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AMT")),
                        .expirationDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPDAT")),
                        .updateDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UPDATEDDATE")),
                        .createDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("CREATEDATE"))
                    }

                    obj.Add(issue)
                End While
            End Using
            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
        End Try
    End Sub


    Public Function CheckIssuanceEdited() As Boolean
        Dim obj As List(Of CxSYS_ISSUANCE) = Nothing
        Dim count As Int16
        Dim query = $"  SELECT VCRNUM_0 INVNUM
                        FROM {GlobalDatabaseSchema}.STOJOU 
                        WHERE 
                        VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU ) AND 
                        TRSTYP_0 = 2"
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    count += 1
                End While
            End Using

            sqlConnection.Close()

        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
        End Try

        If obj.Count > 1 Then
            Return True
        Else
            Return False
        End If
    End Function


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

    Public Sub GetNewProductList(ByRef obj As List(Of CxSYS_PRODUCT))
        Dim query As String = $"SELECT
                                DRUGTYP_0 DRUGTYPE, 
                                LEFT(TSICOD_1,1) PHARMACEUTICALFORM,
                                CASE WHEN ITMDES2_0 <> '' 
                                    THEN CONCAT(TRIM(ITMREF_0),' ',TRIM(ITMDES1_0), ' ', TRIM(ITMDES2_0))
                                    ELSE CONCAT(TRIM(ITMREF_0),' ',TRIM(ITMDES1_0)) 
                                END TRADENAME,
                                CONCAT(TRIM(ITMDES1_0), ' ', TRIM(ITMDES2_0)) GENERICNAME,
                                DES4AXX_0 DISPLAYNAME,
                                PLMITMREF_0 PURPOSE,
                                ITMREF_0 ITEMNUMBER, 
                                STU_0 MEASUREMENT,
                                SSUSTUCOE_0 UNIT,
                                DRUGCLS_0 DRUGCLASS, 
                                PURBASPRI_0 SALESPRICE, 
                                SAUSTUCOE_0 COSTPRICE, 
                                MINAGEMTH_0 AGELIMIT, 
                                DOSAGE_0 DOSAGE, 
                                DEFAULTQTY_0 DEFAULTQTY,
                                INDICATION_0 INDICATIONGUIDE, 
                                DOSAGEG_0 DOSAGEGUIDE
                            FROM {GlobalDatabaseSchema}.ITMMASTER
                            WHERE ITMREF_0 = (SELECT TOP 1 ITMREF_0 FROM {GlobalDatabaseSchema}.TEMP_ITMMASTER GROUP BY ITMREF_0)"

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim product As New CxSYS_PRODUCT With {
                        .type = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DRUGTYPE")),
                        .form = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("PHARMACEUTICALFORM")),
                        .trade_name = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("TRADENAME")),
                        .generic_name = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("GENERICNAME")),
                        .display_name = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DISPLAYNAME")),
                        .purpose = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("PURPOSE")),
                        .itemReference = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEMNUMBER")),
                        .measurement = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("MEASUREMENT")),
                        .unit = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UNIT")),
                        .itemClass = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DRUGCLASS")),
                        .sales_price = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SALESPRICE")),
                        .cost_price = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("COSTPRICE")),
                        .age_limit = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AGELIMIT")),
                        .dosage = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DOSAGE")),
                        .default_qty = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DEFAULTQTY")),
                        .indic_guide = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INDICATIONGUIDE")),
                        .dosage_guide = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DOSAGEGUIDE"))
                    }
                    obj.Add(product)
                End While
            End Using

            sqlConnection.Close()
        Catch ex As Exception
            sqlConnection.Close()
            Logger.WriteLine(ex.ToString & " | " & ex.Message)
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
