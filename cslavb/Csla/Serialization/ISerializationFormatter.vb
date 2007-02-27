Namespace Serialization

  ''' <summary>
  ''' Defines an object that can serialize and deserialize
  ''' object graphs.
  ''' </summary>
  Public Interface ISerializationFormatter
    ''' <summary>
    ''' Converts a serialization stream into an
    ''' object graph.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Byte stream containing the serialized data.</param>
    ''' <returns>A deserialized object graph.</returns>
    Function Deserialize(ByVal serializationStream As System.IO.Stream) As Object
    ''' <summary>
    ''' Converts an object graph into a byte stream.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Stream that will contain the the serialized data.</param>
    ''' <param name="graph">Object graph to be serialized.</param>
    Sub Serialize(ByVal serializationStream As System.IO.Stream, ByVal graph As Object)
  End Interface

End Namespace
