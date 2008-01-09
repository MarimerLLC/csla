''' <summary>
''' Exception indicating a failure to
''' set a property's field.
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class PropertyLoadException
  Inherits Exception

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="message">Text describing the exception.</param>
  Public Sub New(ByVal message As String)

    MyBase.New(message)

  End Sub

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="message">Text describing the exception.</param>
  ''' <param name="ex">Inner exception.</param>
  Public Sub New(ByVal message As String, ByVal ex As Exception)

    MyBase.New(message, ex)

  End Sub

  ''' <summary>
  ''' Creates an instance of the object for serialization.
  ''' </summary>
  ''' <param name="info">Serialiation info object.</param>
  ''' <param name="context">Serialization context object.</param>
  Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.New(info, context)

  End Sub

  ''' <summary>
  ''' Serializes the object.
  ''' </summary>
  ''' <param name="info">Serialiation info object.</param>
  ''' <param name="context">Serialization context object.</param>
  Public Overrides Sub GetObjectData( _
    ByVal info As System.Runtime.Serialization.SerializationInfo, _
    ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.GetObjectData(info, context)

  End Sub

End Class
