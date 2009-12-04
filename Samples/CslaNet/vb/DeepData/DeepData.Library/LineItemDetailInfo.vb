<Serializable()> _
Public Class LineItemDetailInfo
  Inherits ReadOnlyBase(Of LineItemDetailInfo)

#Region " Business Methods "

  Private _orderId As Integer
  Public ReadOnly Property OrderId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _orderId
    End Get
  End Property

  Private _lineId As Integer
  Public ReadOnly Property LineId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _lineId
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

  Private _detail As String = ""
  Public ReadOnly Property Detail() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _detail
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return String.Format("{0}::{1}::{2}", _orderId, _lineId, _id)
  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function GetItem(ByVal data As SafeDataReader) As LineItemDetailInfo

    Return New LineItemDetailInfo(data)

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
    _lineId = data.GetInt32("LineId")
    _id = data.GetInt32("Id")
    _detail = data.GetString("Detail")

  End Sub

#End Region

End Class
