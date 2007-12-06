Imports System.ComponentModel
Imports System.Reflection

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
    Private mNonSerializableChangedHandlers As PropertyChangedEventHandler
    Private mSerializableChangedHandlers As PropertyChangedEventHandler

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
          mSerializableChangedHandlers = _
            DirectCast(System.Delegate.Combine( _
              mSerializableChangedHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableChangedHandlers = _
            DirectCast(System.Delegate.Combine( _
              mNonSerializableChangedHandlers, value), PropertyChangedEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          mSerializableChangedHandlers = DirectCast( _
            System.Delegate.Remove( _
              mSerializableChangedHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableChangedHandlers = DirectCast( _
            System.Delegate.Remove( _
              mNonSerializableChangedHandlers, value), PropertyChangedEventHandler)
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        If mNonSerializableChangedHandlers IsNot Nothing Then
          mNonSerializableChangedHandlers.Invoke(sender, e)
        End If
        If mSerializableChangedHandlers IsNot Nothing Then
          mSerializableChangedHandlers.Invoke(sender, e)
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
    Private mNonSerializableChangingHandlers As PropertyChangingEventHandler
    Private mSerializableChangingHandlers As PropertyChangingEventHandler

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
          mSerializableChangingHandlers = _
            DirectCast(System.Delegate.Combine( _
              mSerializableChangingHandlers, value), PropertyChangingEventHandler)
        Else
          mNonSerializableChangingHandlers = _
            DirectCast(System.Delegate.Combine( _
              mNonSerializableChangingHandlers, value), PropertyChangingEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangingEventHandler)
        If value.Method.IsPublic AndAlso _
          (value.Method.DeclaringType.IsSerializable OrElse _
          value.Method.IsStatic) Then
          mSerializableChangingHandlers = DirectCast( _
            System.Delegate.Remove( _
              mSerializableChangingHandlers, value), PropertyChangingEventHandler)
        Else
          mNonSerializableChangingHandlers = DirectCast( _
            System.Delegate.Remove( _
              mNonSerializableChangingHandlers, value), PropertyChangingEventHandler)
        End If
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangingEventArgs)
        If mNonSerializableChangingHandlers IsNot Nothing Then
          mNonSerializableChangingHandlers.Invoke(sender, e)
        End If
        If mSerializableChangingHandlers IsNot Nothing Then
          mSerializableChangingHandlers.Invoke(sender, e)
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
