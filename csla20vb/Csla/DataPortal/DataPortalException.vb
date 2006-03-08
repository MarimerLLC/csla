Imports System.Security.Permissions

''' <summary>
''' This exception is returned for any errors occuring
''' during the server-side DataPortal invocation.
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")> _
<Serializable()> _
Public Class DataPortalException
  Inherits Exception

  Private mBusinessObject As Object
  Private mInnerStackTrace As String

  ''' <summary>
  ''' Returns a reference to the business object
  ''' from the server-side DataPortal.
  ''' </summary>
  ''' <remarks>
  ''' Remember that this object may be in an invalid
  ''' or undefined state. This is the business object
  ''' (and any child objects) as it existed when the
  ''' exception occured on the server. Thus the object
  ''' state may have been altered by the server and
  ''' may no longer reflect data in the database.
  ''' </remarks>
  Public ReadOnly Property BusinessObject() As Object
    Get
      Return mBusinessObject
    End Get
  End Property

  ''' <summary>
  ''' Gets the original server-side exception.
  ''' </summary>
  ''' <returns>An exception object.</returns>
  ''' <remarks>
  ''' When an exception occurs in business code behind
  ''' the data portal, it is wrapped in a 
  ''' <see cref="Csla.Server.DataPortalException"/>, which 
  ''' is then wrapped in a 
  ''' <see cref="Csla.DataPortalException"/>. This property
  ''' unwraps and returns the original exception 
  ''' thrown by the business code on the server.
  ''' </remarks>
  Public ReadOnly Property BusinessException() As Exception
    Get
      Return Me.InnerException.InnerException
    End Get
  End Property

  ''' <summary>
  ''' Get the combined stack trace from the server
  ''' and client.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.String.Format(System.String,System.Object,System.Object,System.Object)")> _
  Public Overrides ReadOnly Property StackTrace() As String
    Get
      Return String.Format("{0}{1}{2}", mInnerStackTrace, vbCrLf, MyBase.StackTrace)
    End Get
  End Property

  Public Sub New(ByVal message As String, ByVal businessObject As Object)

    MyBase.New(message)
    mInnerStackTrace = ""
    mBusinessObject = businessObject

  End Sub

  Public Sub New(ByVal message As String, ByVal ex As Exception, ByVal businessObject As Object)

    MyBase.New(message, ex)
    mInnerStackTrace = ex.StackTrace
    mBusinessObject = businessObject

  End Sub

  Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.New(info, context)
    mBusinessObject = info.GetValue("mBusinessObject", GetType(Object))
    mInnerStackTrace = info.GetString("mInnerStackTrace")

  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <SecurityPermission(SecurityAction.LinkDemand, _
    Flags:=SecurityPermissionFlag.SerializationFormatter)> _
  <SecurityPermission(SecurityAction.Demand, _
    Flags:=SecurityPermissionFlag.SerializationFormatter)> _
  Public Overrides Sub GetObjectData( _
    ByVal info As System.Runtime.Serialization.SerializationInfo, _
    ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.GetObjectData(info, context)
    info.AddValue("mBusinessObject", mBusinessObject)
    info.AddValue("mInnerStackTrace", mInnerStackTrace)

  End Sub

End Class
