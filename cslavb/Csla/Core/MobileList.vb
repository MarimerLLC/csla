Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Serialization.Mobile
Imports Csla.Serialization
Imports System.IO

Namespace Core
  <Serializable()> _
  Public Class MobileList(Of T)
    Inherits List(Of T)
    Implements IMobileObject
    Public Sub New()
      MyBase.New()
    End Sub
    Public Sub New(ByVal capacity As Integer)
      MyBase.New(capacity)
    End Sub
    Public Sub New(ByVal collection As IEnumerable(Of T))
      MyBase.New(collection)
    End Sub

#Region "IMobileObject Members"

    Private Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      OnGetChildren(info, formatter)
    End Sub

    Private Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      OnGetState(info)
    End Sub

    Protected Overridable Sub OnGetState(ByVal info As SerializationInfo)
    End Sub

    Protected Overridable Sub OnGetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      Dim mobileChildren As Boolean = GetType(IMobileObject).IsAssignableFrom(GetType(T))
      If mobileChildren Then
        Dim references As New List(Of Integer)()
        For Each child As IMobileObject In Me
          Dim childInfo As SerializationInfo = formatter.SerializeObject(child)
          references.Add(childInfo.ReferenceId)
        Next
        If references.Count > 0 Then
          info.AddValue("$list", references)
        End If
      Else
        Using stream As New MemoryStream()
          Dim serializer As New System.Runtime.Serialization.DataContractSerializer([GetType]())
          serializer.WriteObject(stream, Me)
          stream.Flush()
          info.AddValue("$list", stream.ToArray())
        End Using
      End If
    End Sub

    Private Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
      OnSetState(info)
    End Sub

    Private Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      OnSetChildren(info, formatter)
    End Sub

    Protected Overridable Sub OnSetState(ByVal info As SerializationInfo)
    End Sub

    Protected Overridable Sub OnSetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      If info.Values.ContainsKey("$list") Then
        Dim mobileChildren As Boolean = GetType(IMobileObject).IsAssignableFrom(GetType(T))
        If mobileChildren Then
          Dim references As List(Of Integer) = DirectCast(info.Values("$list").Value, List(Of Integer))
          For Each reference As Integer In references
            Dim child As T = DirectCast(formatter.GetObject(reference), T)
            Me.Add(child)
          Next
        Else
          Dim buffer As Byte() = DirectCast(info.Values("$list").Value, Byte())
          Using stream As New MemoryStream(buffer)
            Dim dcs As New System.Runtime.Serialization.DataContractSerializer([GetType]())
            Dim list As MobileList(Of T) = DirectCast(dcs.ReadObject(stream), MobileList(Of T))
            AddRange(list)
          End Using
        End If
      End If
    End Sub

#End Region
  End Class
End Namespace