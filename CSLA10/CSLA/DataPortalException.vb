''' <summary>
''' This exception is returned for any errors occuring
''' during the server-side DataPortal invocation.
''' </summary>
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

  End Sub

End Class
