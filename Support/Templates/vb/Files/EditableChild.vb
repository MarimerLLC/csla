Imports Csla.Security

<Serializable()> _
Public Class EditableChild
  Inherits BusinessBase(Of EditableChild)

#Region " Business Methods "

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

#Region " Factory Methods "

  Friend Shared Function NewEditableChild() As EditableChild
    Return DataPortal.CreateChild(Of EditableChild)()
  End Function

  Friend Shared Function GetEditableChild(ByVal childData As Object) As EditableChild
    Return DataPortal.FetchChild(Of EditableChild)(childData)
  End Function

  Private Sub New()
    'Require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub Child_Create()
    'TODO: load default values
    'omit this override if you have no defaults to set
    MyBase.Child_Create()
  End Sub

  Private Sub Child_Fetch(ByVal childData As Object)
    ' TODO: load values
  End Sub

  Private Sub Child_Insert(ByVal parent As Object)
    ' TODO: insert values
  End Sub

  Private Sub Child_Update(ByVal parent As Object)
    ' TODO: insert values
  End Sub

  Friend Sub Child_DeleteSelf(ByVal parent As Object)
    ' TODO: delete values
  End Sub

#End Region

End Class
