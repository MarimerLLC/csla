Imports System.ServiceModel
Imports Csla.Server
Imports Csla.Server.Hosts
Imports Csla.Server.Hosts.WcfChannel

Namespace DataPortalClient

  ''' <summary>
  ''' Implements a data portal proxy to relay data portal
  ''' calls to a remote application server by using WCF.
  ''' </summary>
  Public Class WcfProxy
    Implements DataPortalClient.IDataPortalProxy

#Region " IDataPortalProxy Members "

    ''' <summary>
    ''' Gets a value indicating whether the data portal
    ''' is hosted on a remote server.
    ''' </summary>
    Public ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

#End Region

    Private _endPoint As String = "WcfDataPortal"

    ''' <summary>
    ''' Gets or sets the WCF endpoint used
    ''' to contact the server.
    ''' </summary>
    ''' <remarks>
    ''' The default value is WcfDataPortal.
    ''' </remarks>
    Protected Property EndPoint() As String
      Get
        Return _endPoint
      End Get
      Set(ByVal value As String)
        _endPoint = value
      End Set
    End Property

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
    Public Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalProxy.Create
      Dim cf As ChannelFactory(Of IWcfPortal) = New ChannelFactory(Of IWcfPortal)(_endPoint)
      Dim svr As IWcfPortal = cf.CreateChannel()
      Dim response As WcfResponse = svr.Create(New CreateRequest(objectType, criteria, context))
      cf.Close()

      Dim result As Object = response.Result
      If TypeOf result Is Exception Then
        Throw CType(result, Exception)
      End If
      Return CType(result, DataPortalResult)
    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to load an
    ''' existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalProxy.Fetch
      Dim cf As ChannelFactory(Of IWcfPortal) = New ChannelFactory(Of IWcfPortal)(_endPoint)
      Dim svr As IWcfPortal = cf.CreateChannel()
      Dim response As WcfResponse = svr.Fetch(New FetchRequest(objectType, criteria, context))
      cf.Close()

      Dim result As Object = response.Result
      If TypeOf result Is Exception Then
        Throw CType(result, Exception)
      End If
      Return CType(result, DataPortalResult)
    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to update a
    ''' business object.
    ''' </summary>
    ''' <param name="obj">The business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalProxy.Update
      Dim cf As ChannelFactory(Of IWcfPortal) = New ChannelFactory(Of IWcfPortal)(_endPoint)
      Dim svr As IWcfPortal = cf.CreateChannel()
      Dim response As WcfResponse = svr.Update(New UpdateRequest(obj, context))
      cf.Close()

      Dim result As Object = response.Result
      If TypeOf result Is Exception Then
        Throw CType(result, Exception)
      End If
      Return CType(result, DataPortalResult)
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
    Public Function Delete( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalProxy.Delete

      Dim cf As ChannelFactory(Of IWcfPortal) = New ChannelFactory(Of IWcfPortal)(_endPoint)
      Dim svr As IWcfPortal = cf.CreateChannel()
      Dim response As WcfResponse = svr.Delete(New DeleteRequest(objectType, criteria, context))
      cf.Close()

      Dim result As Object = response.Result
      If TypeOf result Is Exception Then
        Throw CType(result, Exception)
      End If
      Return CType(result, DataPortalResult)
    End Function

#End Region

  End Class

End Namespace
