<Serializable()> _
Public Class OrderInfo
  Inherits ReadOnlyBase(Of OrderInfo)

#Region " Business Methods "

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _customer As String = ""
  Public ReadOnly Property Customer() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _customer
    End Get
  End Property

  Private _lineItems As LineItemList = LineItemList.NewList
  Public ReadOnly Property LineItems() As LineItemList
    Get
      Return _lineItems
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return _id
  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function GetOrderInfo(ByVal data As SafeDataReader) As OrderInfo

    Return New OrderInfo(data)

  End Function

  Private Sub New()

  End Sub

  Private Sub New(ByVal data As SafeDataReader)

    Fetch(data)

  End Sub


#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal data As SafeDataReader)

    _id = data.GetInt32("Id")
    _customer = data.GetString("Customer")

  End Sub

  Friend Sub LoadItem(ByVal data As SafeDataReader)

    _lineItems.LoadChild(data)

  End Sub

  Friend Sub LoadDetail(ByVal data As SafeDataReader)

    _lineItems.FindById(data.GetInt32("LineId")).LoadDetail(data)

  End Sub

#End Region

End Class