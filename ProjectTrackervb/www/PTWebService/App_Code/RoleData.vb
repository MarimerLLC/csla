Option Strict On

Imports Microsoft.VisualBasic

Public Class RoleData

  Private _id As Integer
  Private _name As String

  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property

End Class
