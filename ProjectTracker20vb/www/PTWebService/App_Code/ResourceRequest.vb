Imports Microsoft.VisualBasic

Public Class ResourceRequest

  Private mId As Integer
  Public Property Id() As Integer
    Get
      Return mId
    End Get
    Set(ByVal value As Integer)
      mId = value
    End Set
  End Property

End Class
