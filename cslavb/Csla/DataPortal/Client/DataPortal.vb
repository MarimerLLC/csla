Imports System.Threading
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports Csla.Reflection

''' <summary>
''' This is the client-side DataPortal as described in
''' Chapter 4.
''' </summary>
Public Module DataPortal

#Region " DataPortal events "

  ''' <summary>
  ''' Raised by DataPortal before it starts
  ''' setting up to call a server-side
  ''' DataPortal method.
  ''' </summary>
  Public Event DataPortalInitInvoke As Action(Of System.Object)
  ''' <summary>
  ''' Raised by DataPortal prior to calling the 
  ''' requested server-side DataPortal method.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")> _
  Public Event DataPortalInvoke As Action(Of DataPortalEventArgs)
  ''' <summary>
  ''' Raised by DataPortal after the requested 
  ''' server-side DataPortal method call is complete.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")> _
  Public Event DataPortalInvokeComplete As Action(Of DataPortalEventArgs)

  Private Sub OnDataPortalInitInvoke(ByVal e As Object)

    RaiseEvent DataPortalInitInvoke(e)

  End Sub

  Private Sub OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

    RaiseEvent DataPortalInvoke(e)

  End Sub

  Private Sub OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

    RaiseEvent DataPortalInvokeComplete(e)

  End Sub

#End Region

#Region " Data Access methods "

  Private Const EmptyCriteria As Integer = 1

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  Public Function Create(Of T)(ByVal criteria As Object) As T
    Return DirectCast(Create(GetType(T), criteria), T)
  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <returns>A new object, populated with default values.</returns>
  Public Function Create(Of T)() As T
    Return DirectCast(Create(GetType(T), EmptyCriteria), T)
  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  Public Function Create(ByVal criteria As Object) As Object

    Return Create(MethodCaller.GetObjectType(criteria), criteria)

  End Function

  Private Function Create( _
    ByVal objectType As Type, ByVal criteria As Object) As Object

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      Dim method As MethodInfo = MethodCaller.GetCreateMethod(objectType, criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(RunLocal(method))

      OnDataPortalInitInvoke(Nothing)

      dpContext = New Server.DataPortalContext( _
        GetPrincipal, proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, DataPortalOperations.Create))

      Try
        result = proxy.Create(objectType, criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Create {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Create))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Create, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Fetch(Of T)(ByVal criteria As Object) As T

    Return DirectCast(Fetch(GetType(T), criteria), T)

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <returns>An object populated with values from the database.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Fetch(Of T)() As T

    Return DirectCast(Fetch(GetType(T), EmptyCriteria), T)

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  Public Function Fetch(ByVal criteria As Object) As Object

    Return Fetch(MethodCaller.GetObjectType(criteria), criteria)

  End Function

  Private Function Fetch( _
    ByVal objectType As Type, ByVal criteria As Object) As Object

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      Dim method As MethodInfo = MethodCaller.GetFetchMethod(objectType, criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(RunLocal(method))

      OnDataPortalInitInvoke(Nothing)

      dpContext = New Server.DataPortalContext( _
        GetPrincipal, proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, DataPortalOperations.Fetch))

      Try
        result = proxy.Fetch(objectType, criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Fetch {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Fetch))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Fetch, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called to execute a Command object on the server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' To be a Command object, the object must inherit from
  ''' <see cref="CommandBase">CommandBase</see>.
  ''' </para><para>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </para><para>
  ''' On the server, the Command object's DataPortal_Execute() method will
  ''' be invoked. Write any server-side code in that method.
  ''' </para>
  ''' </remarks>
  ''' <typeparam name="T">Specific type of the Command object.</typeparam>
  ''' <param name="obj">A reference to the Command object to be executed.</param>
  ''' <returns>A reference to the updated Command object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", _
    MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Execute(Of T As CommandBase)(ByVal obj As T) As T

    Return DirectCast(Update(CObj(obj)), T)

  End Function

  ''' <summary>
  ''' Called to execute a Command object on the server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </para><para>
  ''' On the server, the Command object's DataPortal_Execute() method will
  ''' be invoked. Write any server-side code in that method.
  ''' </para>
  ''' </remarks>
  ''' <param name="obj">A reference to the Command object to be executed.</param>
  ''' <returns>A reference to the updated Command object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Execute(ByVal obj As CommandBase) As CommandBase

    Return DirectCast(Update(obj), CommandBase)

  End Function

  ''' <summary>
  ''' Called by the business object's Save() method to
  ''' insert, update or delete an object in the database.
  ''' </summary>
  ''' <remarks>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </remarks>
  ''' <typeparam name="T">Specific type of the business object.</typeparam>
  ''' <param name="obj">A reference to the business object to be updated.</param>
  ''' <returns>A reference to the updated business object.</returns>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Function Update(Of T)(ByVal obj As T) As T

    Return DirectCast(Update(CObj(obj)), T)

  End Function

  ''' <summary>
  ''' Called by the business object's Save() method to
  ''' insert, update or delete an object in the database.
  ''' </summary>
  ''' <remarks>
  ''' Note that this method returns a reference to the updated business object.
  ''' If the server-side DataPortal is running remotely, this will be a new and
  ''' different object from the original, and all object references MUST be updated
  ''' to use this new object.
  ''' </remarks>
  ''' <param name="obj">A reference to the business object to be updated.</param>
  ''' <returns>A reference to the updated business object.</returns>
  Public Function Update(ByVal obj As Object) As Object

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Dim operation = DataPortalOperations.Update
    Try
      Dim method As MethodInfo
      Dim methodName As String
      If TypeOf obj Is CommandBase Then
        methodName = "DataPortal_Execute"
        operation = DataPortalOperations.Execute

      ElseIf TypeOf obj Is Core.BusinessBase Then
        Dim tmp As Core.BusinessBase = DirectCast(obj, Core.BusinessBase)
        If tmp.IsDeleted Then
          methodName = "DataPortal_DeleteSelf"
        Else
          If tmp.IsNew Then
            methodName = "DataPortal_Insert"

          Else
            methodName = "DataPortal_Update"
          End If
        End If
      Else
        methodName = "DataPortal_Update"
      End If

      method = MethodCaller.GetMethod(obj.GetType, methodName)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(RunLocal(method))

      OnDataPortalInitInvoke(Nothing)

      dpContext = New Server.DataPortalContext(GetPrincipal, proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, operation))

      Try
        If Not proxy.IsServerRemote AndAlso ApplicationContext.AutoCloneOnUpdate Then
          ' when using local data portal, automatically
          ' clone original object before saving
          Dim cloneable As ICloneable = TryCast(obj, ICloneable)
          If cloneable IsNot Nothing Then
            obj = cloneable.Clone
          End If
        End If
        result = proxy.Update(obj, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Update {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, operation))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, operation, ex))
      Throw
    End Try

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a Shared (static in C#) method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="criteria">Object-specific criteria.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage( _
    "Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", _
    MessageId:="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")> _
  Public Sub Delete(ByVal criteria As Object)

    Dim result As Server.DataPortalResult
    Dim dpContext As Server.DataPortalContext = Nothing
    Try
      Dim method As MethodInfo = _
        MethodCaller.GetMethod(MethodCaller.GetObjectType(criteria), "DataPortal_Delete", criteria)

      Dim proxy As DataPortalClient.IDataPortalProxy
      proxy = GetDataPortalProxy(RunLocal(method))

      OnDataPortalInitInvoke(Nothing)

      dpContext = New Server.DataPortalContext(GetPrincipal, proxy.IsServerRemote)

      OnDataPortalInvoke(New DataPortalEventArgs(dpContext, DataPortalOperations.Delete))

      Try
        result = proxy.Delete(criteria, dpContext)

      Catch ex As Server.DataPortalException
        result = ex.Result
        If proxy.IsServerRemote Then
          ApplicationContext.SetGlobalContext(result.GlobalContext)
        End If
        Throw New DataPortalException( _
          String.Format("DataPortal.Delete {0} ({1})", My.Resources.Failed, ex.InnerException.InnerException), _
          ex.InnerException, result.ReturnObject)
      End Try

      If proxy.IsServerRemote Then
        ApplicationContext.SetGlobalContext(result.GlobalContext)
      End If

      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Delete))

    Catch ex As Exception
      OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext, DataPortalOperations.Delete, ex))
      Throw
    End Try

  End Sub

