Imports Microsoft.VisualBasic

Public Class ProjectRequest

  Private mId As Guid
  Public Property Id() As Guid
    Get
      Return mId
    End Get
    Set(ByVal value As Guid)
      mId = value
    End Set
  End Property

End Class
