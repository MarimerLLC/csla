Imports System
Imports Csla
Imports Csla.Security

Namespace $rootnamespace$

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
      ' TODO: load values from childData 
    End Sub

#End Region
  End Class

End Namespace