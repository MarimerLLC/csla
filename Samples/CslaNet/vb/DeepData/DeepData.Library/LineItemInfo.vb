<Serializable()> _
Public Class LineItemInfo
  Inherits ReadOnlyBase(Of LineItemInfo)

#Region " Business Methods "

  Private _orderId As Integer
  Public ReadOnly Property OrderId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _orderId
    End Get
  End Property

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _product As String = ""
  Public ReadOnly Property Product() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _product
    End Get
  End Property

  Private _detailList As LineItemDetailList = LineItemDetailList.NewList
  Public ReadOnly Property Details() As LineItemDetailList
    Get
      Return _detailList
    End Get
  End Property


  Protected Overrides Function GetIdValue() As Object
    Return String.Format("{0}::{1}", _orderId, _id)
  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function GetItem(ByVal data As SafeDataReader) As LineItemInfo

    Return New LineItemInfo(data)

  End Function

  Private Sub New()

  End Sub

  Private Sub New(ByVal data As SafeDataReader)

    Fetch(data)

  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal data As SafeDataReader)

    _orderId = data.GetInt32("OrderId")
    _id = data.GetInt32("Id")
    _product = data.GetString("Product")

  End Sub

  Friend Sub LoadDetail(ByVal data As SafeDataReader)

    _detailList.LoadChild(data)

  End Sub

#End Region

End Class
