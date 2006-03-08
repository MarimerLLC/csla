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

    Private mResult As DataPortalResult
    Private mInnerStackTrace As String

    ''' <summary>
    ''' Returns the DataPortalResult object from the server.
    ''' </summary>
    Public ReadOnly Property Result() As DataPortalResult
      Get
        Return mResult
      End Get
    End Property

    ''' <summary>
    ''' Get the server-side stack trace from the
    ''' original exception.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.String.Format(System.String,System.Object,System.Object,System.Object)")> _
    Public Overrides ReadOnly Property StackTrace() As String
      Get
        Return String.Format("{0}{1}{2}", mInnerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    Public Sub New( _
      ByVal message As String, ByVal ex As Exception, _
      ByVal result As DataPortalResult)

      MyBase.New(message, ex)
      mInnerStackTrace = ex.StackTrace
      mResult = result

    End Sub

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      mResult = DirectCast(info.GetValue("mResult", GetType(DataPortalResult)), DataPortalResult)
      mInnerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> <SecurityPermission(SecurityAction.LinkDemand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    <SecurityPermission(SecurityAction.Demand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mResult", mResult)
      info.AddValue("mInnerStackTrace", mInnerStackTrace)

    End Sub

  End Class

End Namespace
