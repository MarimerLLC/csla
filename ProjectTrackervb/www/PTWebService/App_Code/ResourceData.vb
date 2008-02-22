Option Strict On

Imports Microsoft.VisualBasic

Public Class ResourceData

  Private _id As Integer
  Private _name As String
  Private _projects As New Generic.List(Of ResourceAssignmentData)

  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property

  Public Sub AddProject(ByVal project As ResourceAssignmentData)
    _projects.Add(project)
  End Sub

  Public Property ResourceAssignments() As ResourceAssignmentData()
    Get
      If _projects.Count > 0 Then
        Return _projects.ToArray

      Else
        Return Nothing
      End If
    End Get
    Set(ByVal value As ResourceAssignmentData())
      _projects = New Generic.List(Of ResourceAssignmentData)(value)
    End Set
  End Property

End Class
