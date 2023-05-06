Imports System.IO
Imports System.Text
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.Logging

Module Module1

    Public Const GlobalHashKey As String = "cx5ysSagei"
    Public Const SQLURL As String = "data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const SQLURLSERVER As String = "server=10.7.111.106:50001,10.7.111.104:50018;data source=KLVM004\SAGEX3DATA;initial catalog=x3erpv12;user id=CMS;password=CMS@123;"
    Public Const FILEPATH As String = "E:\Sage\CxSysTest\Process"
    Public Const USER As String = "X3fcpudu"
    Public GlobalDatabaseSchema As String = XMLX.GetSingleValue("//database/databaseSchema")

    Public myConn As SqlConnection
    Public myCmd As SqlCommand
    Public myReader As SqlDataReader

    Sub Main()

        Dim impersonator As New clsAuthenticator
        Dim RDP_Directory As String = XMLX.GetSingleValue("//RDP/Directory")
        Dim RDP_Domain As String = XMLX.GetSingleValue("//RDP/Domain")
        Dim RDP_Username As String = XMLX.GetSingleValue("//RDP/Username")
        Dim RDP_Password As String = XMLX.GetSingleValue("//RDP/Password")
        Dim json As New JSONGenerator()
        Dim retVal As String = ""

        retVal = json.SaveProductID_EndpointC()
        retVal = json.GetIssuance_EndpointD()
        Logger.WriteLine(retVal)
    End Sub

End Module

Public Class CMS_PCS

    Public Sub CreateOrUpdateProduct()
        Dim sql As New SQL()
        Dim products As New List(Of PCS_PRODUCT)
        sql.GetPCSItems(products)

        For Each itm In products

            ' Create strings to be written into the PCS files
            Dim pcsCreateString = $"""CREATE""|""{itm.ItemReference}""|""{itm.ItemDescription1}""|""{itm.ItemDescription2}""|""{itm.ItemDescription3}""|""{itm.PackageUOM}""|{itm.Unit}|""{itm.StockUOM}""|{itm.MinStock}|{itm.MaxStock}|""{itm.Remark}"""

            Dim pcsUpdateString = $"""UPDATE"

        Next

    End Sub

    Public Sub Issuance()

    End Sub
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
    Shared filepath As String = $"{My.Application.Info.DirectoryPath}\Logs\Log_{Date.UtcNow.ToString("yyyyMMdd")}.log"
    Shared file As New FileInfo(filepath)
    Public Shared Sub WriteLine(str As String)
        If Not file.Exists() Then
            file.Create().Close()
        End If

        Dim datetime = Date.Now.ToString("ddMMyyyy hh:mm:ss tt")
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
