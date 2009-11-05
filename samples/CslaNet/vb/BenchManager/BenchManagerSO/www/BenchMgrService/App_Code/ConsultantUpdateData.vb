Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ConsultantUpdateData

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

  Private _onBench As Boolean
  <DataMember()> _
  Public Property OnBench() As Boolean
    Get
      Return _onBench
    End Get
    Set(ByVal value As Boolean)
      _onBench = value
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
