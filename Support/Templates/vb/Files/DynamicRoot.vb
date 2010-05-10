Imports Csla.Security

<Serializable()> _
Public Class DynamicRoot
  Inherits BusinessBase(Of DynamicRoot)

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

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ' TODO: add validation rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Private Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewDynamicRoot() As DynamicRoot
    Return DataPortal.Create(Of DynamicRoot)()
  End Function

  Friend Shared Function GetDynamicRoot(ByVal rootData As Object) As DynamicRoot
    Return New DynamicRoot(rootData)
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
    MarkOld()
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
