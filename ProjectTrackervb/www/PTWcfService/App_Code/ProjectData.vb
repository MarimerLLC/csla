Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectData

  Private _id As Guid
  Private _name As String
  Private _started As String
  Private _ended As String
  Private _description As String
  Private _resources As List(Of ProjectResourceData) = New List(Of ProjectResourceData)()

  <DataMember()> _
  Public Property Id() As Guid
    Get
      Return _id
    End Get
    Set(ByVal value As Guid)
      _id = value
    End Set
  End Property

  <DataMember()> _
  Public Property Name() As String
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property

  <DataMember()> _
  Public Property Started() As String
    Get
      Return _started
    End Get
    Set(ByVal value As String)
      _started = value
    End Set
  End Property

  <DataMember()> _
  Public Property Ended() As String
    Get
      Return _ended
    End Get
    Set(ByVal value As String)
      _ended = value
    End Set
  End Property

  <DataMember()> _
  Public Property Description() As String
    Get
      Return _description
    End Get
    Set(ByVal value As String)
      _description = value
    End Set
  End Property

  Public Sub AddResource(ByVal resource As ProjectResourceData)
    _resources.Add(resource)
  End Sub

  <DataMember()> _
  Public Property ProjectResources() As ProjectResourceData()
    Get
      If _resources.Count > 0 Then
        Return _resources.ToArray()
      End If
      Return Nothing
    End Get
    Set(ByVal value As ProjectResourceData())
      _resources = New List(Of ProjectResourceData)(value)
    End Set
  End Property

End Class
