Imports Csla
Imports Csla.Security

Namespace
  <Serializable()> _
  Public Class $safeitemname$
    Inherits EditableRootListBase(Of $childitem$)

#Region " Business Methods "

    Protected Overrides Function AddNewCore() As Object

      Dim item As $childitem$ = $childitem$.New$childitem$
      Add(item)
      Return item

    End Function
#End Region

#Region " Authorization Rules "

    Private Shared Sub AddObjectAuthorizationRules()
      'TODO: add authorization rules
      'AuthorizationRules.AllowGet(GetType($safeitemname$), "Role")
      'AuthorizationRules.AllowEdit(GetType($safeitemname$), "Role")
    End Sub

#End Region

#Region " Factory Methods "

    Public Shared Function New$safeitemname$() As $safeitemname$
      Return DataPortal.Create(Of $safeitemname$)()
    End Function

    Public Shared Function Get$safeitemname$() As $safeitemname$
      Return DataPortal.Fetch(Of $safeitemname$)()
    End Function

    Private Sub New()
      ' require use of factory methods
      AllowNew = True
    End Sub

#End Region

#Region " Data Access "

    Private Overloads Sub DataPortal_Fetch()

      ' TODO: load values
      RaiseListChangedEvents = False
      Dim listData As Object = Nothing
      For Each oneItemData As Object In CType(listData, List(Of Object))
        Add($childitem$.Get$childitem$(oneItemData))
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
      'AuthorizationRules.AllowWrite(NameProperty, "")

    End Sub

    Private Shared Sub AddObjectAuthorizationRules()
      'TODO: add authorization rules
      'AuthorizationRules.AllowEdit(GetType(DynamicRoot), "Role")
    End Sub

#End Region

#Region " Factory Methods "

    Public Shared Function New$childitem$() As $childitem$
      Return DataPortal.Create(Of $childitem$)()
    End Function

    Friend Shared Function Get$childitem$(ByVal rootData As Object) As $childitem$
      Return New $childitem$(rootData)
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