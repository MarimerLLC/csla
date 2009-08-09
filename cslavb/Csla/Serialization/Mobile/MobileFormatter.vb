Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Xml.Linq
Imports System.Linq
Imports System.Runtime.Serialization
Imports Csla.Validation
Imports Csla.Reflection

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
      ConvertEnumsToIntegers()
      Dim serialized As List(Of SerializationInfo) = _serializationReferences.Values.ToList()
      Dim dc As DataContractSerializer = GetDataContractSerializer()
      dc.WriteObject(writer, serialized)
    End Sub

    ''' <summary>
    ''' <para>
    ''' Converts any enum values in the <see cref="SerializationInfo" /> objects to
    ''' integer representations. Normally, <see cref="DataContractSerializer" /> requires
    ''' all non-standard primitive types to be provided to it's constructor both upon
    ''' serialization and deserialization. Since there is no way of knowing what enum
    ''' values are being deserialized, there is no way to supply the types to the constructor
    ''' at the time of deserialization.
    ''' </para>
    ''' <para>
    ''' Instead we convert the enum values to integers prior to serialization and then back
    ''' to proper enum objects after deserialization.
    ''' </para>
    ''' </summary>
    Private Sub ConvertEnumsToIntegers()

      For Each serializationInfo As SerializationInfo In _serializationReferences.Values
        For Each fieldData As SerializationInfo.FieldData In serializationInfo.Values.Values

          If fieldData.Value IsNot Nothing Then
            Dim fieldType As Type = fieldData.Value.GetType()

            If fieldType.IsEnum Then
#If SILVERLIGHT Then
              fieldData.Value = Convert.ChangeType(fieldData.Value, [Enum].GetUnderlyingType(fieldType), CultureInfo.CurrentCulture)
#Else
              fieldData.Value = Convert.ChangeType(fieldData.Value, [Enum].GetUnderlyingType(fieldType))
#End If
              fieldData.EnumTypeName = fieldType.AssemblyQualifiedName
            End If
          End If

        Next
      Next

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
        Dim nullPlaceholder As New NullPlaceholder

        info = New SerializationInfo(_serializationReferences.Count + 1)

        _serializationReferences.Add(nullPlaceholder, info)

        info.TypeName = GetType(NullPlaceholder).AssemblyQualifiedName

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
      Dim deserialized As List(Of SerializationInfo) = TryCast(dc.ReadObject(reader), List(Of SerializationInfo))

      _deserializationReferences = New Dictionary(Of Integer, IMobileObject)

      For Each info As SerializationInfo In deserialized
        Dim type As Type = Csla.Reflection.MethodCaller.GetType(info.TypeName)

        If type Is Nothing Then
          Throw New SerializationException(String.Format(My.Resources.MobileFormatterUnableToDeserialize, info.TypeName))
        ElseIf type Is GetType(NullPlaceholder) Then
          _deserializationReferences.Add(info.ReferenceId, Nothing)
        Else
#If SILVERLIGHT Then
        Dim mobile As IMobileObject  = CType(Activator.CreateInstance(type), IMobileObject)
#Else
          Dim mobile As IMobileObject = CType(Activator.CreateInstance(type, True), IMobileObject)
#End If
          _deserializationReferences.Add(info.ReferenceId, mobile)

          ConvertEnumsFromIntegers(info)
          mobile.SetState(info)
        End If

      Next

      For Each info As SerializationInfo In deserialized
        Dim mobile As IMobileObject = _deserializationReferences(info.ReferenceId)

        If mobile IsNot Nothing Then
          mobile.SetChildren(info, Me)
        End If

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

    ''' <summary>
    ''' Converts any enum values in the <see cref="SerializationInfo" /> object from their
    ''' integer representations to normal enum objects.
    ''' </summary>
    ''' <param name="serializationInfo"></param>
    ''' <remarks></remarks>
    Private Shared Sub ConvertEnumsFromIntegers(ByVal serializationInfo As SerializationInfo)
      For Each fieldData As SerializationInfo.FieldData In serializationInfo.Values.Values
        If Not String.IsNullOrEmpty(fieldData.EnumTypeName) Then
          Dim enumType As Type = MethodCaller.GetType(fieldData.EnumTypeName)
          fieldData.Value = [Enum].ToObject(enumType, fieldData.Value)
        End If
      Next
    End Sub

    ''' <summary>
    ''' Gets a deserialized object based on the object's
    ''' reference id within the serialization stream.
    ''' </summary>
    ''' <param name="referenceId">Id of object in stream.</param>
    ''' <returns></returns>
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

