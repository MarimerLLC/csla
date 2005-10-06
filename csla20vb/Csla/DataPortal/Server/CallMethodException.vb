Imports System.Security.Permissions

Namespace Server

  ''' <summary>
  ''' This exception is returned from the 
  ''' CallMethod method in the server-side DataPortal
  ''' and contains the exception thrown by the
  ''' underlying business object method that was
  ''' being invoked.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")> <Serializable()> _
  Public Class CallMethodException
    Inherits Exception

    Private mInnerStackTrace As String

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.String.Format(System.String,System.Object,System.Object,System.Object)")> _
    Public Overrides ReadOnly Property StackTrace() As String
      Get
        Return String.Format("{0}{1}{2}", mInnerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    Public Sub New(ByVal message As String, ByVal ex As Exception)

      MyBase.New(message, ex)
      mInnerStackTrace = ex.StackTrace

    End Sub

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      mInnerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> <SecurityPermission(SecurityAction.LinkDemand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    <SecurityPermission(SecurityAction.Demand, _
      Flags:=SecurityPermissionFlag.SerializationFormatter)> _
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mInnerStackTrace", mInnerStackTrace)

    End Sub

  End Class

End Namespace
