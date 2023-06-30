Imports System.IO
Imports System.Data.SqlClient

Module Module1

    Public Const GlobalHashKey As String = "cx5ysSagei"
    Public Const SQLURL As String = "data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const SQLURLSERVER As String = "server=10.7.111.106:50001,10.7.111.104:50018;data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const FILEPATH As String = "E:\Sage\CxSysTest\Process"
    Public Const USER As String = "X3fcpudu"
    Public VendorID As String = XMLX.GetSingleValue("//CMS/VendorID")
    Public GlobalDatabaseSchema As String = XMLX.GetSingleValue("//database/databaseSchema")

    Public myConn As SqlConnection
    Public myCmd As SqlCommand
    Public myReader As SqlDataReader

    Sub Main()

        Logger.WriteLine("***********************************************************************************************************")

        Dim impersonator As New clsAuthenticator
        Dim RDP_Directory As String = XMLX.GetSingleValue("//RDP/Directory")
        Dim RDP_Domain As String = XMLX.GetSingleValue("//RDP/Domain")
        Dim RDP_Username As String = XMLX.GetSingleValue("//RDP/Username")
        Dim RDP_Password As String = XMLX.GetSingleValue("//RDP/Password")
        Dim json As New JSONGenerator()
        Dim retVal As String
        retVal = json.SaveProductID_EndpointC()
        retVal = json.GetIssuance_EndpointD()

        ' How to know that a user did an issuance?
        ' Check the creation and updated date?
        ' 







        Logger.WriteLine(retVal)
    End Sub

End Module

Public Class CMS_PCS

    Public Sub CreateOrUpdateProduct()
        Dim sql As New SQL()
        Dim products As New List(Of PCS_PRODUCT)
        Dim dt As String = DateTime.Now.ToString("yyyy-MM-dd-HH-mm")
        Dim destinationPath As String = XMLX.GetSingleValue("")


        sql.GetPCSItems(products)



        For Each itm In products

            ' Create strings to be written into the PCS files
            Dim pcsCreate As String = $"""CREATE""|""{itm.ItemReference}""|""{itm.ItemDescription1}""|""{itm.ItemDescription2}""|""{itm.ItemDescription3}""|""{itm.PackageUOM}""|{itm.Unit}|""{itm.StockUOM}""|{itm.MinStock}|{itm.MaxStock}|""{itm.Remark}"""



        Next

    End Sub

    Public Sub Issuance()

        Dim sql As New SQL()
        Dim issuances As New List(Of PCS_ISSUANCE)
        Dim dt As String = DateTime.Now.ToString("yyyy-MM-dd-HH-mm")
        Dim destinationPath As String = XMLX.GetSingleValue("")

        sql.GetPCSIssuance(issuances)

        For Each issue In issuances

            Dim remark As String = "-"
            Dim action As String
            If issue.qty > 0 Then
                action = "CREATE"
            Else
                action = "REVERSE"
            End If

            Dim pcsIssuance As String =
                $"""{action}""|""{issue.supplierCode}""|""{issue.invoiceNumber}""|{issue.invoiceDate}|{issue.purchaseOrderNo}|{issue.totalBillCost:0.00}|""{issue.itemCode}""|" &
                $"{issue.qty:0.00}|{issue.costPricePerUnit:0.000}|{issue.discount:0}|{issue.discountRM:0.000}|{issue.grossAmount:0.000}|{issue.expiryDate:dd/MM/yyyy}|""{remark}"""






        Next


    End Sub
End Class

Public Class PCS_ISSUANCE

    Public action As String
    Public supplierCode As String
    Public invoiceNumber As String
    Public invoiceDate As DateTime
    Public purchaseOrderNo As Integer
    Public totalBillCost As Decimal
    Public itemCode As String
    Public qty As Decimal
    Public costPricePerUnit As Decimal
    Public discount As Decimal
    Public discountRM As Decimal
    Public grossAmount As Decimal
    Public expiryDate As Date
    Public remarks As String

End Class

Public Class PCS_PRODUCT
    Public ItemReference As String
    Public Model As String
    Public ItemDescription1 As String
    Public ItemDescription2 As String
    Public ItemDescription3 As String
    Public PackageUOM As String
    Public Unit As String
    Public StockUOM As String
    Public MinStock As String
    Public MaxStock As String
    Public Remark As String
End Class

Public Class Logger
    Shared ReadOnly filepath As String = $"{My.Application.Info.DirectoryPath}\Logs\Log_{Date.UtcNow:yyyyMMdd}.log"
    Shared ReadOnly file As New FileInfo(filepath)
    Public Shared Sub WriteLine(str As String)
        If Not file.Exists() Then
            file.Create().Close()
        End If

        Dim datetime = Date.Now.ToString("dd-MM-yyyy hh:mm:ss tt")
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
    Public createDate As Date

End Class

Public Class CMS_PRODUCT
    Public userID As String
    Public hash As String
    Public type As String
    Public form As String
    Public trade_name As String
    Public generic_name As String
    Public display_name As String
    Public purpose As String
    Public measurement As String
    Public unit As String
    Public itemClass As String
    Public sales_price As String
    Public cost_price As String
    Public age_limit As String
    Public dosage As String
    Public default_qty As String
    Public indic_guide As String
    Public dosage_guide As String
    Public itemReference As String
End Class
