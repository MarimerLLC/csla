Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Windows.Forms

' code from Bill McCarthy
' http://msmvps.com/bill/archive/2005/10/05/69012.aspx
' used with permission

Namespace Windows

  ''' <summary>
  ''' Windows Forms extender control that resolves the
  ''' data refresh issue with data bound detail controls
  ''' as discussed in Chapter 5.
  ''' </summary>
  <DesignerCategory("")> _
  <ProvideProperty("ReadValuesOnChange", GetType(BindingSource))> _
  Public Class BindingSourceRefresh
    Inherits System.ComponentModel.Component

    Implements IExtenderProvider

    Private mSources As New Dictionary(Of BindingSource, Boolean)

    Public Sub New(ByVal container As System.ComponentModel.IContainer)
      container.Add(Me)
    End Sub

    ''' <summary>
    ''' Gets a value indicating whether the extender control
    ''' can extend the specified control.
    ''' </summary>
    ''' <param name="extendee">The control to be extended.</param>
    ''' <remarks>
    ''' This control only extends <see cref="BindingSource"/> controls.
    ''' </remarks>
    Public Function CanExtend(ByVal extendee As Object) As Boolean _
      Implements IExtenderProvider.CanExtend

      If TypeOf extendee Is BindingSource Then
        Return True

      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Gets the value of the custom ReadValuesOnChange extender
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    Public Function GetReadValuesOnChange(ByVal source As BindingSource) As Boolean
      If mSources.ContainsKey(source) Then
        Return mSources.Item(source)
      Else
        Return False
      End If
    End Function

    ''' <summary>
    ''' Sets the value of the custom ReadValuesOnChange extender
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    ''' <param name="value">New value of property.</param>
    ''' <remarks></remarks>
    Public Sub SetReadValuesOnChange( _
      ByVal source As BindingSource, ByVal value As Boolean)

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

    Private Sub Source_BindingComplete( _
      ByVal sender As Object, ByVal e As BindingCompleteEventArgs)

      e.Binding.ReadValue()
    End Sub

  End Class

End Namespace
