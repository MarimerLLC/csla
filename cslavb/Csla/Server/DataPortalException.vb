Imports System.Security.Permissions

Namespace Server

  ''' <summary>
  ''' This exception is returned from the 
  ''' server-side DataPortal and contains the exception
  ''' and context data from the server.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")> <Serializable()> _
  Public Class DataPortalException
    Inherits Exception

    Private _result As DataPortalResult
    Private _innerStackTrace As String

    ''' <summary>
    ''' Returns the DataPortalResult object from the server.
    ''' </summary>
    Public ReadOnly Property Result() As DataPortalResult
      Get
        Return _result
      End Get
    End Property

    ''' <summary>
    ''' Get the server-side stack trace from the
    ''' original exception.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.String.Format(System.String,System.Object,System.Object,System.Object)")> _
    Public Overrides ReadOnly Property StackTrace() As String
      Get
        Return String.Format("{0}{1}{2}", _innerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">Text describing the exception.</param>
    ''' <param name="ex">Inner exception.</param>
    ''' <param name="result">The data portal result object.</param>
    Public Sub New( _
      ByVal message As String, ByVal ex As Exception, _
      ByVal result As DataPortalResult)

      MyBase.New(message, ex)
      _innerStackTrace = ex.StackTrace
      _result = result

    End Sub

    ''' <summary>
    ''' Creates an instance of the object for serialization.
    ''' </summary>
    ''' <param name="info">Serialiation info object.</param>
    ''' <param name="context">Serialization context object.</param>
    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      _result = DirectCast(info.GetValue("mResult", GetType(DataPortalResult)), DataPortalResult)
      _innerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    ''' <summary>
    ''' Serializes the object.
    ''' </summary>
    ''' <param name="info">Serialiation info object.</param>
    ''' <param name="context">Serialization context object.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> <SecurityPermission(SecurityAction.LinkDemand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    <SecurityPermission(SecurityAction.Demand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mResult", _result)
      info.AddValue("mInnerStackTrace", _innerStackTrace)

    End Sub

  End Class

End Namespace