#End Region

#Region " Child Data Access methods "

  ''' <summary>
  ''' Creates and initializes a new
  ''' child business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to create.
  ''' </typeparam>
  ''' <param name="parameters">
  ''' Parameters passed to child create method.
  ''' </param>
  Public Function CreateChild(Of T)(ByVal ParamArray parameters() As Object) As T

    Dim portal As New Server.ChildDataPortal
    Return DirectCast(portal.Create(GetType(T), parameters), T)

  End Function

  ''' <summary>
  ''' Creates and loads an existing
  ''' child business object.
  ''' </summary>
  ''' <typeparam name="T">
  ''' Type of business object to retrieve.
  ''' </typeparam>
  ''' <param name="parameters">
  ''' Parameters passed to child fetch method.
  ''' </param>
  Public Function FetchChild(Of T)(ByVal ParamArray parameters() As Object) As T

    Dim portal As New Server.ChildDataPortal
    Return DirectCast(portal.Fetch(GetType(T), parameters), T)

  End Function

  ''' <summary>
  ''' Inserts, updates or deletes an existing
  ''' child business object.
  ''' </summary>
  ''' <param name="child">
  ''' Business object to update.
  ''' </param>
  ''' <param name="parameters">
  ''' Parameters passed to child update method.
  ''' </param>
  Public Sub UpdateChild(ByVal child As Object, ByVal ParamArray parameters() As Object)

    Dim portal As New Server.ChildDataPortal
    portal.Update(child, parameters)

  End Sub

#End Region

#Region " Get DataPortal Proxy "

  Private mLocalPortal As DataPortalClient.IDataPortalProxy
  Private mPortal As DataPortalClient.IDataPortalProxy

  Private Function GetDataPortalProxy( _
    ByVal forceLocal As Boolean) As DataPortalClient.IDataPortalProxy

    If forceLocal Then
      If mLocalPortal Is Nothing Then
        mLocalPortal = New DataPortalClient.LocalProxy
      End If
      Return mLocalPortal

    Else
      If mPortal Is Nothing Then

        Dim proxyTypeName As String = ApplicationContext.DataPortalProxy
        If proxyTypeName = "Local" Then
          mPortal = New DataPortalClient.LocalProxy

        Else
          Dim typeName As String = _
            proxyTypeName.Substring(0, proxyTypeName.IndexOf(",")).Trim
          Dim assemblyName As String = _
            proxyTypeName.Substring(proxyTypeName.IndexOf(",") + 1).Trim
          mPortal = DirectCast(Activator.CreateInstance(assemblyName, _
            typeName).Unwrap, DataPortalClient.IDataPortalProxy)
        End If
      End If
      Return mPortal
    End If

  End Function

  ''' <summary>
  ''' Releases any remote data portal proxy object, so
  ''' the next data portal call will create a new
  ''' proxy instance.
  ''' </summary>
  <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)> _
  Public Sub ReleaseProxy()

    Dim disp = TryCast(mPortal, IDisposable)
    If disp IsNot Nothing Then
      disp.Dispose()
    End If
    mPortal = Nothing

  End Sub
#End Region

#Region " Security "

  Private Function GetPrincipal() As System.Security.Principal.IPrincipal
    If ApplicationContext.AuthenticationType = "Windows" Then
      ' Windows integrated security 
      Return Nothing

    Else
      ' we assume using the CSLA framework security
      Return ApplicationContext.User
    End If
  End Function

#End Region

#Region " Helper methods "

  Private Function RunLocal(ByVal method As MethodInfo) As Boolean

    Return Attribute.IsDefined(method, GetType(RunLocalAttribute), False)

  End Function

#End Region

End Module
