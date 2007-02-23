<Serializable()> _
Public Class ProjectInfo
  Inherits ReadOnlyBase(Of ProjectInfo)

  Private mId As Guid
  Private mName As String

  Public Property Id() As Guid
    Get
      Return mId
    End Get
    Friend Set(ByVal value As Guid)
      mId = value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return mName
    End Get
    Friend Set(ByVal value As String)
      mName = value
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

  Public Overrides Function ToString() As String
    Return mName
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Friend Sub New(ByVal id As Guid, ByVal name As String)
    mId = id
    mName = name
  End Sub

End Class
