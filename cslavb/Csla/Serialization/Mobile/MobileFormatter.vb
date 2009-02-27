Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Xml.Linq
Imports System.Linq
Imports System.Runtime.Serialization
Imports Csla.Validation
Imports Csla.Properties

Namespace Serialization.Mobile

  ''' <summary>
  ''' Serializes and deserializes objects
  ''' at the field level. A Silverlight-
  ''' compatible facsimile of the
  ''' BinaryFormatter or NetDataContractSerializer.
  ''' </summary>
#If TESTING Then
  [System.Diagnostics.DebuggerStepThrough]
#End If
  Public NotInheritable Class MobileFormatter
    Implements ISerializationFormatter

#Region "Serialize"

    ''' <summary>
    ''' Serialize an object graph into XML.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Stream to which the serialized data
    ''' will be written.
    ''' </param>
    ''' <param name="graph">
    ''' Root object of the object graph
    ''' to serialize.
    ''' </param>
    Public Sub Serialize(ByVal serializationStream As System.IO.Stream, ByVal graph As Object) Implements ISerializationFormatter.Serialize
      Dim writer As XmlWriter = XmlWriter.Create(serializationStream)
      Serialize(writer, graph)
      writer.Flush()
    End Sub

    ''' <summary>
    ''' Serialize an object graph into XML.
    ''' </summary>
    ''' <param name="textWriter">
    ''' TextWriter to which the serialized data
    ''' will be written.
    ''' </param>
    ''' <param name="graph">
    ''' Root object of the object graph
    ''' to serialize.
    ''' </param>
    Public Sub Serialize(ByVal textWriter As TextWriter, ByVal graph As Object)
      Dim writer As XmlWriter = XmlWriter.Create(textWriter)
      Serialize(writer, graph)
      writer.Flush()
    End Sub

    ''' <summary>
    ''' Serialize an object graph into XML.
    ''' </summary>
    ''' <param name="writer">
    ''' XmlWriter to which the serialized data
    ''' will be written.
    ''' </param>
    ''' <param name="graph">
    ''' Root object of the object graph
    ''' to serialize.
    ''' </param>
    Public Sub Serialize(ByVal writer As XmlWriter, ByVal graph As Object)
      _serializationReferences.Clear()
      SerializeObject(graph)
      Dim serialized As List(Of SerializationInfo) = _serializationReferences.Values.ToList()
      Dim dc As DataContractSerializer = GetDataContractSerializer()
      dc.WriteObject(writer, serialized)
    End Sub

    Private Function GetDataContractSerializer() As DataContractSerializer
      Return New DataContractSerializer( _
        GetType(List(Of SerializationInfo)), _
        New Type() {GetType(List(Of Integer)), GetType(Byte()), GetType(DateTimeOffset)})
    End Function

    ''' <summary>
    ''' Serializes an object into a SerializationInfo object.
    ''' </summary>
    ''' <param name="obj">Object to be serialized.</param>
    ''' <returns></returns>
    Public Function SerializeObject(ByVal obj As Object) As SerializationInfo
      Dim info As SerializationInfo = Nothing

      If obj Is Nothing Then
        info = New SerializationInfo(_serializationReferences.Count + 1)

      Else
        Dim thisType = obj.GetType()

        If IsSerializable(thisType) = False Then
          Throw New InvalidOperationException(String.Format(My.Resources.ObjectNotSerializableFormatted, thisType.FullName))
        End If

        Dim mobile = TryCast(obj, IMobileObject)

        If mobile Is Nothing Then
          Throw New InvalidOperationException(String.Format(My.Resources.MustImplementIMobileObject, thisType.Name))
        End If

        If Not _serializationReferences.TryGetValue(mobile, info) Then
          info = New SerializationInfo(_serializationReferences.Count + 1)
          _serializationReferences.Add(mobile, info)
          info.TypeName = thisType.AssemblyQualifiedName
          mobile.GetChildren(info, Me)
          mobile.GetState(info)
        End If
      End If

      Return info

    End Function

    Private _serializationReferences As Dictionary(Of IMobileObject, SerializationInfo) = New Dictionary(Of IMobileObject, SerializationInfo)

    Private Shared Function IsSerializable(ByVal objectType As Type) As Boolean
#If SILVERLIGHT Then
 Return objectType.IsSerializable()
#Else
      Return objectType.IsSerializable
#End If
    End Function

#End Region

#Region "Deserialize"

    Private _deserializationReferences As Dictionary(Of Integer, IMobileObject) = New Dictionary(Of Integer, IMobileObject)

    ''' <summary>
    ''' Deserialize an object from XML.
    ''' </summary>
    ''' <param name="serializationStream">
    ''' Stream containing the serialized XML
    ''' data.
    ''' </param>
    ''' <returns></returns>
    Public Function Deserialize(ByVal serializationStream As System.IO.Stream) As Object Implements ISerializationFormatter.Deserialize
      Dim reader As XmlReader = XmlReader.Create(serializationStream)
      Return Deserialize(reader)
    End Function

    ''' <summary>
    ''' Deserialize an object from XML.
    ''' </summary>
    ''' <param name="textReader">
    ''' TextReader containing the serialized XML
    ''' data.
    ''' </param>
    ''' <returns></returns>
    Public Function Deserialize(ByVal textReader As TextReader) As Object
      Dim reader As XmlReader = XmlReader.Create(textReader)
      Return Deserialize(reader)
    End Function

    ''' <summary>
    ''' Deserialize an object from XML.
    ''' </summary>
    ''' <param name="reader">
    ''' XmlReader containing the serialized XML
    ''' data.
    ''' </param>
    ''' <returns></returns>
    Public Function Deserialize(ByVal reader As XmlReader) As Object
      Dim dc As DataContractSerializer = GetDataContractSerializer()
      Dim deserialized As List(Of SerializationInfo) = CType(dc.ReadObject(reader), List(Of SerializationInfo))

      _deserializationReferences = New Dictionary(Of Integer, IMobileObject)

      For Each info As SerializationInfo In deserialized
        Dim type As Type = System.Type.GetType(info.TypeName)

        If type Is Nothing Then
          Throw New SerializationException(String.Format(My.Resources.MobileFormatterUnableToDeserialize, info.TypeName))
        End If

#If SILVERLIGHT Then
        Dim mobile As IMobileObject  = CType(Activator.CreateInstance(type), IMobileObject)
#Else
        Dim mobile As IMobileObject = CType(Activator.CreateInstance(type, True), IMobileObject)
#End If
        _deserializationReferences.Add(info.ReferenceId, mobile)

        mobile.SetState(info)

      Next

      For Each info As SerializationInfo In deserialized
        Dim mobile As IMobileObject = _deserializationReferences(info.ReferenceId)
        mobile.SetChildren(info, Me)
      Next

      For Each info In deserialized        
        Dim notifiable As ISerializationNotification = TryCast(_deserializationReferences(info.ReferenceId), ISerializationNotification)
        If notifiable IsNot Nothing Then
          notifiable.Deserialized()
        End If
      Next

      If _deserializationReferences.Count > 0 Then
        Return _deserializationReferences(1)
      Else
        Return Nothing
      End If

    End Function

    Public Function GetObject(ByVal referenceId As Integer) As IMobileObject
      Return _deserializationReferences(referenceId)
    End Function

#End Region

#Region "Static Helpers"

    ''' <summary>
    ''' Serializes the object into a byte array.
    ''' </summary>
    ''' <param name="obj">
    ''' The object to be serialized, which must implement
    ''' IMobileObject.
    ''' </param>
    ''' <returns></returns>
    Public Shared Function Serialize(ByVal obj As Object) As Byte()
      Using buffer = New System.IO.MemoryStream()
        Dim formatter = New MobileFormatter()
        formatter.Serialize(buffer, obj)
        Return buffer.ToArray()
      End Using
    End Function

    ''' <summary>
    ''' Deserializes a byte stream into an object.
    ''' </summary>
    ''' <param name="data">
    ''' Byte array containing the object's serialized
    ''' data.
    ''' </param>
    ''' <returns>
    ''' An object containing the data from the
    ''' byte stream. The object must implement
    ''' IMobileObject to be deserialized.
    ''' </returns>
    Public Shared Function Deserialize(ByVal data() As Byte) As Object
      Using buffer = New System.IO.MemoryStream(data)
        Dim formatter = New MobileFormatter()
        Return formatter.Deserialize(buffer)
      End Using
    End Function

#End Region

  End Class

End Namespace

