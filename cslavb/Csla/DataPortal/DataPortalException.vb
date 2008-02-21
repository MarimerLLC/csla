Imports System.Security.Permissions

''' <summary>
''' This exception is returned for any errors occuring
''' during the server-side DataPortal invocation.
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")> _
<Serializable()> _
Public Class DataPortalException
  Inherits Exception

  Private _businessObject As Object
  Private _innerStackTrace As String

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
      Return _businessObject
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
      Return String.Format("{0}{1}{2}", _innerStackTrace, vbCrLf, MyBase.StackTrace)
    End Get
  End Property

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="message">Text describing the exception.</param>
  ''' <param name="businessObject">The business object
  ''' as it was at the time of the exception.</param>
  Public Sub New(ByVal message As String, ByVal businessObject As Object)

    MyBase.New(message)
    _innerStackTrace = ""
    _businessObject = businessObject

  End Sub

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="message">Text describing the exception.</param>
  ''' <param name="ex">Inner exception.</param>
  ''' <param name="businessObject">The business object
  ''' as it was at the time of the exception.</param>
  Public Sub New(ByVal message As String, ByVal ex As Exception, ByVal businessObject As Object)

    MyBase.New(message, ex)
    _innerStackTrace = ex.StackTrace
    _businessObject = businessObject

  End Sub

  ''' <summary>
  ''' Creates an instance of the object for serialization.
  ''' </summary>
  ''' <param name="info">Serialiation info object.</param>
  ''' <param name="context">Serialization context object.</param>
  Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.New(info, context)
    _businessObject = info.GetValue("mBusinessObject", GetType(Object))
    _innerStackTrace = info.GetString("mInnerStackTrace")

  End Sub

  ''' <summary>
  ''' Serializes the object.
  ''' </summary>
  ''' <param name="info">Serialiation info object.</param>
  ''' <param name="context">Serialization context object.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <SecurityPermission(SecurityAction.LinkDemand, _
    Flags:=SecurityPermissionFlag.SerializationFormatter)> _
  <SecurityPermission(SecurityAction.Demand, _
    Flags:=SecurityPermissionFlag.SerializationFormatter)> _
  Public Overrides Sub GetObjectData( _
    ByVal info As System.Runtime.Serialization.SerializationInfo, _
    ByVal context As System.Runtime.Serialization.StreamingContext)

    MyBase.GetObjectData(info, context)
    info.AddValue("mBusinessObject", _businessObject)
    info.AddValue("mInnerStackTrace", _innerStackTrace)

  End Sub

End Class
