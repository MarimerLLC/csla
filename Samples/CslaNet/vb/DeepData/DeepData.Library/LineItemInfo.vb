<Serializable()> _
Public Class LineItemInfo
  Inherits ReadOnlyBase(Of LineItemInfo)

#Region " Business Methods "


  Private Shared OrderIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.OrderId)
  ''' <Summary>
  ''' Gets and sets the NewProperty value.
  ''' </Summary>
  Public Property OrderId() As Integer
    Get
      Return GetProperty(OrderIdProperty)
    End Get
    Private Set(ByVal value As Integer)
      LoadProperty(OrderIdProperty, value)
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



  Private Shared ProductProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Product)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public Property Product() As String
    Get
      Return GetProperty(ProductProperty)
    End Get
    Private Set(ByVal value As String)
      LoadProperty(ProductProperty, value)
    End Set
  End Property


  Private Shared DetailsProperty As PropertyInfo(Of LineItemDetailList) = RegisterProperty(Of LineItemDetailList)(Function(c) c.Details)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public ReadOnly Property Details() As LineItemDetailList
    Get
      If (Not FieldManager.FieldExists(DetailsProperty)) Then
        LoadProperty(DetailsProperty, LineItemDetailList.NewList)
      End If

      Return GetProperty(DetailsProperty)
    End Get
  End Property


  Protected Overrides Function GetIdValue() As Object
    Return String.Format("{0}::{1}", OrderId, Id)
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

    LoadProperty(OrderIdProperty, data.GetInt32("OrderId"))
    LoadProperty(IdProperty, data.GetInt32("Id"))
    LoadProperty(ProductProperty, data.GetString("Product"))

  End Sub

  Friend Sub LoadDetail(ByVal data As SafeDataReader)

    Details.LoadChild(data)

  End Sub

#End Region

End Class
