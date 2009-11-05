Imports System.Threading
Imports Csla
Imports Csla.DataPortalClient

Public Class Proxy

  Implements IDataPortalProxy

  Private mAppDomain As AppDomain
  Private mPortal As Csla.Server.IDataPortalServer

  Private ReadOnly Property Portal() As Server.IDataPortalServer
    Get
      If mAppDomain Is Nothing Then
        Dim current As AppDomain = AppDomain.CurrentDomain
        Dim setup As AppDomainSetup = current.SetupInformation
        setup.ApplicationName = "Csla.DataPortal"
        mAppDomain = _
          AppDomain.CreateDomain("ServerDomain", current.Evidence, setup)
      End If
      If mPortal Is Nothing Then
        mPortal = CType(mAppDomain.CreateInstanceAndUnwrap( _
          "Csla", "Csla.Server.Hosts.RemotingPortal"), Server.IDataPortalServer)
      End If
      Return mPortal
    End Get
  End Property

#Region " Create "

  Private Class CreateTask
    Public ObjectType As Type
    Public Criteria As Object
    Public Context As Server.DataPortalContext
    Public Result As Server.DataPortalResult
  End Class

  Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

    Dim t As New Thread(AddressOf DoCreate)
    Dim task As New CreateTask
    With task
      .ObjectType = objectType
      .Criteria = criteria
      .Context = context
    End With
    t.Start(task)
    t.Join()
    Return task.Result

  End Function

  Private Sub DoCreate(ByVal state As Object)

    Dim task As CreateTask = CType(state, CreateTask)
    task.Result = Portal.Create(task.ObjectType, task.Criteria, task.Context)

  End Sub

#End Region

#Region " Fetch "

  Private Class FetchTask
    Public ObjectType As Type
    Public Criteria As Object
    Public Context As Server.DataPortalContext
    Public Result As Server.DataPortalResult
  End Class

  Public Function Fetch( _
    ByVal objectType As System.Type, _
    ByVal criteria As Object, _
    ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
    Implements Server.IDataPortalServer.Fetch

    Dim t As New Thread(AddressOf DoFetch)
    Dim task As New FetchTask
    With task
      .ObjectType = objectType
      .Criteria = criteria
      .Context = context
    End With
    t.Start(task)
    t.Join()
    Return task.Result

  End Function

  Private Sub DoFetch(ByVal state As Object)

    Dim task As FetchTask = CType(state, FetchTask)
    task.Result = Portal.Fetch(task.ObjectType, task.Criteria, task.Context)

  End Sub

#End Region

#Region " Update "

  Private Class UpdateTask
    Public Obj As Object
    Public Context As Server.DataPortalContext
    Public Result As Server.DataPortalResult
  End Class

  Public Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

    Dim t As New Thread(AddressOf DoUpdate)
    Dim task As New UpdateTask
    With task
      .Obj = obj
      .Context = context
    End With
    t.Start(task)
    t.Join()
    Return task.Result

  End Function

  Private Sub DoUpdate(ByVal state As Object)

    Dim task As UpdateTask = CType(state, UpdateTask)
    task.Result = Portal.Update(task.Obj, task.Context)

  End Sub

#End Region

#Region " Delete "

  Private Class DeleteTask
    Public Criteria As Object
    Public Context As Server.DataPortalContext
    Public Result As Server.DataPortalResult
  End Class

  Public Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

    Dim t As New Thread(AddressOf DoDelete)
    Dim task As New DeleteTask
    With task
      .Criteria = criteria
      .Context = context
    End With
    t.Start(task)
    t.Join()
    Return task.Result

  End Function

  Private Sub DoDelete(ByVal state As Object)

    Dim task As DeleteTask = CType(state, DeleteTask)
    task.Result = Portal.Delete(task.Criteria, task.Context)

  End Sub

#End Region

  Public ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
    Get
      Return True
    End Get
  End Property

End Class
