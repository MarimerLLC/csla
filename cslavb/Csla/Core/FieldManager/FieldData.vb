Imports System
Imports System.ComponentModel

Namespace Core.FieldManager

  ''' <summary>
  ''' Contains a field value and related metadata.
  ''' </summary>
  ''' <typeparam name="T">Type of field value contained.</typeparam>
  <Serializable()> _
  Public Class FieldData(Of T)
    Implements IFieldData(Of T)

    Private _name As String
    Private _data As T
    Private _isDirty As Boolean

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="name">
    ''' Name of the field.
    ''' </param>
    Public Sub New(ByVal name As String)
      _name = name
    End Sub

    ''' <summary>
    ''' Gets the name of the field.
    ''' </summary>
    Public ReadOnly Property Name() As String Implements IFieldData.Name
      Get
        Return _name
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets the value of the field.
    ''' </summary>
    Public Overridable Property Value() As T Implements IFieldData(Of T).Value
      Get
        Return _data
      End Get
      Set(ByVal value As T)
        _data = value
        _isDirty = True
      End Set
    End Property

    Private Property IFieldData_Value() As Object Implements IFieldData.Value
      Get
        Return Me.Value
      End Get
      Set(ByVal value As Object)
        If value Is Nothing Then
          Me.Value = Nothing
        Else
          Me.Value = CType(value, T)
        End If
      End Set
    End Property

    Private ReadOnly Property IsDeleted() As Boolean Implements ITrackStatus.IsDeleted
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDeleted

        Else
          Return False
        End If
      End Get
    End Property

    Private ReadOnly Property IsSavable() As Boolean Implements ITrackStatus.IsSavable
      Get
        Return True
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the field
    ''' has been changed.
    ''' </summary>
    Public Overridable ReadOnly Property IsSelfDirty() As Boolean Implements ITrackStatus.IsSelfDirty
      Get
        Return IsDirty
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the field
    ''' has been changed.
    ''' </summary>
    Public Overridable ReadOnly Property IsDirty() As Boolean Implements ITrackStatus.IsDirty
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDirty

        Else
          Return _isDirty
        End If
      End Get
    End Property

    ''' <summary>
    ''' Marks the field as unchanged.
    ''' </summary>
    Public Sub MarkClean() Implements IFieldData.MarkClean

      _isDirty = False

    End Sub

    Private ReadOnly Property IsNew() As Boolean Implements ITrackStatus.IsNew
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsNew

        Else
          Return False
        End If
      End Get
    End Property

    Protected Overridable ReadOnly Property IsValid() As Boolean Implements ITrackStatus.IsValid, ITrackStatus.IsSelfValid
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsValid

        Else
          Return True
        End If
      End Get
    End Property

#Region " INotifyBusy Members "

    Private Custom Event INotifyBusy_BusyChanged As BusyChangedEventHandler Implements INotifyBusy.BusyChanged
      AddHandler(ByVal value As BusyChangedEventHandler)
        Throw New NotImplementedException
      End AddHandler

      RemoveHandler(ByVal value As BusyChangedEventHandler)
        Throw New NotImplementedException
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
        Throw New NotImplementedException
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Gets a value indicating whether this object or
    ''' any of its child objects are busy.
    ''' </summary>
    <Browsable(False)> _
    Public ReadOnly Property IsBusy() As Boolean Implements INotifyBusy.IsBusy
      Get
        Dim tmpIsBusy As Boolean = False
        Dim status As ITrackStatus = CType(_data, ITrackStatus)
        If status IsNot Nothing Then tmpIsBusy = status.IsBusy

        Return tmpIsBusy
      End Get
    End Property

    Public ReadOnly Property IsSelfBusy() As Boolean Implements INotifyBusy.IsSelfBusy
      Get
        Return IsBusy
      End Get
    End Property
#End Region

#Region "INotifyUnhandledAsyncException Members"

    <NotUndoable()> _
    <NonSerialized()> _
    Private _unhandledAsyncException As EventHandler(Of ErrorEventArgs)

    ''' <summary>
    ''' Event indicating that an exception occurred on
    ''' a background thread.
    ''' </summary>    
    Public Custom Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs) Implements INotifyUnhandledAsyncException.UnhandledAsyncException
      AddHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Remove(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As ErrorEventArgs)
        If _unhandledAsyncException IsNot Nothing Then
          _unhandledAsyncException.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the UnhandledAsyncException event.
    ''' </summary>
    ''' <param name="ex">Exception that occurred on the background thread.</param>    
    Protected Overridable Sub OnUnhandledAsyncException(ByVal ex As ErrorEventArgs)

      RaiseEvent UnhandledAsyncException(Me, ex)

    End Sub

    ''' <summary>
    ''' Raises the UnhandledAsyncException event.
    ''' </summary>
    ''' <param name="originalSender">Original source of the event.</param>
    ''' <param name="ex">Exception that occurred on the background thread.</param>    
    Protected Sub OnUnhandledAsyncException(ByVal originalSender As Object, ByVal ex As Exception)

      OnUnhandledAsyncException(New ErrorEventArgs(originalSender, ex))

    End Sub

#End Region

  End Class

End Namespace