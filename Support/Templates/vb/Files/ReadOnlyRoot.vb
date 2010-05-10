Imports System
Imports Csla
Imports Csla.Security

<Serializable()> _
  Public Class ReadOnlyRoot
  Inherits ReadOnlyBase(Of ReadOnlyRoot)
#Region "Business Methods"

  ' TODO: add your own fields, properties and methods 

  ' example with managed backing field 
  Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Id", "Id"))
  Public ReadOnly Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
  End Property

  ' example with private backing field 
  Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Name", "Name"))
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

  Private Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

#End Region

#Region "Factory Methods"

  Public Shared Function GetReadOnlyRoot(ByVal id As Integer) As ReadOnlyRoot
    Return DataPortal.Fetch(Of ReadOnlyRoot)(id)
  End Function

  ' require use of factory methods 
  Private Sub New()
  End Sub

#End Region

#Region "Data Access"

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Integer)
    ' TODO: load values 
  End Sub

#End Region
End Class

