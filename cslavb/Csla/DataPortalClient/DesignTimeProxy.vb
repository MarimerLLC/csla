Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Reflection

Namespace DataPortalClient

  ''' <summary>
  ''' Data portal proxy used when an object is to be created with
  ''' design time data for Visual Studio or Expression Blend.
  ''' </summary>
  Public Class DesignTimeProxy
    Implements IDataPortalProxy

#Region "IDataPortalProxy Members"

    ''' <summary>
    ''' Gets a value indicating whether the data portal
    ''' will run remotely. Always returns false.
    ''' </summary>
    Public ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
      Get
        Return False
      End Get
    End Property

#End Region

#Region "IDataPortalServer Members"

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to create a
    ''' new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create
      Return CreateDesignTimeObject(objectType, criteria, context)
    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to load an
    ''' existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to retrieve.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch
      Return CreateDesignTimeObject(objectType, criteria, context)
    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to update a
    ''' business object.
    ''' </summary>
    ''' <param name="obj">The business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update
      Return New Csla.Server.DataPortalResult(obj)
    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to delete a
    ''' business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Delete(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete
      Return CreateDesignTimeObject(objectType, criteria, context)
    End Function

    Private Function CreateDesignTimeObject(ByVal objectType As Type, ByVal criteria As Object, ByVal context As Csla.Server.DataPortalContext) As Csla.Server.DataPortalResult
      Dim obj = Activator.CreateInstance(objectType, True)
      Dim returnValue As Object = Nothing
      returnValue = MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create")
      Return New Csla.Server.DataPortalResult(returnValue)
    End Function

#End Region

  End Class
End Namespace

