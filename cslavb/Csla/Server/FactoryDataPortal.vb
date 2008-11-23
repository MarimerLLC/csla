Imports System
Imports System.Configuration
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Properties

Namespace Server

  ''' <summary>
  ''' Server-side data portal implementation that
  ''' invokes an object factory rather than directly
  ''' interacting with the business object.
  ''' </summary>
  Public Class FactoryDataPortal
    Implements IDataPortalServer

#Region "Factory Loader"

    Private Shared _factoryLoader As IObjectFactoryLoader

    ''' <summary>
    ''' Gets or sets a delegate reference to the method
    ''' called to create instances of factory objects
    ''' as requested by the ObjectFactory attribute on
    ''' a CSLA .NET business object.
    ''' </summary>
    Public Shared Property FactoryLoader() As IObjectFactoryLoader
      Get

        If _factoryLoader Is Nothing Then
          Dim setting As String = ConfigurationManager.AppSettings("CslaObjectFactoryLoader")

          If Not String.IsNullOrEmpty(setting) Then
            _factoryLoader = DirectCast(Activator.CreateInstance(Type.GetType(setting, True, True)), IObjectFactoryLoader)
          Else
            _factoryLoader = New ObjectFactoryLoader()
          End If
        End If

        Return _factoryLoader

      End Get
      Set(ByVal value As IObjectFactoryLoader)
        _factoryLoader = value
      End Set
    End Property

#End Region

#Region "Method invokes"

    Private Function InvokeMethod(ByVal factoryTypeName As String, ByVal methodName As String, ByVal context As DataPortalContext) As DataPortalResult
      Dim factory As Object = FactoryLoader.GetFactory(factoryTypeName)
      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", context)
      Dim result As Object

      Try
        result = Csla.Reflection.MethodCaller.CallMethod(factory, methodName)
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", context)
      Catch ex As Exception
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", ex)
        Throw
      End Try

      Return New DataPortalResult(result)

    End Function

    Private Function InvokeMethod(ByVal factoryTypeName As String, ByVal methodName As String, ByVal e As Object, ByVal context As DataPortalContext) As DataPortalResult
      Dim factory As Object = FactoryLoader.GetFactory(factoryTypeName)
      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", context)
      Dim result As Object

      Try
        result = Csla.Reflection.MethodCaller.CallMethod(factory, methodName, e)
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", context)
      Catch ex As Exception
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", ex)
        Throw
      End Try

      Return New DataPortalResult(result)

    End Function

#End Region

#Region "IDataPortalServer Members"

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create
      Try
        Dim result As DataPortalResult = Nothing

        If TypeOf criteria Is Integer Then
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.CreateMethodName, context)
        Else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.CreateMethodName, criteria, context)
        End If
        Return result
      Catch ex As Exception
        Throw New DataPortalException(context.FactoryInfo.CreateMethodName + " " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to retrieve.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch
      Try
        Dim result As DataPortalResult = Nothing

        If TypeOf criteria Is Integer Then
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.FetchMethodName, context)
        Else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.FetchMethodName, criteria, context)
        End If
        Return result
      Catch ex As Exception
        Throw New DataPortalException(context.FactoryInfo.FetchMethodName + " " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update
      Try
        Dim result As DataPortalResult = Nothing
        result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.UpdateMethodName, obj, context)
        Return result
      Catch ex As Exception
        Throw New DataPortalException(context.FactoryInfo.UpdateMethodName + " " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Delete(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete
      Try
        Dim result As DataPortalResult = Nothing

        If TypeOf criteria Is Integer Then
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.DeleteMethodName, context)
        Else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.DeleteMethodName, criteria, context)
        End If
        Return result
      Catch ex As Exception
        Throw New DataPortalException(context.FactoryInfo.DeleteMethodName + " " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

#End Region

  End Class
End Namespace

