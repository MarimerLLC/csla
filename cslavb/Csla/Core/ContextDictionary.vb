Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Collections.Generic
Imports Csla.Serialization.Mobile
Imports System.Collections.Specialized

Namespace Core
  ''' <summary> 
  ''' Dictionary type that is serializable 
  ''' with the MobileFormatter. 
  ''' </summary> 
  <Serializable()> _
  Public Class ContextDictionary
    Inherits HybridDictionary
    Implements IMobileObject

#Region "IMobileObject Members"

    Private Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      For Each key As String In Me.Keys
        Dim value As Object = Me(key)
        If Not (TypeOf value Is IMobileObject) Then
          info.AddValue(key, value)
        End If
      Next
    End Sub

    Private Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      For Each key As String In Me.Keys
        Dim value As Object = Me(key)
        Dim mobile As IMobileObject = TryCast(value, IMobileObject)
        If mobile IsNot Nothing Then
          Dim si As SerializationInfo = formatter.SerializeObject(mobile)
          info.AddChild(key, si.ReferenceId)
        End If
      Next
    End Sub

    Private Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
      For Each key As String In info.Values.Keys
        Add(key, info.Values(key).Value)
      Next
    End Sub

    Private Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      For Each key As String In info.Children.Keys
        Dim referenceId As Integer = info.Children(key).ReferenceId
        Me.Add(key, formatter.GetObject(referenceId))
      Next
    End Sub

#End Region
  End Class
End Namespace