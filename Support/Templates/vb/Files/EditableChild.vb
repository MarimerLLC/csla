Imports Csla.Security

<Serializable()>
Public Class EditableChild
  Inherits BusinessBase(Of EditableChild)

  Public Shared ReadOnly IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(NameOf(Id))
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(IdProperty, value)
    End Set
  End Property

  Public Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(NameOf(Name))
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(NameProperty, value)
    End Set
  End Property

  Protected Overrides Sub AddBusinessRules()
    'call base class implementation to add data annotation rules to BusinessRules 
    MyBase.AddBusinessRules()

    ' TODO: add validation rules
    'BusinessRules.AddRule(New MyRule, IdProperty)

  End Sub

  Public Shared Sub AddObjectAuthorizationRules()
    'TODO: add authorization rules
    'BusinessRules.AddRule(...)
  End Sub

  <CreateChild>
  Private Sub Create()
    'TODO: load default values
    'omit this override if you have no defaults to set
    BusinessRules.CheckRules()
  End Sub

  <FetchChild>
  Private Sub Fetch(ByVal childData As Object)
    ' TODO: load values
  End Sub

  <InsertChild>
  Private Sub Insert(ByVal parent As Object)
    ' TODO: insert values
  End Sub

  <UpdateChild>
  Private Sub Update(ByVal parent As Object)
    ' TODO: insert values
  End Sub

  <DeleteSelfChild>
  Private Sub DeleteSelf(ByVal parent As Object)
    ' TODO: delete values
  End Sub

End Class
