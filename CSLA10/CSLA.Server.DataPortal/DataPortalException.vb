''' <summary>
'''
''' </summary>
Namespace Server

  ''' <summary>
  ''' This exception is returned from the 
  ''' server-side DataPortal and contains the exception
  ''' and context data from the server.
  ''' </summary>
  <Serializable()> _
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

    Public Overrides ReadOnly Property StackTrace() As String
      Get
        Return String.Format("{0}{1}{2}", mInnerStackTrace, vbCrLf, MyBase.StackTrace)
      End Get
    End Property

    Public Sub New(ByVal message As String, ByVal ex As Exception)

      MyBase.New(message, ex)
      mInnerStackTrace = ex.StackTrace
      mResult = New DataPortalResult

    End Sub

    Public Sub New(ByVal message As String, ByVal result As DataPortalResult)

      MyBase.New(message)
      mInnerStackTrace = ""
      mResult = result

    End Sub

    Public Sub New(ByVal message As String, ByVal ex As Exception, ByVal result As DataPortalResult)

      MyBase.New(message, ex)
      mInnerStackTrace = ex.StackTrace
      mResult = result

    End Sub

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.New(info, context)
      mResult = DirectCast(info.GetValue("mResult", GetType(DataPortalResult)), DataPortalResult)
      mInnerStackTrace = info.GetString("mInnerStackTrace")

    End Sub

    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      MyBase.GetObjectData(info, context)
      info.AddValue("mResult", mResult)
      info.AddValue("mInnerStackTrace", mInnerStackTrace)

    End Sub

  End Class

End Namespace
