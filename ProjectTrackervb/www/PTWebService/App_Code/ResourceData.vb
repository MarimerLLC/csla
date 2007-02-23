Option Strict On

Imports Microsoft.VisualBasic

Public Class ResourceData

  Private mId As Integer
  Private mName As String
  Private mProjects As New Generic.List(Of ResourceAssignmentData)

  Public Property Id() As Integer
    Get
      Return mId
    End Get
    Set(ByVal value As Integer)
      mId = value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return mName
    End Get
    Set(ByVal value As String)
      mName = value
    End Set
  End Property

  Public Sub AddProject(ByVal project As ResourceAssignmentData)
    mProjects.Add(project)
  End Sub

  Public Property ResourceAssignments() As ResourceAssignmentData()
    Get
      If mProjects.Count > 0 Then
        Return mProjects.ToArray

      Else
        Return Nothing
      End If
    End Get
    Set(ByVal value As ResourceAssignmentData())
      mProjects = New Generic.List(Of ResourceAssignmentData)(value)
    End Set
  End Property

End Class
