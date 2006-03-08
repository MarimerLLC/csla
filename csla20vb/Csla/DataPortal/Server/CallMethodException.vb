Imports System.Security.Permissions

Namespace Server

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

    Private mInnerStackTrace As String

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
          mInnerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    Public Sub New(ByVal message As String, ByVal ex As Exception)

      MyBase.New(message, ex)
      mInnerStackTrace = ex.StackTrace

    End Sub

    Protected Sub New( _
      ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      mInnerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", _
      "CA1062:ValidateArgumentsOfPublicMethods")> _
    <SecurityPermission(SecurityAction.LinkDemand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    <SecurityPermission(SecurityAction.Demand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    Public Overrides Sub GetObjectData( _
      ByVal info As System.Runtime.Serialization.SerializationInfo, _
      ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mInnerStackTrace", mInnerStackTrace)

    End Sub

  End Class

End Namespace
