Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class SwitchableObject
  Inherits BusinessBase(Of SwitchableObject)

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

#Region "Root Factory Methods"

  Public Shared Function NewSwitchableObject() As SwitchableObject
    Return DataPortal.Create(Of SwitchableObject)()
  End Function

  Public Shared Function GetSwitchableObject(ByVal id As Integer) As SwitchableObject
    Return DataPortal.Fetch(Of SwitchableObject)(id)
  End Function

  Public Shared Sub DeleteSwitchableObject(ByVal id As Integer)
    DataPortal.Delete(Of SwitchableObject)(id)
  End Sub

#End Region

#Region "Child Factory Methods"

  Friend Shared Function NewSwitchableChild() As SwitchableObject
    Return DataPortal.CreateChild(Of SwitchableObject)()
  End Function

  Friend Shared Function GetSwitchableChild(ByVal childData As Object) As SwitchableObject
    Return DataPortal.FetchChild(Of SwitchableObject)(childData)
  End Function

  ' Require use of factory methods 
  Private Sub New()
  End Sub

#End Region

#Region "Root Data Access"

  <RunLocal()> _
  Protected Overloads Overrides Sub DataPortal_Create()
    ' TODO: load default values 
    ' omit this override if you have no defaults to set 
    MyBase.DataPortal_Create()
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Integer)
    ' TODO: load values 
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_Insert()
    ' TODO: insert values 
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_Update()
    ' TODO: update values 
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overloads Overrides Sub DataPortal_DeleteSelf()
    DataPortal_Delete(Me.Id)
  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Private Overloads Sub DataPortal_Delete(ByVal criteria As Integer)
    ' TODO: delete values 
  End Sub

#End Region

#Region "Child Data Access"

  Protected Overloads Overrides Sub Child_Create()
    ' TODO: load default values 
    ' omit this override if you have no defaults to set 
    MyBase.Child_Create()
  End Sub

  Private Sub Child_Fetch(ByVal childData As Object)
    ' TODO: load values 
  End Sub

  Private Sub Child_Insert(ByVal parent As Object)
    ' TODO: insert values 
  End Sub

  Private Sub Child_Update(ByVal parent As Object)
    ' TODO: update values 
  End Sub

  Private Sub Child_DeleteSelf(ByVal parent As Object)
    ' TODO: delete values 
  End Sub

#End Region
End Class
