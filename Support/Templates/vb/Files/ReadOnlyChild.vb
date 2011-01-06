Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
Public Class ReadOnlyChild
  Inherits ReadOnlyBase(Of ReadOnlyChild)

#Region "Business Methods"

  ' TODO: add your own fields, properties and methods 

  ' example with managed backing field 
  Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(p) p.Id)
  Public ReadOnly Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
  End Property

  ' example with private backing field 
  Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(p) p.Name, Nothing, RelationshipTypes.PrivateField)
  Private _name As String = NameProperty.DefaultValue
  Public ReadOnly Property Name() As String
    Get
      Return GetProperty(NameProperty, _name)
    End Get
  End Property

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ' TODO: add authorization rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Public Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

#End Region

#Region "Factory Methods"

  Friend Shared Function GetReadOnlyChild(ByVal childData As Object) As ReadOnlyChild
    Return DataPortal.FetchChild(Of ReadOnlyChild)(childData)
  End Function

  ' require use of factory methods 
  Private Sub New()
  End Sub

#End Region

#Region "Data Access"

  Private Sub Child_Fetch(ByVal childData As Object)
    ' TODO: load values from childData 
  End Sub

#End Region
End Class
