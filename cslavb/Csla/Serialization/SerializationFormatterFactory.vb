Namespace Serialization

  ''' <summary>
  ''' Factory used to create the appropriate
  ''' serialization formatter object based
  ''' on the application configuration.
  ''' </summary>
  Public Module SerializationFormatterFactory

    ''' <summary>
    ''' Creates a serialization formatter object.
    ''' </summary>
    Public Function GetFormatter() As ISerializationFormatter
#If SILVERLIGHT Then
      Return New Csla.Serialization.Mobile.MobileFormatter()
#Else
      If ApplicationContext.SerializationFormatter = ApplicationContext.SerializationFormatters.BinaryFormatter Then
        Return New BinaryFormatterWrapper()

      Else
        Return New NetDataContractSerializerWrapper()
      End If
#End If

    End Function

  End Module

End Namespace
