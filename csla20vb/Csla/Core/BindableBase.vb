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

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <remarks></remarks>
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
        If mNonSerializableHandlers IsNot Nothing Then
          mNonSerializableHandlers.Invoke(sender, e)
        End If
        If mSerializableHandlers IsNot Nothing Then
          mSerializableHandlers.Invoke(sender, e)
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

  End Class

End Namespace
