Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectData

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

End Class
