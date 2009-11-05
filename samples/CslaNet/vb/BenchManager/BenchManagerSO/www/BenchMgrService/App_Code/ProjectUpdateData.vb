Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectUpdateData

  Private _projectId As Integer
  <DataMember()> _
  Public Property ProjectId() As Integer
    Get
      Return _projectId
    End Get
    Set(ByVal value As Integer)
      _projectId = value
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
