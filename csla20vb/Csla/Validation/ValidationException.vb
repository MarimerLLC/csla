Namespace Validation

  ''' <summary>
  ''' Exception class indicating that there was a validation
  ''' problem with a business object.
  ''' </summary>
  <Serializable()> _
  Public Class ValidationException
    Inherits Exception

    ''' <summary>
    ''' Initializes a new instance of the 
    ''' <see cref="T:Csla.ValidationException" /> class.
    ''' </summary>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Initializes a new instance of the 
    ''' <see cref="ValidationException" /> class
    ''' with a specified error message.
    ''' </summary>
    Public Sub New(ByVal message As String)

      MyBase.New(message)

    End Sub

    ''' <summary>
    ''' Initializes a new instance of the 
    ''' <see cref="T:Csla.ValidationException" /> class
    ''' with a specified error message and a reference to the 
    ''' inner exception that is the cause of this exception.
    ''' </summary>
    Public Sub New(ByVal message As String, ByVal innerException As Exception)

      MyBase.New(message, innerException)

    End Sub

    ''' <summary>
    ''' Initializes a new instance of the 
    ''' <see cref="T:Csla.ValidationException" /> class
    ''' with serialized data.
    ''' </summary>
    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)

    End Sub

  End Class

End Namespace
