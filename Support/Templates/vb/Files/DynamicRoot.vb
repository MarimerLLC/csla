Imports Csla.Security

<Serializable()> _
Public Class DynamicRoot
  Inherits BusinessBase(Of DynamicRoot)

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

  Public Shared Function NewDynamicRoot() As DynamicRoot
    Return DataPortal.Create(Of DynamicRoot)()
  End Function

  Friend Shared Function GetDynamicRoot(ByVal rootData As Object) As DynamicRoot
    Return New DynamicRoot(rootData)
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Private Sub New(ByVal rootData As Object)
    Fetch(rootData)
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal rootData As Object)
    MarkOld()
    'TODO: load values
  End Sub

  Protected Overrides Sub DataPortal_Insert()
    'TODO: insert values
  End Sub

  Protected Overrides Sub DataPortal_Update()
    'TODO: update values
  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()
    'TODO: delete values
  End Sub

#End Region

End Class
