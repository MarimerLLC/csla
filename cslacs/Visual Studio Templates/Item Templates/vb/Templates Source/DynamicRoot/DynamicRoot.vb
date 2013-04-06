Imports Csla
Imports Csla.Security
Imports System

Namespace $rootnamespace$

  <Serializable()> _
  Public Class $safeitemname$
    Inherits BusinessBase(Of $safeitemname$)

#Region " Business Methods "

    ' TODO: add your own fields, properties and methods

    'example with private backing field
    Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Id", "Id"))
    Private _id As Integer = IdProperty.DefaultValue
    ''' <Summary>
    ''' Gets and sets the Id value.
    ''' </Summary>
    Public Property Id() As Integer
      Get
        Return GetProperty(IdProperty, _id)
      End Get
      Set(ByVal value As Integer)
        SetProperty(IdProperty, _id, value)
      End Set
    End Property

    'example with managed backing field
    Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Name", "Name"))
    ''' <Summary>
    ''' Gets and sets the Name value.
    ''' </Summary>
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(NameProperty, value)
      End Set
    End Property


#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      ' TODO: add validation rules
      'ValidationRules.AddRule(Nothing, "")

    End Sub

#End Region

#Region " Authorization Rules "

    Protected Overrides Sub AddAuthorizationRules()

      ' TODO: add authorization rules
      'AuthorizationRules.AllowWrite(NameProperty, "")

    End Sub

    Private Shared Sub AddObjectAuthorizationRules()
      'TODO: add authorization rules
      'AuthorizationRules.AllowEdit(GetType(DynamicRoot), "Role")
    End Sub

#End Region

#Region " Factory Methods "

    Public Shared Function New$safeitemname$() As $safeitemname$
      Return DataPortal.Create(Of $safeitemname$)()
    End Function

    Friend Shared Function Get$safeitemname$(ByVal rootData As Object) As $safeitemname$
      Return New $safeitemname$(rootData)
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

End Namespace