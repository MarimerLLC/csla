<Serializable()> _
Public Class LineItemDetailInfo
  Inherits ReadOnlyBase(Of LineItemDetailInfo)

#Region " Business Methods "



  Private Shared OrderIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.OrderId)
  ''' <Summary>
  ''' Gets the   value.
  ''' </Summary>
  Public Property OrderId() As Integer
    Get
      Return GetProperty(OrderIdProperty)
    End Get
    Private Set(ByVal value As Integer)
      LoadProperty(OrderIdProperty, value)
    End Set
  End Property


  Private Shared LineIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.LineId)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public Property LineId() As Integer
    Get
      Return GetProperty(LineIdProperty)
    End Get
    Private Set(ByVal value As Integer)
      LoadProperty(LineIdProperty, value)
    End Set
  End Property


  Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Id)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
    Private Set(ByVal value As Integer)
      LoadProperty(IdProperty, value)
    End Set
  End Property


  Private Shared DetailProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Detail)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public Property Detail() As String
    Get
      Return GetProperty(DetailProperty)
    End Get
    Private Set(ByVal value As String)
      LoadProperty(DetailProperty, value)
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return String.Format("{0}::{1}::{2}", OrderId, LineId, Id)
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

    LoadProperty(OrderIdProperty, data.GetInt32("OrderId"))
    LoadProperty(LineIdProperty, data.GetInt32("LineId"))
    LoadProperty(IdProperty, data.GetInt32("Id"))
    LoadProperty(DetailProperty, data.GetString("Detail"))

  End Sub

#End Region

End Class
