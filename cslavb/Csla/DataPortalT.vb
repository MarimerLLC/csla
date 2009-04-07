Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

''' <summary>
''' Client side data portal used for making asynchronous
''' data portal calls in .NET.
''' </summary>
''' <typeparam name="T">Type of business object.</typeparam>
Public Class DataPortal(Of T)
  Friend Const EmptyCriteria As Integer = 1

#Region " Data Portal Async Request "

  Private Class DataPortalAsyncRequest
    Private _argument As Object
    Private _principal As System.Security.Principal.IPrincipal
    Private _clientContext As Core.ContextDictionary
    Private _globalContext As Core.ContextDictionary
    Private _userState As Object

    Public Property Argument() As Object
      Get
        Return _argument
      End Get
      Set(ByVal value As Object)
        _argument = value
      End Set
    End Property

    Public Property Principal() As System.Security.Principal.IPrincipal
      Get
        Return _principal
      End Get
      Set(ByVal value As System.Security.Principal.IPrincipal)
        _principal = value
      End Set
    End Property

    Public Property ClientContext() As Core.ContextDictionary
      Get
        Return _clientContext
      End Get
      Set(ByVal value As Core.ContextDictionary)
        _clientContext = value
      End Set
    End Property

    Public Property GlobalContext() As Core.ContextDictionary
      Get
        Return _globalContext
      End Get
      Set(ByVal value As Core.ContextDictionary)
        _globalContext = value
      End Set
    End Property

    Public Property UserState() As Object
      Get
        Return _userState
      End Get
      Set(ByVal value As Object)
        _userState = value
      End Set
    End Property

    Public Sub New(ByVal argument As Object, ByVal userState As Object)
      Me.Argument = argument
      Me.Principal = ApplicationContext.User
      Me.ClientContext = ApplicationContext.ClientContext
      Me.GlobalContext = ApplicationContext.GlobalContext
      Me.UserState = userState
    End Sub
  End Class


  Private Class DataPortalAsyncResult
    Private _result As T
    Private _globalContext As Core.ContextDictionary
    Private _userState As Object

    Public Property Result() As T
      Get
        Return _result
      End Get
      Set(ByVal value As T)
        _result = value
      End Set
    End Property

    Public Property GlobalContext() As Core.ContextDictionary
      Get
        Return _globalContext
      End Get
      Set(ByVal value As Core.ContextDictionary)
        _globalContext = value
      End Set
    End Property

    Public Property UserState() As Object
      Get
        Return _userState
      End Get
      Set(ByVal value As Object)
        _userState = value
      End Set
    End Property

    Public Sub New(ByVal result As T, ByVal globalContext As Core.ContextDictionary, ByVal userState As Object)
      Me.Result = result
      Me.GlobalContext = globalContext
      Me.UserState = userState
    End Sub
  End Class
#End Region

#Region " Set Background Thread Context "

  Private Sub SetThreadContext(ByVal request As DataPortalAsyncRequest)
    ApplicationContext.User = request.Principal
    ApplicationContext.SetContext(request.ClientContext, request.GlobalContext)
  End Sub

#End Region

#Region " GlobalContext "

  Private _globalContext As Core.ContextDictionary


  ''' <summary>
  ''' Gets a reference to the global context returned from
  ''' the background thread and/or server.
  ''' </summary>
  Public ReadOnly Property GlobalContext() As Core.ContextDictionary
    Get
      Return _globalContext
    End Get
  End Property

#End Region

#Region " Create "

  ''' <summary>
  ''' Event raised when the operation has completed.
  ''' </summary>
  ''' <remarks><para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para></remarks>
  Public Event CreateCompleted As EventHandler(Of DataPortalResult(Of T))

  ''' <summary>
  ''' Raises the event.
  ''' </summary>
  ''' <param name="e">The parameter provided to the event handler.</param>
  ''' <remarks><para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para></remarks>
  Protected Overridable Sub OnCreateCompleted(ByVal e As DataPortalResult(Of T))
    RaiseEvent CreateCompleted(Me, e)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to create a new object, which is loaded
  ''' with default values from the database.
  ''' </summary>
  Public Sub BeginCreate()
    BeginCreate(EmptyCriteria)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to create a new object, which is loaded
  ''' with default values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  Public Sub BeginCreate(ByVal criteria As Object)
    BeginCreate(criteria, Nothing)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to create a new object, which is loaded 
  ''' with default values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginCreate(ByVal criteria As Object, ByVal userState As Object)
    Dim bw As New System.ComponentModel.BackgroundWorker()
    AddHandler bw.RunWorkerCompleted, AddressOf Create_RunWorkerCompleted
    AddHandler bw.DoWork, AddressOf Create_DoWork
    bw.RunWorkerAsync(New DataPortalAsyncRequest(criteria, userState))
  End Sub

  Private Sub Create_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
    If e.Error Is Nothing Then

      Dim result = TryCast(e.Result, DataPortalAsyncResult)
      If result IsNot Nothing Then
        _globalContext = result.GlobalContext
        If result.Result IsNot Nothing Then
          OnCreateCompleted(New DataPortalResult(Of T)(DirectCast(result.Result, T), Nothing, result.UserState))
        Else
          OnCreateCompleted(New DataPortalResult(Of T)(Nothing, Nothing, result.UserState))
        End If
        Return
      End If
      Return
    End If
    
    OnCreateCompleted(New DataPortalResult(Of T)(Nothing, e.Error, Nothing))
  End Sub

  Private Sub Create_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    Dim request = TryCast(e.Argument, DataPortalAsyncRequest)
    SetThreadContext(request)

    Dim state As Object = request.Argument
    Dim result As T = Nothing
    If TypeOf state Is Integer Then
      result = DirectCast(DataPortal.Create(Of T)(), T)
    Else
      result = DirectCast(DataPortal.Create(Of T)(state), T)
    End If
    e.Result = New DataPortalAsyncResult(result, ApplicationContext.GlobalContext, request.UserState)
  End Sub
