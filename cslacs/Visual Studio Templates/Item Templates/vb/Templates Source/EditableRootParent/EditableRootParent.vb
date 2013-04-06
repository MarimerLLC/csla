Imports System
Imports System.Collections.Generic
Imports Csla

namespace $rootnamespace$

  <Serializable()> _
  Public Class $safeitemname$
    Inherits BusinessBase(Of $safeitemname$)

#Region "Business Methods"

    ' TODO: add your own fields, properties and methods 

    ' example with private backing field 
    Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Id"))
    Private _Id As Integer = IdProperty.DefaultValue
    Public Property Id() As Integer
      Get
        Return GetProperty(IdProperty, _Id)
      End Get
      Set(ByVal value As Integer)
        SetProperty(IdProperty, _Id, value)
      End Set
    End Property

    ' example with managed backing field 
    Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Name"))
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(NameProperty, value)
      End Set
    End Property

    Private Shared ChildListProperty As PropertyInfo(Of $childlist$) = RegisterProperty(Of $childlist$)(New PropertyInfo(Of $childlist$)("ChildList", "Child list"))
    Public ReadOnly Property ChildList() As $childlist$
      Get
        Return GetProperty(Of $childlist$)(ChildListProperty)
      End Get
    End Property

    Private Shared ChildProperty As PropertyInfo(Of $childitem$) = RegisterProperty(New PropertyInfo(Of $childitem$)("Child", "Child"))
    Public ReadOnly Property Child() As $childitem$
      Get
        Return GetProperty(Of $childitem$)(ChildProperty)
      End Get
    End Property
#End Region

#Region "Validation Rules"

    Protected Overloads Overrides Sub AddBusinessRules()
      ' TODO: add validation rules 
      'ValidationRules.AddRule(RuleMethod, ""); 
    End Sub

#End Region

#Region "Authorization Rules"

    Protected Overloads Overrides Sub AddAuthorizationRules()
      ' TODO: add authorization rules 
      AuthorizationRules.AllowWrite(NameProperty, "Role")
    End Sub

    Private Shared Sub AddObjectAuthorizationRules()
      ' TODO: add authorization rules 
      'AuthorizationRules.AllowEdit(GetType($childlist$), "Role")
    End Sub

#End Region

#Region "Factory Methods"

    Public Shared Function New$safeitemname$() As $safeitemname$
      Return DataPortal.Create(Of $safeitemname$)()
    End Function

    Public Shared Function Get$safeitemname$(ByVal id As Integer) As $safeitemname$
      Return DataPortal.Fetch(Of $safeitemname$)(New SingleCriteria(Of $safeitemname$, Integer)(id))
    End Function

    Public Shared Sub Delete$safeitemname$(ByVal id As Integer)
      DataPortal.Delete(New SingleCriteria(Of $safeitemname$, Integer)(id))
    End Sub

    ' Require use of factory methods 
    Private Sub New()
    End Sub

#End Region

#Region "Data Access"

    <RunLocal()> _
    Protected Overloads Overrides Sub DataPortal_Create()
      ' TODO: load default values 
      ' omit this override if you have no defaults to set 
      LoadProperty(ChildListProperty, $childlist$.New$childlist$())
      LoadProperty(ChildProperty, $childitem$.New$childitem$())
      MyBase.DataPortal_Create()
    End Sub

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of $safeitemname$, Integer))
      ' TODO: load values 
      LoadProperty(ChildListProperty, $childlist$.Get$childlist$(Nothing))
      LoadProperty(ChildProperty, $childitem$.Get$childitem$(Nothing))
    End Sub

    <Transactional(TransactionalTypes.TransactionScope)> _
    Protected Overloads Overrides Sub DataPortal_Insert()
      ' TODO: insert values 
      FieldManager.UpdateChildren(Me)
    End Sub

    <Transactional(TransactionalTypes.TransactionScope)> _
    Protected Overloads Overrides Sub DataPortal_Update()
      ' TODO: update values 
      FieldManager.UpdateChildren(Me)
    End Sub

    <Transactional(TransactionalTypes.TransactionScope)> _
    Protected Overloads Overrides Sub DataPortal_DeleteSelf()
      DataPortal_Delete(New SingleCriteria(Of $safeitemname$, Integer)(Me.Id))
    End Sub

    <Transactional(TransactionalTypes.TransactionScope)> _
    Private Overloads Sub DataPortal_Delete(ByVal criteria As SingleCriteria(Of $safeitemname$, Integer))
      ' TODO: delete values 
      FieldManager.UpdateChildren(Me)
    End Sub

#End Region
  End Class



  <Serializable()> _
  Public Class $childlist$
    Inherits BusinessListBase(Of $childlist$, $childitem$)

#Region " Factory Methods "

    Friend Shared Function New$childlist$() As $safeitemname$
      Return DataPortal.CreateChild(Of $childlist$)()
    End Function

    Friend Shared Function Get$childlist$(ByVal childData As Object) As $childlist$
      Return DataPortal.FetchChild(Of $childlist$)(childData)
    End Function

    Private Sub New()
      'use factory methods
    End Sub

#End Region

#Region " Data Access "

    Private Sub Child_Fetch(ByVal childData As Object)
      RaiseListChangedEvents = False
      For Each child As Object In CType(childData, IList(Of Object))
        Me.Add($childitem$.Get$childitem$(child))
      Next
      RaiseListChangedEvents = True
    End Sub

#End Region

  End Class


  <Serializable()> _
  Public Class $childitem$
    Inherits BusinessBase(Of $childitem$)

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
      AuthorizationRules.AllowWrite(NameProperty, "")

    End Sub

    Private Shared Sub AddObjectAuthorizationRules()
      'TODO: add authorization rules
      AuthorizationRules.AllowEdit(GetType($childitem$), "Role")
    End Sub

#End Region

#Region " Factory Methods "

    Friend Shared Function New$childitem$() As $childitem$
      Return DataPortal.CreateChild(Of $safeitemname$)()
    End Function

    Friend Shared Function Get$childitem$(ByVal childData As Object) As $childitem$
      Return DataPortal.FetchChild(Of $safeitemname$)(childData)
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

End Namespace