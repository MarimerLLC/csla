Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Text

Namespace Windows

  ''' <summary>
  ''' Maintains a reference to a BindingSource object
  ''' on the form.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class BindingSourceNode

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="source">
    ''' BindingSource object to be mananaged.
    ''' </param>
    Public Sub New(ByVal source As BindingSource)
      _Source = source
      AddHandler _Source.CurrentChanged, AddressOf BindingSource_CurrentChanged
    End Sub

    Private _Source As BindingSource
    Private _Children As List(Of BindingSourceNode)
    Private _Parent As BindingSourceNode

    Sub BindingSource_CurrentChanged(ByVal sender As Object, ByVal e As EventArgs)
      If _Children.Count > 0 Then
        For Each child As BindingSourceNode In _Children
          child.Source.EndEdit()
        Next
      End If
    End Sub

    Friend ReadOnly Property Source() As BindingSource
      Get
        Return _Source
      End Get
    End Property

    Friend ReadOnly Property Children() As List(Of BindingSourceNode)
      Get
        If _Children Is Nothing Then
          _Children = New List(Of BindingSourceNode)()
        End If
        Return _Children
      End Get
    End Property

    Friend Property Parent() As BindingSourceNode
      Get
        Return _Parent
      End Get
      Set(ByVal value As BindingSourceNode)
        _Parent = value
      End Set
    End Property

    Friend Sub Unbind(ByVal cancel As Boolean)

      If _Source Is Nothing Then
        Return
      End If

      If _Children.Count > 0 Then
        For Each child As BindingSourceNode In _Children
          child.Unbind(cancel)
        Next
      End If

      Dim current As IEditableObject = TryCast(_Source.Current, IEditableObject)

      If Not TypeOf _Source.DataSource Is BindingSource Then
        _Source.DataSource = Nothing
      End If

      If current IsNot Nothing Then
        If cancel Then
          current.CancelEdit()
        Else
          current.EndEdit()
        End If
      End If

      If TypeOf _Source.DataSource Is BindingSource Then
        _Source.DataSource = _Parent.Source
      End If

    End Sub

    Friend Sub EndEdit()

      If _Source Is Nothing Then
        Return
      End If

      If _Children.Count > 0 Then
        For Each child As BindingSourceNode In _Children
          child.EndEdit()
        Next
      End If

      _Source.EndEdit()

    End Sub

    Friend Sub SetEvents(ByVal value As Boolean)

      If _Source Is Nothing Then
        Return
      End If

      _Source.RaiseListChangedEvents = value

      If _Children.Count > 0 Then
        For Each child As BindingSourceNode In _Children
          child.SetEvents(value)
        Next
      End If

    End Sub

    Friend Sub ResetBindings(ByVal refreshMetadata As Boolean)

      If _Source Is Nothing Then
        Return
      End If

      If _Children.Count > 0 Then
        For Each child As BindingSourceNode In _Children
          child.ResetBindings(refreshMetadata)
        Next
      End If

      _Source.ResetBindings(refreshMetadata)

    End Sub

    ''' <summary>
    ''' Binds a business object to the BindingSource.
    ''' </summary>
    ''' <param name="objectToBind">
    ''' Business object.
    ''' </param>
    Public Sub Bind(ByVal objectToBind As Object)
      Dim root As Csla.Core.ISupportUndo = TryCast(objectToBind, Csla.Core.ISupportUndo)
      If root IsNot Nothing Then
        root.BeginEdit()
      End If

      _Source.DataSource = objectToBind
      SetEvents(True)
      ResetBindings(False)
    End Sub

    ''' <summary>
    ''' Applies changes to the business object.
    ''' </summary>
    Public Sub Apply()

      SetEvents(False)

      Dim root As Csla.Core.ISupportUndo = TryCast(_Source.DataSource, Csla.Core.ISupportUndo)

      Unbind(False)
      EndEdit()

      If root IsNot Nothing Then
        root.ApplyEdit()
      End If

    End Sub

    ''' <summary>
    ''' Cancels changes to the business object.
    ''' </summary>
    ''' <param name="businessObject"></param>
    Public Sub Cancel(ByVal businessObject As Object)

      SetEvents(False)

      Dim root As Csla.Core.ISupportUndo = TryCast(_Source.DataSource, Csla.Core.ISupportUndo)

      Unbind(True)

      If root IsNot Nothing Then
        root.CancelEdit()
      End If

      Bind(businessObject)

    End Sub

    ''' <summary>
    ''' Disconnects from the BindingSource object.
    ''' </summary>
    Public Sub Close()

      SetEvents(False)
      Unbind(True)

    End Sub

  End Class

End Namespace
