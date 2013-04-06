Imports System
Imports Csla
Imports Csla.Security

namespace $rootnamespace$

  <Serializable()> _
    Public Class $safeitemname$
    Inherits ReadOnlyBase(Of $safeitemname$)
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

#Region "Authorization Rules"

    Protected Overloads Overrides Sub AddAuthorizationRules()
      ' TODO: add authorization rules 
      'AuthorizationRules.AllowRead(NameProperty, "Role")
    End Sub

    Private Shared Sub AddObjectAuthorizationRules()
      ' TODO: add authorization rules 
      'AuthorizationRules.AllowGet(GetType($safeitemname$), "Role")
    End Sub

#End Region

#Region "Factory Methods"

    Public Shared Function Get$safeitemname$(ByVal id As Integer) As $safeitemname$
      Return DataPortal.Fetch(Of $safeitemname$)(New SingleCriteria(Of $safeitemname$, Integer)(id))
    End Function

    ' require use of factory methods 
    Private Sub New()
    End Sub

#End Region

#Region "Data Access"

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of $safeitemname$, Integer))
      ' TODO: load values 
    End Sub

#End Region
  End Class

End Namespace