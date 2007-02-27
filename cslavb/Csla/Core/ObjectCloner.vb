Imports System.IO
Imports Csla.Serialization

Namespace Core

  Friend Module ObjectCloner

    ''' <summary>
    ''' Clones an object.
    ''' </summary>
    ''' <param name="obj">The object to clone.</param>
    ''' <remarks>
    ''' <para>The object to be cloned must be serializable.</para>
    ''' <para>The serialization is performed using the formatter
    ''' specified in the application's configuration file
    ''' using the CslaSerializationFormatter key.</para>
    ''' <para>The default is to use the 
    ''' <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" />
    ''' </para>. You may also choose to use the Microsoft .NET 3.0
    ''' <see cref="System.Runtime.Serialization.NetDataContractSerializer">
    ''' NetDataContractSerializer</see> provided as part of WCF.
    ''' </remarks>
    Public Function Clone(ByVal obj As Object) As Object

      Using buffer As MemoryStream = New MemoryStream()
        Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter()
        formatter.Serialize(buffer, obj)
        buffer.Position = 0
        Dim temp As Object = formatter.Deserialize(buffer)
        Return temp
      End Using

    End Function

  End Module

End Namespace
