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

<<<<<<< HEAD
    Public Sub ExecuteAndReturnSTOJOURecords(ByRef obj As List(Of CMS_ISSUANCE))
        Dim query = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE, CREDATTIM_0 CREATEDATE
=======
    Public Sub ExecuteAndReturnSTOJOURecords(ByRef obj As List(Of CxSYS_ISSUANCE))
        Dim query = $"SELECT VCRNUM_0 INVNUM, STOFCY_0 SITES, ITMREF_0 ITEM, QTYSTU_0 * -1 QTY, AMTORD_0 * -1 AMT, SHLDAT_0 EXPDAT, UPDDATTIM_0 UPDATEDDATE 
>>>>>>> Latest Changes
                        FROM {GlobalDatabaseSchema}.STOJOU 
                        WHERE VCRNUM_0 = ( SELECT TOP 1 VCRNUM_0 FROM {GlobalDatabaseSchema}.TEMP_STOJOU) AND TRSTYP_0 = 2"
        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
<<<<<<< HEAD
                    Dim issue As New CMS_ISSUANCE With {
                        .siteTo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SITES")),
                        .invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVNUM")),
                        .itemNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM")),
                        .quantity = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY")),
                        .cost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AMT")),
                        .expirationDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPDAT")),
                        .updateDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UPDATEDDATE")),
                        .createDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("CREATEDATE"))
                    }
=======
                    Dim issue As New CxSYS_ISSUANCE

                    issue.siteTo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SITES"))
                    issue.invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVNUM"))
                    issue.itemNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM"))
                    issue.quantity = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY"))
                    issue.cost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("AMT"))
                    issue.expirationDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPDAT"))
                    issue.updateDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("UPDATEDDATE"))
>>>>>>> Latest Changes

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
<<<<<<< HEAD
    Public Sub GetNewProductList(ByRef obj As List(Of CMS_PRODUCT))
=======
    Public Function GetNewProductList(ByRef obj As List(Of CxSYS_PRODUCT))
>>>>>>> Latest Changes
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
        'WHERE ITMREF_0 = (SELECT TOP 1 ITMREF_0 FROM {GlobalDatabaseSchema}.TEMP_ITMMASTER GROUP BY ITMREF_0)

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

    Public Sub GetPCSItems(ByRef obj As List(Of PCS_PRODUCT))
        Dim query = $"
                SELECT 
	                ITMREF_0 ItemReference, 
	                TSICOD_2 Model,
	                ITMDES1_0 ItemDescription1,
	                ITMDES2_0 ItemDescription2,
	                ITMDES3_0 ItemDescription3,
	                STU_0 PackageUOM,
	                SSUSTUCOE_0 Unit,
	                STU_0 StockUOM,
	                XMINSTOCK_0 MinStock,
	                XMAXSTOCK_0 MaxStock,
	                PLMITMREF_0 Remark
                FROM {GlobalDatabaseSchema}.ITMMASTER
                WHERE TSICOD_1 = 'CNMED' AND UPDDATTIM_0 >= DATEADD(MINUTE, -1, GETDATE())"

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim product As New PCS_PRODUCT With {
                        .ItemReference = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ItemReference")),
                        .Model = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("Model")),
                        .ItemDescription1 = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ItemDescription1")),
                        .ItemDescription2 = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ItemDescription2")),
                        .ItemDescription3 = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ItemDescription3")),
                        .PackageUOM = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("PackageUOM")),
                        .Unit = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("Unit")),
                        .StockUOM = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("StockUOM")),
                    .MinStock = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("MinStock")),
                    .MaxStock = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("MaxStock")),
                    .Remark = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("Remark"))
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

    Public Sub GetPCSIssuance(ByRef obj)
        Dim query As String = $"
                        SELECT 
	                        '' SUPPLIER_CODE,
	                        VCRNUM_0 INVOICE_NUMBER,
	                        UPDDATTIM_0 INVOICE_DATE,
	                        LOT_0 PO_NO,
	                        VARVAL_0 TOTAL_BILL_COST,
	                        ITMREF_0 ITEM_CODE,
	                        QTYSTU_0 * -1 QTY, 
	                        ( SELECT SUM(AMTORD_0 * -1) FROM YTCPROD.STOJOU A WHERE A.VCRNUM_0 = VCRNUM_0) COST_PRICE_PER_UNIT, 
	                        0 DISCOUNT,
	                        0 DISCOUNT_RM,
	                        AMTORD_0 * -1 GROSS_AMOUNT,
	                        SHLDAT_0 EXPIRY_DATE,
	                        '-' REMARKS
                        FROM YTCPROD.STOJOU 
                        WHERE VCRNUM_0 = ( SELECT VCRNUM_0 FROM YTCPROD.TEMP_STOJOU_PCS GROUP BY VCRNUM_0) AND TRSTYP_0 = 2"

        Try
            sqlConnection.Open()
            sqlCommand = New SqlCommand(query, sqlConnection)

            Using sqlDataReader = sqlCommand.ExecuteReader()
                While sqlDataReader.Read
                    Dim issuance As New PCS_ISSUANCE With {
                        .supplierCode = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("SUPPLIER_CODE")),
                        .invoiceNumber = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVOICE_NUMBER")),
                        .invoiceDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("INVOICE_DATE")),
                        .purchaseOrderNo = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("PO_NO")),
                        .totalBillCost = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("TOTAL_BILL_COST")),
                        .itemCode = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("ITEM_CODE")),
                        .qty = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("QTY")),
                        .costPricePerUnit = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("COST_PRICE_PER_UNIT")),
                        .discount = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DISCOUNT")),
                        .discountRM = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("DISCOUNT_RM")),
                        .grossAmount = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("GROSS_AMOUNT")),
                        .expiryDate = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("EXPIRY_DATE")),
                        .remarks = sqlDataReader.GetValue(sqlDataReader.GetOrdinal("REMARKS"))
                    }
                    obj.Add(issuance)
                End While
            End Using
            sqlConnection.Close()
        Catch ex As Exception

        End Try
    End Sub

End Class
