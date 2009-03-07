Namespace Validation

  ''' <summary>
  ''' Exception class indicating that there was a validation
  ''' problem with a business object.
  ''' </summary>
  <Serializable()> _
  Public Class ValidationException
    Inherits Exception

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">Message describing the exception.</param>
    Public Sub New(ByVal message As String)

      MyBase.New(message)

    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">Message describing the exception.</param>
    ''' <param name="innerException">Inner exception object.</param>
    Public Sub New(ByVal message As String, ByVal innerException As Exception)

      MyBase.New(message, innerException)

    End Sub

#If Not Silverlight Then
    ''' <summary>
    ''' Creates an instance of the object for serialization.
    ''' </summary>
    ''' <param name="context">Serialization context.</param>
    ''' <param name="info">Serialization info.</param>
    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)

    End Sub
#End If


  End Class

End Namespace
