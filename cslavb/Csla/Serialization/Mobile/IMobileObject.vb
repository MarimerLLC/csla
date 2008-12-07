Imports System

Namespace Serialization.Mobile

  ''' <summary>
  ''' Interface to be implemented by any object
  ''' that supports serialization by the
  ''' MobileFormatter.
  ''' </summary>
  Public Interface IMobileObject

    ''' <summary>
    ''' Method called by MobileFormatter when an object
    ''' should serialize its data. The data should be
    ''' serialized into the SerializationInfo parameter.
    ''' </summary>
    ''' <param name="info">
    ''' Object to contain the serialized data.
    ''' </param>
    Sub GetState(ByVal info As SerializationInfo)

    ''' <summary>
    ''' Method called by MobileFormatter when an object
    ''' should serialize its child references. The data should be
    ''' serialized into the SerializationInfo parameter.
    ''' </summary>
    ''' <param name="info">
    ''' Object to contain the serialized data.
    ''' </param>
    ''' <param name="formatter">
    ''' Reference to the formatter performing the serialization.
    ''' </param>
    Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)

    ''' <summary>
    ''' Method called by MobileFormatter when an object
    ''' should be deserialized. The data should be
    ''' deserialized from the SerializationInfo parameter.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the serialized data.
    ''' </param>
    Sub SetState(ByVal info As SerializationInfo)


    ''' <summary>
    ''' Method called by MobileFormatter when an object
    ''' should deserialize its child references. The data should be
    ''' deserialized from the SerializationInfo parameter.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the serialized data.
    ''' </param>
    ''' <param name="formatter">
    ''' Reference to the formatter performing the deserialization.
    ''' </param>
    Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)


  End Interface

End Namespace

