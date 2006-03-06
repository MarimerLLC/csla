Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Core

  Friend Module ObjectCloner

    ''' <summary>
    ''' Clones an object by using the
    ''' <see cref="BinaryFormatter" />.
    ''' </summary>
    ''' <param name="obj">The object to clone.</param>
    ''' <remarks>
    ''' The object to be cloned must be serializable.
    ''' </remarks>
    Public Function Clone(ByVal obj As Object) As Object

      Using buffer As New MemoryStream()
        Dim formatter As New BinaryFormatter()

        formatter.Serialize(buffer, obj)
        buffer.Position = 0
        Dim temp As Object = formatter.Deserialize(buffer)
        Return temp
      End Using

    End Function

  End Module

End Namespace
