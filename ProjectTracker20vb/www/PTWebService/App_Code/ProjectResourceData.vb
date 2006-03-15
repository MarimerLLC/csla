Imports Microsoft.VisualBasic

Public Class ProjectResourceData

  Private mResourceId As Integer
  Private mFirstName As String
  Private mLastName As String
  Private mAssigned As String
  Private mRole As Integer

  Public Property ResourceId() As Integer
    Get
      Return mResourceId
    End Get
    Set(ByVal value As Integer)
      mResourceId = value
    End Set
  End Property

  Public Property FirstName() As String
    Get
      Return mFirstName
    End Get
    Set(ByVal value As String)
      mFirstName = value
    End Set
  End Property

  Public Property LastName() As String
    Get
      Return mLastName
    End Get
    Set(ByVal value As String)
      mLastName = value
    End Set
  End Property

  Public Property Assigned() As String
    Get
      Return mAssigned
    End Get
    Set(ByVal value As String)
      mAssigned = value
    End Set
  End Property

  Public Property Role() As Integer
    Get
      Return mRole
    End Get
    Set(ByVal value As Integer)
      mRole = value
    End Set
  End Property

End Class