#End Region

#Region " Fetch "

  ''' <summary>
  ''' Event raised when the operation has completed.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Public Event FetchCompleted As EventHandler(Of DataPortalResult(Of T))

  ''' <summary>
  ''' Raises the event.
  ''' </summary>
  ''' <param name="e">
  ''' The parameter provided to the event handler.
  ''' </param>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Protected Overridable Sub OnFetchCompleted(ByVal e As DataPortalResult(Of T))
    RaiseEvent FetchCompleted(Me, e)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to retrieve an existing object, which is loaded 
  ''' with values from the database.
  ''' </summary>
  Public Sub BeginFetch()
    BeginFetch(EmptyCriteria)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to retrieve an existing object, which is loaded 
  ''' with values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  Public Sub BeginFetch(ByVal criteria As Object)
    BeginFetch(criteria, Nothing)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to retrieve an existing object, which is loaded 
  ''' with values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginFetch(ByVal criteria As Object, ByVal userState As Object)
    Dim bw As New System.ComponentModel.BackgroundWorker()
    AddHandler bw.RunWorkerCompleted, AddressOf Fetch_RunWorkerCompleted
    AddHandler bw.DoWork, AddressOf Fetch_DoWork
    bw.RunWorkerAsync(New DataPortalAsyncRequest(criteria, userState))
  End Sub

  Private Sub Fetch_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

    If e.Error Is Nothing Then
      Dim result = TryCast(e.Result, DataPortalAsyncResult)
      If result IsNot Nothing Then
        _globalContext = result.GlobalContext
        If result.Result IsNot Nothing Then
          OnFetchCompleted(New DataPortalResult(Of T)(DirectCast(result.Result, T), Nothing, result.UserState))
        Else
          OnFetchCompleted(New DataPortalResult(Of T)(Nothing, Nothing, result.UserState))
        End If
      End If
      Return
    End If
    
    OnFetchCompleted(New DataPortalResult(Of T)(Nothing, e.Error, Nothing))
  End Sub

  Private Sub Fetch_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    Dim request = TryCast(e.Argument, DataPortalAsyncRequest)
    SetThreadContext(request)

    Dim state As Object = request.Argument
    Dim result As T = Nothing
    If TypeOf state Is Integer Then
      result = DirectCast(Dataportal.Fetch(Of T)(), T)
    Else
      result = DirectCast(DataPortal.Fetch(Of T)(state), T)
    End If
    e.Result = New DataPortalAsyncResult(result, ApplicationContext.GlobalContext, request.UserState)
  End Sub

