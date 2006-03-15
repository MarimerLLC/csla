Option Strict On

Imports Microsoft.VisualBasic

Public Class ResourceAssignmentData

  Private mProjectId As Guid
  Private mProjectName As String
  Private mAssigned As String
  Private mRole As Integer

  Public Property ProjectId() As Guid
    Get
      Return mProjectId
    End Get
    Set(ByVal value As Guid)
      mProjectId = value
    End Set
  End Property

  Public Property ProjectName() As String
    Get
      Return mProjectName
    End Get
    Set(ByVal value As String)
      mProjectName = value
    End Set
  End Property

  Public Property Assigned() As String
    Get
      Return mAssigned
    End Get
    Set(ByVal value As String)
      mAssigned = value
    End Set
  End Property

  Public Property Role() As Integer
    Get
      Return mRole
    End Get
    Set(ByVal value As Integer)
      mRole = value
    End Set
  End Property

End Class
