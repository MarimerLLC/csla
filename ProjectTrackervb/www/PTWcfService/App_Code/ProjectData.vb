Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization

<DataContract()> _
Public Class ProjectData

  Private mId As Guid
  Private mName As String
  Private mStarted As String
  Private mEnded As String
  Private mDescription As String
  Private mResources As List(Of ProjectResourceData) = New List(Of ProjectResourceData)()

  <DataMember()> _
  Public Property Id() As Guid
    Get
      Return mId
    End Get
    Set(ByVal value As Guid)
      mId = value
    End Set
  End Property

  <DataMember()> _
  Public Property Name() As String
    Get
      Return mName
    End Get
    Set(ByVal value As String)
      mName = value
    End Set
  End Property

  <DataMember()> _
  Public Property Started() As String
    Get
      Return mStarted
    End Get
    Set(ByVal value As String)
      mStarted = value
    End Set
  End Property

  <DataMember()> _
  Public Property Ended() As String
    Get
      Return mEnded
    End Get
    Set(ByVal value As String)
      mEnded = value
    End Set
  End Property

  <DataMember()> _
  Public Property Description() As String
    Get
      Return mDescription
    End Get
    Set(ByVal value As String)
      mDescription = value
    End Set
  End Property

  Public Sub AddResource(ByVal resource As ProjectResourceData)
    mResources.Add(resource)
  End Sub

  <DataMember()> _
  Public Property ProjectResources() As ProjectResourceData()
    Get
      If mResources.Count > 0 Then
        Return mResources.ToArray()
      End If
      Return Nothing
    End Get
    Set(ByVal value As ProjectResourceData())
      mResources = New List(Of ProjectResourceData)(value)
    End Set
  End Property

End Class
