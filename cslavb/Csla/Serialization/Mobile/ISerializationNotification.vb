Imports System

Namespace Serialization.Mobile

  ''' <summary>
  ''' Interface defining callback methods used
  ''' by the MobileFormatter.
  ''' </summary>
  Public Interface ISerializationNotification

    ''' <summary>
    ''' Method called on an object after deserialization
    ''' is complete.
    ''' </summary>
    ''' <remarks>
    ''' This method is called on all objects in an
    ''' object graph after all the objects have been
    ''' deserialized.
    ''' </remarks>
    Sub Deserialized()

  End Interface

End Namespace

