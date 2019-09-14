Imports System
Imports Csla
Imports Csla.Security

<Serializable()>
Public Class EditableRootParent
  Inherits BusinessBase(Of EditableRootParent)

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

  Public Shared ReadOnly ChildListProperty As PropertyInfo(Of EditableChildList) = RegisterProperty(Of EditableChildList)(NameOf(ChildList), "Child list")
  Public ReadOnly Property ChildList() As EditableChildList
    Get
      Return GetProperty(Of EditableChildList)(ChildListProperty)
    End Get
  End Property

  Public Shared ReadOnly ChildProperty As PropertyInfo(Of EditableChild) = RegisterProperty(Of EditableChild)(NameOf(Child), "Child")
  Public ReadOnly Property Child() As EditableChild
    Get
      Return GetProperty(Of EditableChild)(ChildProperty)
    End Get
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
    LoadProperty(ChildListProperty, DataPortal.CreateChild(Of EditableChildList)())
    LoadProperty(ChildProperty, DataPortal.CreateChild(Of EditableChild)())
    BusinessRules.CheckRules()
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal id As Integer)
    ' TODO: load values 
    LoadProperty(ChildListProperty, DataPortal.FetchChild(Of EditableChildList)(Nothing))
    LoadProperty(ChildProperty, DataPortal.FetchChild(Of EditableChild)(Nothing))
  End Sub

  <Insert>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Sub Insert()
    ' TODO: insert values 
    FieldManager.UpdateChildren(Me)
  End Sub

  <Update>
  <Transactional(TransactionalTypes.TransactionScope)>
  Private Sub Update()
    ' TODO: update values 
    FieldManager.UpdateChildren(Me)
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
    FieldManager.UpdateChildren(Me)
  End Sub

End Class
