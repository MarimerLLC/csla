''' <summary>
''' 
''' </summary>
Namespace Server

  ''' <summary>
  ''' Returns data from the server-side DataPortal to the 
  ''' client-side DataPortal. Intended for internal CSLA .NET
  ''' use only.
  ''' </summary>
  <Serializable()> _
  Public Class DataPortalResult

    Public ReturnObject As Object
    Public GlobalContext As Object

    Public Sub New()

      GlobalContext = ApplicationContext.GlobalContext

    End Sub

    Public Sub New(ByVal returnObject As Object)

      Me.ReturnObject = returnObject
      GlobalContext = ApplicationContext.GlobalContext

    End Sub
  End Class

End Namespace
