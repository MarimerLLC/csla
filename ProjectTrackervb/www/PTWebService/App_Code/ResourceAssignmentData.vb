Option Strict On

Imports Microsoft.VisualBasic

Public Class ResourceAssignmentData

  Private _projectId As Guid
  Private _projectName As String
  Private _assigned As String
  Private _role As Integer

  Public Property ProjectId() As Guid
    Get
      Return _projectId
    End Get
    Set(ByVal value As Guid)
      _projectId = value
    End Set
  End Property

  Public Property ProjectName() As String
    Get
      Return _projectName
    End Get
    Set(ByVal value As String)
      _projectName = value
    End Set
  End Property

  Public Property Assigned() As String
    Get
      Return _assigned
    End Get
    Set(ByVal value As String)
      _assigned = value
    End Set
  End Property

  Public Property Role() As Integer
    Get
      Return _role
    End Get
    Set(ByVal value As Integer)
      _role = value
    End Set
  End Property

End Class
