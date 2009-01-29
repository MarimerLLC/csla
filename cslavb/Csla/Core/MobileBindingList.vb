Imports System
Imports System.ComponentModel
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection
Imports Csla.Serialization
Imports Csla.Serialization.Mobile
Imports Csla.Core.FieldManager
Imports Csla.Core

Namespace Core

  ''' <summary>
  ''' Inherit from this base class to easily
  ''' create a serializable list class.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of the items contained in the list.
  ''' </typeparam>
  <Serializable()> _
  Public Class MobileBindingList(Of T)
    Inherits BindingList(Of T)
    Implements IMobileObject
#Region "IMobileObject Members"

    Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      OnGetChildren(info, formatter)
    End Sub

    Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      OnGetState(info)
    End Sub

    ''' <summary>
    ''' Override this method to get custom field values
    ''' from the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    Protected Overridable Sub OnGetState(ByVal info As SerializationInfo)
      info.AddValue("Csla.Core.MobileList.AllowEdit", AllowEdit)
      info.AddValue("Csla.Core.MobileList.AllowNew", AllowNew)
      info.AddValue("Csla.Core.MobileList.AllowRemove", AllowRemove)
      info.AddValue("Csla.Core.MobileList.RaiseListChangedEvents", RaiseListChangedEvents)
    End Sub

    ''' <summary>
    ''' Override this method to get custom child object
    ''' values from the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="formatter">Reference to the MobileFormatter.</param>
    Protected Overridable Sub OnGetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      If Not GetType(IMobileObject).IsAssignableFrom(GetType(T)) Then
        Throw New InvalidOperationException(My.Resources.CannotSerializeCollectionsNotOfIMobileObject)
      End If

      Dim references As New List(Of Integer)()
      For x As Integer = 0 To Me.Count - 1
        Dim child As T = Me(x)
        If child IsNot Nothing Then
          Dim childInfo As SerializationInfo = formatter.SerializeObject(child)
          references.Add(childInfo.ReferenceId)
        End If
      Next
      If references.Count > 0 Then
        info.AddValue("$list", references)
      End If
    End Sub

    Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
      OnSetState(info)
    End Sub

    Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      OnSetChildren(info, formatter)
    End Sub

    ''' <summary>
    ''' Override this method to set custom field values
    ''' into the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    Protected Overridable Sub OnSetState(ByVal info As SerializationInfo)
      AllowEdit = info.GetValue(Of Boolean)("Csla.Core.MobileList.AllowEdit")
      AllowNew = info.GetValue(Of Boolean)("Csla.Core.MobileList.AllowNew")
      AllowRemove = info.GetValue(Of Boolean)("Csla.Core.MobileList.AllowRemove")
      RaiseListChangedEvents = info.GetValue(Of Boolean)("Csla.Core.MobileList.RaiseListChangedEvents")
    End Sub

    ''' <summary>
    ''' Override this method to set custom child object
    ''' values into the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="formatter">Reference to the MobileFormatter.</param>
    Protected Overridable Sub OnSetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      If Not GetType(IMobileObject).IsAssignableFrom(GetType(T)) Then
        Throw New InvalidOperationException(My.Resources.CannotSerializeCollectionsNotOfIMobileObject)
      End If

      If info.Values.ContainsKey("$list") Then
        Dim references As List(Of Integer) = DirectCast(info.Values("$list").Value, List(Of Integer))
        For Each reference As Integer In references
          Dim child As T = DirectCast(formatter.GetObject(reference), T)
          Me.Add(child)
        Next
      End If
    End Sub

#End Region
  End Class
End Namespace