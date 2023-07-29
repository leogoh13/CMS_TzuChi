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


