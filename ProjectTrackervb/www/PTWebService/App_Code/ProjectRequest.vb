Imports Microsoft.VisualBasic

Public Class ProjectRequest

  Private _id As Guid
  Public Property Id() As Guid
    Get
      Return _id
    End Get
    Set(ByVal value As Guid)
      _id = value
    End Set
  End Property

End Class
