Imports Csla.Security

<Serializable()> _
Public Class DynamicRoot
  Inherits BusinessBase(Of DynamicRoot)

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

  <Fetch>
  Private Sub Fetch(ByVal rootData As Object)
    MarkOld()
    'TODO: load values
  End Sub

  <Insert>
  Private Sub Insert()
    'TODO: insert values
  End Sub

  <Update>
  Private Sub Update()
    'TODO: update values
  End Sub

  <DeleteSelf>
  Private Sub DeleteSelf()
    'TODO: delete values
  End Sub

End Class
