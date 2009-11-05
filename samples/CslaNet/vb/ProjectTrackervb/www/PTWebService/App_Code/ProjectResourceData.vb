Imports Microsoft.VisualBasic

Public Class ProjectResourceData

  Private _resourceId As Integer
  Private _firstName As String
  Private _lastName As String
  Private _assigned As String
  Private _role As Integer

  Public Property ResourceId() As Integer
    Get
      Return _resourceId
    End Get
    Set(ByVal value As Integer)
      _resourceId = value
    End Set
  End Property

  Public Property FirstName() As String
    Get
      Return _firstName
    End Get
    Set(ByVal value As String)
      _firstName = value
    End Set
  End Property

  Public Property LastName() As String
    Get
      Return _lastName
    End Get
    Set(ByVal value As String)
      _lastName = value
    End Set
  End Property

  Public Property Assigned() As String
    Get
      Return _assigned
    End Get
    Set(ByVal value As String)
      _assigned = value
    End Set
  End Property

  Public Property Role() As Integer
    Get
      Return _role
    End Get
    Set(ByVal value As Integer)
      _role = value
    End Set
  End Property

End Class
