<Serializable()> _
Public Class ProjectInfo
  Inherits ReadOnlyBase(Of ProjectInfo)

  Private _id As Guid
  Private _name As String

  Public Property Id() As Guid
    Get
      Return _id
    End Get
    Friend Set(ByVal value As Guid)
      _id = value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return _name
    End Get
    Friend Set(ByVal value As String)
      _name = value
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return _id
  End Function

  Public Overrides Function ToString() As String
    Return _name
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Friend Sub New(ByVal id As Guid, ByVal name As String)
    _id = id
    _name = name
  End Sub

End Class
