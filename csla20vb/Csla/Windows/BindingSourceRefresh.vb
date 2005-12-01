Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Windows.Forms

' code from Bill McCarthy
' http://msmvps.com/bill/archive/2005/10/05/69012.aspx
' used with permission

Namespace Windows

  <DesignerCategory("")> _
  <ProvideProperty("ReadValuesOnChange", GetType(BindingSource))> _
  Public Class BindingSourceRefresh
    Inherits System.ComponentModel.Component

    Implements IExtenderProvider

    Private mSources As New Dictionary(Of BindingSource, Boolean)

    Public Sub New(ByVal container As System.ComponentModel.IContainer)
      container.Add(Me)
    End Sub

    Public Function CanExtend(ByVal extendee As Object) As Boolean Implements IExtenderProvider.CanExtend

      If TypeOf extendee Is BindingSource Then
        Return True

      Else
        Return False
      End If

    End Function

    Public Function GetReadValuesOnChange(ByVal source As BindingSource) As Boolean
      If mSources.ContainsKey(source) Then
        Return mSources.Item(source)
      Else
        Return False
      End If
    End Function

    Public Sub SetReadValuesOnChange(ByVal source As BindingSource, ByVal value As Boolean)
      If mSources.ContainsKey(source) Then
        mSources.Item(source) = value
      Else
        mSources.Add(source, value)
      End If
      If value Then
        'hook
        AddHandler source.BindingComplete, AddressOf Source_BindingComplete
      Else
        'unhook
        RemoveHandler source.BindingComplete, AddressOf Source_BindingComplete
      End If
    End Sub

    Private Sub Source_BindingComplete(ByVal sender As Object, ByVal e As BindingCompleteEventArgs)
      e.Binding.ReadValue()
    End Sub

  End Class

End Namespace
