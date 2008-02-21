Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Returns data from the server-side DataPortal to the 
  ''' client-side DataPortal. Intended for internal CSLA .NET
  ''' use only.
  ''' </summary>
  <Serializable()> _
  Public Class DataPortalResult

    Private _returnObject As Object
    Private _globalContext As HybridDictionary

    ''' <summary>
    ''' The business object being returned from
    ''' the server.
    ''' </summary>
    Public ReadOnly Property ReturnObject() As Object
      Get
        Return _returnObject
      End Get
    End Property

    ''' <summary>
    ''' The global context being returned from
    ''' the server.
    ''' </summary>
    Public ReadOnly Property GlobalContext() As HybridDictionary
      Get
        Return _globalContext
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()

      _globalContext = ApplicationContext.GetGlobalContext

    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="returnObject">Object to return as part
    ''' of the result.</param>
    Public Sub New(ByVal returnObject As Object)

      _returnObject = returnObject
      _globalContext = ApplicationContext.GetGlobalContext

    End Sub
  End Class

End Namespace
