Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectSummaryData

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

End Class
