Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class SwitchableObject
  Inherits BusinessBase(Of SwitchableObject)

  Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(NameOf(Id))
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(IdProperty, value)
    End Set
  End Property

  Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(NameOf(Name))
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(NameProperty, value)
    End Set
  End Property

  Protected Overrides Sub AddBusinessRules()
    'call base class implementation to add data annotation rules to BusinessRules 
    MyBase.AddBusinessRules()

    ' TODO: add validation rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Public Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

  <Create>
  <RunLocal()>
  Private Sub Create()
    ' TODO: load default values 
    ' omit this override if you have no defaults to set 
    BusinessRules.CheckRules()
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal id As Integer)
    ' TODO: load values 
  End Sub

  <Insert>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Sub Insert()
    ' TODO: insert values 
  End Sub

  <Update>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Sub Update()
    ' TODO: update values 
  End Sub

  <DeleteSelf>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Sub DeleteSelf()
    Delete(Me.Id)
  End Sub

  <Delete>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Shadows Sub Delete(ByVal id As Integer)
    ' TODO: delete values 
  End Sub

  Private Sub CreateChild()
    ' TODO: load default values 
    ' omit this override if you have no defaults to set 
    BusinessRules.CheckRules()
  End Sub

  Private Sub FetchChild(ByVal childData As Object)
    ' TODO: load values 
  End Sub

  Private Sub InsertChild(ByVal parent As Object)
    ' TODO: insert values 
  End Sub

  Private Sub UpdateChild(ByVal parent As Object)
    ' TODO: update values 
  End Sub

  Private Sub DeleteSelfChild(ByVal parent As Object)
    ' TODO: delete values 
  End Sub

End Class
