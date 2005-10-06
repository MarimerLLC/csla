Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Friend NotInheritable Class ObjectCloner

  Private Sub New()

  End Sub

  Public Shared Function Clone(ByVal obj As Object) As Object

    Using buffer As New MemoryStream()
      Dim formatter As New BinaryFormatter()

      formatter.Serialize(buffer, obj)
      buffer.Position = 0
      Dim temp As Object = formatter.Deserialize(buffer)
      Return temp
    End Using

  End Function

End Class
