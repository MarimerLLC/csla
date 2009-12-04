Imports Microsoft.VisualBasic

Public Class ResourceRequest

  Private _id As Integer
  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

End Class
