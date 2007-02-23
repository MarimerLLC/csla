Option Strict On

Imports Microsoft.VisualBasic

Public Class ProjectData

  Private mId As Guid
  Private mName As String
  Private mStarted As String
  Private mEnded As String
  Private mDescription As String
  Private mResources As New Generic.List(Of ProjectResourceData)

  Public Property Id() As Guid
    Get
      Return mId
    End Get
    Set(ByVal value As Guid)
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

  Public Property Started() As String
    Get
      Return mStarted
    End Get
    Set(ByVal value As String)
      mStarted = value
    End Set
  End Property

  Public Property Ended() As String
    Get
      Return mEnded
    End Get
    Set(ByVal value As String)
      mEnded = value
    End Set
  End Property

  Public Property Description() As String
    Get
      Return mDescription
    End Get
    Set(ByVal value As String)
      mDescription = value
    End Set
  End Property

  Public Sub AddResource(ByVal resource As ProjectResourceData)
    mResources.Add(resource)
  End Sub

  Public Property ProjectResources() As ProjectResourceData()
    Get
      If mResources.Count > 0 Then
        Return mResources.ToArray

      Else
        Return Nothing
      End If
    End Get
    Set(ByVal value As ProjectResourceData())
      mResources = New Generic.List(Of ProjectResourceData)(value)
    End Set
  End Property

End Class