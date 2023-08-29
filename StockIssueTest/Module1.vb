Imports System.IO
Imports System.Text
Imports System.Data.SqlClient

Module Module1

    Public GlobalEnvironmentFlag As String = XMLX.GetSingleValue("//Environment")
    Public GlobalDatabaseSchema As String
    Public Const GlobalHashKey As String = "cx5ysSagei"
    Public VendorID As String = XMLX.GetSingleValue("//CMS/VendorID")


    Public myConn As SqlConnection
    Public myCmd As SqlCommand
    Public myReader As SqlDataReader
    Public GlobalEnvironment As String

    Sub Main()

        If GlobalEnvironmentFlag = "1" Then
            GlobalEnvironment = "Production"
        Else
            GlobalEnvironment = "Testing"
        End If

        GlobalDatabaseSchema = XMLX.GetSingleValue($"//{GlobalEnvironmentFlag}/database/databaseSchema")

        Logger.WriteLine("***********************************************************************************************************")

        Dim json As New JSONGenerator()
        Dim retVal As String
        retVal = json.SaveProductID_EndpointC()

        ' Exclude Melaka record for now
        If XMLX.GetSingleValue($"//{GlobalEnvironment}/API/ExcludeMelaka") = "1" Then
            Dim sql As New SQL()
            sql.ExcludeMelakaFomStojouRecord()
        End If

        retVal = json.GetIssuance_EndpointD()

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

    End Sub
End Class


