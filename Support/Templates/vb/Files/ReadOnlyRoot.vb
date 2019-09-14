Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class ReadOnlyRoot
  Inherits ReadOnlyBase(Of ReadOnlyRoot)

  Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(NameOf(Id))
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
    Private Set(ByVal value As Integer)
      LoadProperty(IdProperty, value)
    End Set
  End Property

  Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(NameOf(Name))
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Private Set(ByVal value As String)
      LoadProperty(NameProperty, value)
    End Set
  End Property

  Protected Overrides Sub AddBusinessRules()

    ' TODO: add authorization rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Public Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

  <Fetch>
  Private Sub Fetch(ByVal id As Integer)
    ' TODO: load values 
  End Sub

End Class

