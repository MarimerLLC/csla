Namespace Server

  ''' <summary>
  ''' Returns data from the server-side DataPortal to the 
  ''' client-side DataPortal. Intended for internal CSLA .NET
  ''' use only.
  ''' </summary>
  <Serializable()> _
  Public Class DataPortalResult

    Private mReturnObject As Object
    Private mGlobalContext As Object

    Public ReadOnly Property ReturnObject() As Object
      Get
        Return mReturnObject
      End Get
    End Property

    Public ReadOnly Property GlobalContext() As Object
      Get
        Return mGlobalContext
      End Get
    End Property

    Public Sub New()

      mGlobalContext = ApplicationContext.GetGlobalContext

    End Sub

    Public Sub New(ByVal returnObject As Object)

      mReturnObject = returnObject
      mGlobalContext = ApplicationContext.GetGlobalContext

    End Sub
  End Class

End Namespace