#End Region

#Region " Update "

  ''' <summary>
  ''' Event raised when the operation has completed.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Public Event UpdateCompleted As EventHandler(Of DataPortalResult(Of T))

  ''' <summary>
  ''' Raises the event.
  ''' </summary>
  ''' <param name="e">
  ''' The parameter provided to the event handler.
  ''' </param>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Protected Overridable Sub OnUpdateCompleted(ByVal e As DataPortalResult(Of T))
    RaiseEvent UpdateCompleted(Me, e)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to update an object.
  ''' </summary>
  ''' <param name="obj">Object to update.</param>
  Public Sub BeginUpdate(ByVal obj As Object)
    BeginUpdate(obj, Nothing)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to update an object.
  ''' </summary>
  ''' <param name="obj">Object to update.</param>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginUpdate(ByVal obj As Object, ByVal userState As Object)
    Dim bw As New System.ComponentModel.BackgroundWorker()
    AddHandler bw.RunWorkerCompleted, AddressOf Update_RunWorkerCompleted
    AddHandler bw.DoWork, AddressOf Update_DoWork
    bw.RunWorkerAsync(New DataPortalAsyncRequest(obj, userState))
  End Sub

  Private Sub Update_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

    If e.Error Is Nothing Then
      Dim result = TryCast(e.Result, DataPortalAsyncResult)
      If result IsNot Nothing Then
        _globalContext = result.GlobalContext
        If result.Result IsNot Nothing Then
          OnUpdateCompleted(New DataPortalResult(Of T)(DirectCast(result.Result, T), Nothing, result.UserState))
        Else
          OnUpdateCompleted(New DataPortalResult(Of T)(Nothing, Nothing, result.UserState))
        End If
      End If
      Return
    End If
    
    OnUpdateCompleted(New DataPortalResult(Of T)(Nothing, e.Error, Nothing))
  End Sub

  Private Sub Update_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    Dim request = TryCast(e.Argument, DataPortalAsyncRequest)
    SetThreadContext(request)

    Dim state As Object = request.Argument
    Dim result As T = Nothing
    result = DirectCast(DataPortal.Update(Of T)(DirectCast(state, T)), T)
    e.Result = New DataPortalAsyncResult(result, ApplicationContext.GlobalContext, request.UserState)
  End Sub

#End Region

