Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class EditableRootParent
  Inherits BusinessBase(Of EditableRootParent)
#Region "Business Methods"

  ' TODO: add your own fields, properties and methods 

  ' example with private backing field 
  Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Id"))
  Private _Id As Integer = IdProperty.DefaultValue
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty, _Id)
    End Get
    Set(ByVal value As Integer)
      SetProperty(IdProperty, _Id, value)
    End Set
  End Property

  ' example with managed backing field 
  Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Name"))
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(NameProperty, value)
    End Set
  End Property

  Private Shared ChildListProperty As PropertyInfo(Of EditableChildList) = RegisterProperty(Of EditableChildList)(New PropertyInfo(Of EditableChildList)("ChildList", "Child list"))
  Public ReadOnly Property ChildList() As EditableChildList
    Get
      Return GetProperty(Of EditableChildList)(ChildListProperty)
    End Get
  End Property

  Private Shared ChildProperty As PropertyInfo(Of EditableChild) = RegisterProperty(New PropertyInfo(Of EditableChild)("Child", "Child"))
  Public ReadOnly Property Child() As EditableChild
    Get
      Return GetProperty(Of EditableChild)(ChildProperty)
    End Get
  End Property
#End Region

#Region "Validation Rules"

  Protected Overloads Overrides Sub AddBusinessRules()
    ' TODO: add validation rules 
    'ValidationRules.AddRule(RuleMethod, ""); 
  End Sub

#End Region

#Region "Authorization Rules"

  Protected Overloads Overrides Sub AddAuthorizationRules()
    ' TODO: add authorization rules 
    AuthorizationRules.AllowWrite(NameProperty, "Role")
  End Sub

  Private Shared Sub AddObjectAuthorizationRules()
    ' TODO: add authorization rules 
    'AuthorizationRules.AllowEdit(GetType(EditableRootParent), "Role")
  End Sub

#End Region

#Region "Factory Methods"

  Public Shared Function NewEditableRootParent() As EditableRootParent
    Return DataPortal.Create(Of EditableRootParent)()
  End Function

  Public Shared Function GetEditableRootParent(ByVal id As Integer) As EditableRootParent
    Return DataPortal.Fetch(Of EditableRootParent)(New SingleCriteria(Of EditableRootParent, Integer)(id))
  End Function

  Public Shared Sub DeleteEditableRootParent(ByVal id As Integer)
    DataPortal.Delete(New SingleCriteria(Of EditableRootParent, Integer)(id))
  End Sub

  ' Require use of factory methods 
  Private Sub New()
  End Sub

#End Region

#Region "Data Access"

  <RunLocal()> _
  Protected Overloads Overrides Sub DataPortal_Create()
    ' TODO: load default values 
    ' omit this override if you have no defaults to set 
    LoadProperty(ChildListProperty, EditableChildList.NewEditableChildList())
    LoadProperty(ChildProperty, EditableChild.NewEditableChild())
    MyBase.DataPortal_Create()
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of EditableRootParent, Integer))
    ' TODO: load values 
    LoadProperty(ChildListProperty, EditableChildList.GetEditableChildList(Nothing))
    LoadProperty(ChildProperty, EditableChild.GetEditableChild(Nothing))
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_Insert()
    ' TODO: insert values 
    FieldManager.UpdateChildren(Me)
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_Update()
    ' TODO: update values 
    FieldManager.UpdateChildren(Me)
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_DeleteSelf()
    DataPortal_Delete(New SingleCriteria(Of EditableRootParent, Integer)(Me.Id))
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As SingleCriteria(Of EditableRootParent, Integer))
    ' TODO: delete values 
    FieldManager.UpdateChildren(Me)
  End Sub

#End Region
End Class
