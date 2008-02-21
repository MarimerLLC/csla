Imports System.ComponentModel

Namespace Core

  ''' <summary>
  ''' This class implements INotifyPropertyChanged
  ''' and INotifyPropertyChanging in a 
  ''' serialization-safe manner.
  ''' </summary>
  <Serializable()> _
  Public MustInherit Class BindableBase

    Implements System.ComponentModel.INotifyPropertyChanged
    Implements System.ComponentModel.INotifyPropertyChanging

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

    End Sub

#Region " INotifyPropertyChanged "

    <NonSerialized()> _
    Private _nonSerializableChangedHandlers As PropertyChangedEventHandler
    Private _serializableChangedHandlers As PropertyChangedEventHandler

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
          _serializableChangedHandlers = _
            DirectCast(System.Delegate.Combine( _
              _serializableChangedHandlers, value), PropertyChangedEventHandler)
        Else
          _nonSerializableChangedHandlers = _
            DirectCast(System.Delegate.Combine( _
              _nonSerializableChangedHandlers, value), PropertyChangedEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          _serializableChangedHandlers = DirectCast( _
            System.Delegate.Remove( _
              _serializableChangedHandlers, value), PropertyChangedEventHandler)
        Else
          _nonSerializableChangedHandlers = DirectCast( _
            System.Delegate.Remove( _
              _nonSerializableChangedHandlers, value), PropertyChangedEventHandler)
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        If _nonSerializableChangedHandlers IsNot Nothing Then
          _nonSerializableChangedHandlers.Invoke(sender, e)
        End If
        If _serializableChangedHandlers IsNot Nothing Then
          _serializableChangedHandlers.Invoke(sender, e)
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
    ''' This method is automatically called by MarkDirty. It actually
    ''' raises a PropertyChanged event for an empty string, which
    ''' tells data binding to refresh all properties.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnUnknownPropertyChanged()

      RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(""))

    End Sub

    ''' <summary>
    ''' Call this method to raise the PropertyChanged event
    ''' for a specific property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property that
    ''' has changed.</param>
    ''' <remarks>
    ''' This method may be called by properties in the business
    ''' class to indicate the change in a specific property.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnPropertyChanged(ByVal propertyName As String)
      RaiseEvent PropertyChanged( _
        Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

#Region " INotifyPropertyChanging "

    <NonSerialized()> _
    Private _nonSerializableChangingHandlers As PropertyChangingEventHandler
    Private _serializableChangingHandlers As PropertyChangingEventHandler

    ''' <summary>
    ''' Implements a serialization-safe PropertyChanging event.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage( _
      "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Custom Event PropertyChanging As PropertyChangingEventHandler _
              Implements INotifyPropertyChanging.PropertyChanging
      AddHandler(ByVal value As PropertyChangingEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          _serializableChangingHandlers = _
            DirectCast(System.Delegate.Combine( _
              _serializableChangingHandlers, value), PropertyChangingEventHandler)
        Else
          _nonSerializableChangingHandlers = _
            DirectCast(System.Delegate.Combine( _
              _nonSerializableChangingHandlers, value), PropertyChangingEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangingEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          _serializableChangingHandlers = DirectCast( _
            System.Delegate.Remove( _
              _serializableChangingHandlers, value), PropertyChangingEventHandler)
        Else
          _nonSerializableChangingHandlers = DirectCast( _
            System.Delegate.Remove( _
              _nonSerializableChangingHandlers, value), PropertyChangingEventHandler)
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangingEventArgs)
        If _nonSerializableChangingHandlers IsNot Nothing Then
          _nonSerializableChangingHandlers.Invoke(sender, e)
        End If
        If _serializableChangingHandlers IsNot Nothing Then
          _serializableChangingHandlers.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Call this method to raise the PropertyChanging event
    ''' for all object properties.
    ''' </summary>
    ''' <remarks>
    ''' This method is for backward compatibility with
    ''' CSLA .NET 1.x.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnIsDirtyChanging()

      OnUnknownPropertyChanging()

    End Sub

    ''' <summary>
    ''' Call this method to raise the PropertyChanging event
    ''' for all object properties.
    ''' </summary>
    ''' <remarks>
    ''' This method is automatically called by MarkDirty. It actually
    ''' raises a PropertyChanging event for an empty string, which
    ''' tells data binding to refresh all properties.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnUnknownPropertyChanging()

      RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(""))

    End Sub

    ''' <summary>
    ''' Call this method to raise the PropertyChanging event
    ''' for a specific property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property that
    ''' has Changing.</param>
    ''' <remarks>
    ''' This method may be called by properties in the business
    ''' class to indicate the change in a specific property.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnPropertyChanging(ByVal propertyName As String)
      RaiseEvent PropertyChanging( _
        Me, New PropertyChangingEventArgs(propertyName))
    End Sub

#End Region

  End Class

End Namespace
