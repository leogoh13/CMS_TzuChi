Public Class CxSYS_ISSUANCE

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

Public Class CxSYS_PRODUCT
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
