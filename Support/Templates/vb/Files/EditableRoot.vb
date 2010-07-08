<Serializable()> _
Public Class EditableRoot
  Inherits BusinessBase(Of EditableRoot)

#Region " Business Methods "

  ' TODO: add your own fields, properties and methods

  'example with private backing field
  Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Id", "Id"))
  Private _id As Integer = IdProperty.DefaultValue
  ''' <Summary>
  ''' Gets and sets the Id value.
  ''' </Summary>
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty, _id)
    End Get
    Set(ByVal value As Integer)
      SetProperty(IdProperty, _id, value)
    End Set
  End Property

  'example with managed backing field
  Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Name", "Name"))
  ''' <Summary>
  ''' Gets and sets the Name value.
  ''' </Summary>
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(NameProperty, value)
    End Set
  End Property

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()
    'call base class implementation to add data annotation rules to BusinessRules 
    MyBase.AddBusinessRules();

    ' TODO: add validation rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Public Shared ReadOnly Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewEditableRoot() As EditableRoot
    Return DataPortal.Create(Of EditableRoot)()
  End Function

  Public Shared Function GetEditableRoot(ByVal id As Integer) As EditableRoot
    Return DataPortal.Fetch(Of EditableRoot)(id)
  End Function

  Public Shared Sub DeleteEditableRoot(ByVal id As Integer)
    DataPortal.Delete(Of EditableRoot)(id)
  End Sub

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Create()
    ' TODO: load default values
    ' omit this override if you have no defaults to set
    MyBase.DataPortal_Create()
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Integer)
    ' load values
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Insert()
    ' TODO: insert values
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()
    ' TODO: update values
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_DeleteSelf()
    DataPortal_Delete(Id)
  End Sub

  Private Overloads Sub DataPortal_Delete(ByVal criteria As Integer)
    ' TODO: delete values
  End Sub

#End Region

End Class