#Region " Delete "

  ''' <summary>
  ''' Event raised when the operation has completed.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Public Event DeleteCompleted As EventHandler(Of DataPortalResult(Of T))

  ''' <summary>
  ''' Raises the event.
  ''' </summary>
  ''' <param name="e">
  ''' The parameter provided to the event handler.
  ''' </param>
  ''' <remarks>
  ''' <para>
  ''' If your application is running in WPF, this event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' If your application is running in Windows Forms,
  ''' this event will be raised on a background thread.
  ''' If you also set DataPortal.SynchronizationObject
  ''' to a Windows Forms form or control, then the event
  ''' will be raised on the UI thread automatically.
  ''' </para><para>
  ''' In any other environment (such as ASP.NET), this
  ''' event will be raised on a background thread.
  ''' </para>
  ''' </remarks>
  Protected Overridable Sub OnDeleteCompleted(ByVal e As DataPortalResult(Of T))
    RaiseEvent DeleteCompleted(Me, e)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to delete an object.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  Public Sub BeginDelete(ByVal criteria As Object)
    BeginDelete(criteria, Nothing)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to delete an object.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginDelete(ByVal criteria As Object, ByVal userState As Object)
    Dim bw As New System.ComponentModel.BackgroundWorker()
    AddHandler bw.RunWorkerCompleted, AddressOf Delete_RunWorkerCompleted
    AddHandler bw.DoWork, AddressOf Delete_DoWork
    bw.RunWorkerAsync(New DataPortalAsyncRequest(criteria, userState))
  End Sub

  Private Sub Delete_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

    If e.Error Is Nothing Then
      Dim result = TryCast(e.Result, DataPortalAsyncResult)
      If result IsNot Nothing Then
        _globalContext = result.GlobalContext
        OnDeleteCompleted(New DataPortalResult(Of T)(Nothing, Nothing, result.UserState))
      End If
      Return
    End If
    
    OnDeleteCompleted(New DataPortalResult(Of T)(Nothing, e.Error, Nothing))
  End Sub

  Private Sub Delete_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    Dim request = TryCast(e.Argument, DataPortalAsyncRequest)
    SetThreadContext(request)

    Dim state As Object = request.Argument
    DataPortal.Delete(Of T)(state)
    e.Result = New DataPortalAsyncResult(Nothing, ApplicationContext.GlobalContext, request.UserState)
  End Sub

#End Region

#Region " Execute "

  ''' <summary>
  ''' Event indicating an execute operation is complete.
  ''' </summary>
  Public Event ExecuteCompleted As EventHandler(Of DataPortalResult(Of T))

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to execute a command object.
  ''' </summary>
  ''' <param name="command">Command object to execute.</param>
  Public Sub BeginExecute(ByVal command As T)
    BeginExecute(command, Nothing)
  End Sub

  ''' <summary>
  ''' Called by a factory method in a business class or
  ''' by the UI to execute a command object.
  ''' </summary>
  ''' <param name="command">Command object to execute.</param>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginExecute(ByVal command As T, ByVal userState As Object)
    Dim bw As New System.ComponentModel.BackgroundWorker()
    AddHandler bw.RunWorkerCompleted, AddressOf Execute_RunWorkerCompleted
    AddHandler bw.DoWork, AddressOf Execute_DoWork
    bw.RunWorkerAsync(New DataPortalAsyncRequest(command, userState))
  End Sub

  Private Sub Execute_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)

    If e.Error Is Nothing Then
      Dim result = TryCast(e.Result, DataPortalAsyncResult)
      If result IsNot Nothing Then
        _globalContext = result.GlobalContext
        If result.Result IsNot Nothing Then
          OnExecuteCompleted(New DataPortalResult(Of T)(DirectCast(result.Result, T), e.Error, result.UserState))
        Else
          OnExecuteCompleted(New DataPortalResult(Of T)(Nothing, e.Error, result.UserState))
        End If
      End If
      Return
    End If
    
    OnExecuteCompleted(New DataPortalResult(Of T)(Nothing, e.Error, Nothing))
  End Sub

  Private Sub Execute_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    Dim request = TryCast(e.Argument, DataPortalAsyncRequest)
    SetThreadContext(request)

    Dim state As Object = request.Argument
    Dim result As T = Nothing
    result = DataPortal.Execute(Of T)(DirectCast(state, T))
    e.Result = New DataPortalAsyncResult(result, ApplicationContext.GlobalContext, request.UserState)
  End Sub

  ''' <summary>
  ''' Raises the ExecuteCompleted event.
  ''' </summary>
  ''' <param name="e">Event arguments.</param>
  Protected Overridable Sub OnExecuteCompleted(ByVal e As DataPortalResult(Of T))
    RaiseEvent ExecuteCompleted(Me, e)
  End Sub

#End Region

End Class

