Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectDataUpdate

  Private _id As Integer
  <DataMember()> _
  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Private _clientId As Integer
  <DataMember()> _
  Public Property ClientId() As Integer
    Get
      Return _clientId
    End Get
    Set(ByVal value As Integer)
      _clientId = value
    End Set
  End Property

  Private _name As String
  <DataMember()> _
  Public Property Name() As String
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property

  Private _description As String
  <DataMember()> _
  Public Property Description() As String
    Get
      Return _description
    End Get
    Set(ByVal value As String)
      _description = value
    End Set
  End Property

  Private _delete As Boolean
  <DataMember()> _
  Public Property Delete() As Boolean
    Get
      Return _delete
    End Get
    Set(ByVal value As Boolean)
      _delete = value
    End Set
  End Property

  Private _isNew As Boolean
  <DataMember()> _
  Public Property IsNew() As Boolean
    Get
      Return _isNew
    End Get
    Set(ByVal value As Boolean)
      _isNew = value
    End Set
  End Property

End Class
