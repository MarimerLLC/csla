Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Returns data from the server-side DataPortal to the 
  ''' client-side DataPortal. Intended for internal CSLA .NET
  ''' use only.
  ''' </summary>
  <Serializable()> _
  Public Class DataPortalResult

    Private mReturnObject As Object
    Private mGlobalContext As HybridDictionary

    ''' <summary>
    ''' The business object being returned from
    ''' the server.
    ''' </summary>
    Public ReadOnly Property ReturnObject() As Object
      Get
        Return mReturnObject
      End Get
    End Property

    ''' <summary>
    ''' The global context being returned from
    ''' the server.
    ''' </summary>
    Public ReadOnly Property GlobalContext() As HybridDictionary
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
