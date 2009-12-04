Imports System
Imports System.Collections.Generic
Imports Csla
Imports Csla.Security

namespace $rootnamespace$

  <Serializable()> _
    Public Class $safeitemname$
    Inherits ReadOnlyListBase(Of $safeitemname$, $childitem$)

#Region "Authorization Rules"

    Private Shared Sub AddObjectAuthorizationRules()
      ' TODO: add authorization rules 
      'AuthorizationRules.AllowGet(GetType($safeitemname$), "Role")
    End Sub

#End Region

#Region "Factory Methods"

    Friend Shared Function Get$safeitemname$(ByVal childData As Object) As $safeitemname$
      Return DataPortal.FetchChild(Of $safeitemname$)(childData)
    End Function

    ' require use of factory methods 
    Private Sub New()
    End Sub

#End Region

#Region "Data Access"

    Private Sub Child_Fetch(ByVal childData As Object)
      RaiseListChangedEvents = False
      IsReadOnly = False
      ' TODO: load values 
      For Each child As Object In DirectCast(childData, List(Of Object))
        Add($childitem$.Get$childitem$(child))
      Next
      IsReadOnly = True
      RaiseListChangedEvents = True
    End Sub

#End Region
  End Class


  <Serializable()> _
  Public Class $childitem$
    Inherits ReadOnlyBase(Of $childitem$)

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

#Region "Factory Methods"

    Friend Shared Function Get$childitem$(ByVal childData As Object) As $childitem$
      Return DataPortal.FetchChild(Of $childitem$)(childData)
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

End Namespace