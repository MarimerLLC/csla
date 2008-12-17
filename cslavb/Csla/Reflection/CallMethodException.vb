Imports System
Imports System.Security.Permissions

Namespace Reflection

  ''' <summary>
  ''' This exception is returned from the 
  ''' CallMethod method in the server-side DataPortal
  ''' and contains the exception thrown by the
  ''' underlying business object method that was
  ''' being invoked.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", _
    "CA1032:ImplementStandardExceptionConstructors")> _
  <Serializable()> _
  Public Class CallMethodException
    Inherits Exception

    Private _innerStackTrace As String

    ''' <summary>
    ''' Get the stack trace from the original
    ''' exception.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", _
      "CA1305:SpecifyIFormatProvider", _
      MessageId:="System.String.Format(System.String,System.Object,System.Object,System.Object)")> _
    Public Overrides ReadOnly Property StackTrace() As String
      Get
        Return String.Format("{0}{1}{2}", _
          _innerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="message">Message text describing the exception.</param>
    ''' <param name="ex">Inner exception object.</param>
    Public Sub New(ByVal message As String, ByVal ex As Exception)

      MyBase.New(message, ex)
      If ex IsNot Nothing Then
        _innerStackTrace = ex.StackTrace
      End If

    End Sub

    ''' <summary>
    ''' Creates an instance of the object for deserialization.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="context">Serialiation context.</param>
    Protected Sub New( _
      ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      _innerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    ''' <summary>
    ''' Serializes the object.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="context">Serialization context.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mInnerStackTrace", _innerStackTrace)

    End Sub

  End Class

End Namespace
