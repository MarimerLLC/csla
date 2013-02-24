Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class EditableRootParent
  Inherits BusinessBase(Of EditableRootParent)
#Region "Business Methods"

  ' TODO: add your own fields, properties and methods 

	' example with private backing field
	Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p) p.Id, RelationshipTypes.PrivateField)
	Private _Id As Integer = IdProperty.DefaultValue
	Public Property Id() As Integer
		Get
			Return GetProperty(IdProperty, _Id)
		End Get
		Set
			SetProperty(IdProperty, _Id, value)
		End Set
	End Property

	' example with managed backing field
	Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p) p.Name)
	Public Property Name() As String
		Get
			Return GetProperty(NameProperty)
		End Get
		Set
			SetProperty(NameProperty, value)
		End Set
	End Property

  Public Shared ReadOnly ChildListProperty As PropertyInfo(Of EditableChildList) = RegisterProperty(Of EditableChildList)(Function(p) p.ChildList, "Child list", RelationshipTypes.Child)
  Public ReadOnly Property ChildList() As EditableChildList
    Get
      Return GetProperty(Of EditableChildList)(ChildListProperty)
    End Get
  End Property

  Public Shared ReadOnly ChildProperty As PropertyInfo(Of EditableChild) = RegisterProperty(Of EditableChild)(Function(p) p.Child, "Child", RelationshipTypes.Child)
  Public ReadOnly Property Child() As EditableChild
    Get
      Return GetProperty(Of EditableChild)(ChildProperty)
    End Get
  End Property
#End Region

#Region " Business Rules "

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

#End Region

#Region "Factory Methods"

  Public Shared Function NewEditableRootParent() As EditableRootParent
    Return DataPortal.Create(Of EditableRootParent)()
  End Function

  Public Shared Function GetEditableRootParent(ByVal id As Integer) As EditableRootParent
    Return DataPortal.Fetch(Of EditableRootParent)(id)
  End Function

  Public Shared Sub DeleteEditableRootParent(ByVal id As Integer)
    DataPortal.Delete(Of EditableRootParent)(id)
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

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Integer)
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
    DataPortal_Delete(Me.Id)
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As Integer)
    ' TODO: delete values 
    FieldManager.UpdateChildren(Me)
  End Sub

#End Region
End Class
