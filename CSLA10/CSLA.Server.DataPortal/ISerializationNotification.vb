''' <summary>
''' Contains interfaces and classes to help support serialization
''' of objects.
''' </summary>
Namespace Serialization

  ''' <summary>
  ''' Objects can implement this interface if they wish to be
  ''' notified of serialization events.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Note that .NET serialization does NOT call these methods. Only
  ''' code that checks for the ISerializationNotification interface
  ''' when serializating and deserializing objects will invoke these
  ''' methods.
  ''' </para><para>
  ''' The CSLA .NET framework's DataPortal processing and the Clone
  ''' method in BusinessBase automatically make these calls.
  ''' </para>
  ''' </remarks>
  Public Interface ISerializationNotification
    ''' <summary>
    ''' This method is called before an object is serialized.
    ''' </summary>
    Sub Serializing()
    ''' <summary>
    ''' This method is called on the original instance of the
    ''' object after it has been serialized.
    ''' </summary>
    Sub Serialized()
    ''' <summary>
    ''' This method is called on a newly deserialized object
    ''' after deserialization is complete.
    ''' </summary>
    Sub Deserialized()
  End Interface

  ''' <summary>
  ''' Helper methods for invoking the ISerializatoinNotification
  ''' methods.
  ''' </summary>
  Public Class SerializationNotification

    ''' <summary>
    ''' Invokes the Serializing method on the target object
    ''' if it has implemented ISerializationNotification.
    ''' </summary>
    ''' <param name="target">Object on which the method should be invoked.</param>
    Public Shared Sub OnSerializing(ByVal target As Object)

      If TypeOf target Is ISerializationNotification Then
        DirectCast(target, ISerializationNotification).Serializing()
      End If

    End Sub

    ''' <summary>
    ''' Invokes the Serialized method on the target object
    ''' if it has implemented ISerializationNotification.
    ''' </summary>
    ''' <param name="target">Object on which the method should be invoked.</param>
    Public Shared Sub OnSerialized(ByVal target As Object)

      If TypeOf target Is ISerializationNotification Then
        DirectCast(target, ISerializationNotification).Serialized()
      End If

    End Sub

    ''' <summary>
    ''' Invokes the Deserialized method on the target object
    ''' if it has implemented ISerializationNotification.
    ''' </summary>
    ''' <param name="target">Object on which the method should be invoked.</param>
    Public Shared Sub OnDeserialized(ByVal target As Object)

      If TypeOf target Is ISerializationNotification Then
        DirectCast(target, ISerializationNotification).Deserialized()
      End If

    End Sub

  End Class

End Namespace
