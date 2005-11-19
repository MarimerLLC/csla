Imports System.ComponentModel
Imports System.Reflection

Namespace Core

  ''' <summary>
  ''' This base class declares the IsDirtyChanged event
  ''' to be NonSerialized so serialization will work.
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
    ''' Implements a serialization-safe IsDirtyChanged event.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Custom Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
      AddHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          mSerializableHandlers = DirectCast(System.Delegate.Combine(mSerializableHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableHandlers = DirectCast(System.Delegate.Combine(mNonSerializableHandlers, value), PropertyChangedEventHandler)
        End If
      End AddHandler

      RemoveHandler(ByVal value As PropertyChangedEventHandler)
        If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
          mSerializableHandlers = DirectCast(System.Delegate.Remove(mSerializableHandlers, value), PropertyChangedEventHandler)
        Else
          mNonSerializableHandlers = DirectCast(System.Delegate.Remove(mNonSerializableHandlers, value), PropertyChangedEventHandler)
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
    ''' for the IsDirty property.
    ''' </summary>
    ''' <remarks>
    ''' This method is automatically called by MarkDirty
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnIsDirtyChanged()

      Dim properties() As PropertyInfo = _
        Me.GetType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
      For Each item As PropertyInfo In properties
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(item.Name))
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
      RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

  End Class

End Namespace
