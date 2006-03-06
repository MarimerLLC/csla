Imports System.ComponentModel
Imports System.Reflection

Namespace Core

  ''' <summary>
  ''' This class implements INotifyPropertyChanged
  ''' in a serialization-safe manner.
  ''' </summary>
  <Serializable()> _
  Public MustInherit Class BindableBase

    Implements System.ComponentModel.INotifyPropertyChanged

    Protected Sub New()

    End Sub

    <NonSerialized()> _
    Private mNonSerializableHandlers As PropertyChangedEventHandler
    Private mSerializableHandlers As PropertyChangedEventHandler

    ''' <summary>
    ''' Implements a serialization-safe PropertyChanged event.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage( _
      "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Custom Event PropertyChanged As PropertyChangedEventHandler _
              Implements INotifyPropertyChanged.PropertyChanged
      AddHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          mSerializableHandlers = _
            DirectCast(System.Delegate.Combine( _
              mSerializableHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableHandlers = _
            DirectCast(System.Delegate.Combine( _
              mNonSerializableHandlers, value), PropertyChangedEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          mSerializableHandlers = DirectCast( _
            System.Delegate.Remove( _
              mSerializableHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableHandlers = DirectCast( _
            System.Delegate.Remove( _
              mNonSerializableHandlers, value), PropertyChangedEventHandler)
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        Dim nonSerializableHandlers As PropertyChangedEventHandler = _
          mNonSerializableHandlers
        If nonSerializableHandlers IsNot Nothing Then
          nonSerializableHandlers.Invoke(sender, e)
        End If
        Dim serializableHandlers As PropertyChangedEventHandler = _
          mSerializableHandlers
        If serializableHandlers IsNot Nothing Then
          serializableHandlers.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Call this method to raise the PropertyChanged event
    ''' for all object properties.
    ''' </summary>
    ''' <remarks>
    ''' This method is for backward compatibility with
    ''' CSLA .NET 1.x.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnIsDirtyChanged()

      OnUnknownPropertyChanged()

    End Sub

    ''' <summary>
    ''' Call this method to raise the PropertyChanged event
    ''' for all object properties.
    ''' </summary>
    ''' <remarks>
    ''' This method is automatically called by MarkDirty.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnUnknownPropertyChanged()

      Dim properties() As PropertyInfo = _
        Me.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
      For Each item As PropertyInfo In properties
        RaiseEvent PropertyChanged( _
          Me, New PropertyChangedEventArgs(item.Name))
      Next

    End Sub

    ''' <summary>
    ''' Call this method to raise the PropertyChanged event
    ''' for a specific property.
    ''' </summary>
    ''' <remarks>
    ''' This method may be called by properties in the business
    ''' class to indicate the change in a specific property.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnPropertyChanged(ByVal propertyName As String)
      RaiseEvent PropertyChanged( _
        Me, New PropertyChangedEventArgs(propertyName))
    End Sub

  End Class

End Namespace
