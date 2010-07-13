<Serializable()> _
Public Class OrderInfo
  Inherits ReadOnlyBase(Of OrderInfo)

#Region " Business Methods "


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



  Private Shared CustomerProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Customer)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public Property Customer() As String
    Get
      Return GetProperty(CustomerProperty)
    End Get
    Private Set(ByVal value As String)
      LoadProperty(CustomerProperty, value)
    End Set
  End Property


  Private Shared LineItemsProperty As PropertyInfo(Of LineItemList) = RegisterProperty(Of LineItemList)(Function(c) c.LineItems)
  ''' <Summary>
  ''' Gets the NewProperty value.
  ''' </Summary>
  Public ReadOnly Property LineItems() As LineItemList
    Get
      If (Not FieldManager.FieldExists(LineItemsProperty)) Then
        LoadProperty(LineItemsProperty, LineItemList.NewList)
      End If

      Return GetProperty(LineItemsProperty)
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return Id
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

    LoadProperty(IdProperty, data.GetInt32("Id"))
    LoadProperty(CustomerProperty, data.GetString("Customer"))

  End Sub

  Friend Sub LoadItem(ByVal data As SafeDataReader)

    LineItems.LoadChild(data)

  End Sub

  Friend Sub LoadDetail(ByVal data As SafeDataReader)

    LineItems.FindById(data.GetInt32("LineId")).LoadDetail(data)

  End Sub

#End Region

End Class