Imports System.Threading
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Configuration

''' <summary>
''' This is the client-side DataPortal as described in
''' Chapter 5.
''' </summary>
Public Class DataPortal

  Private Shared mPortalRemote As Boolean = False

#Region " DataPortal events "

  ''' <summary>
  ''' Raised by DataPortal prior to calling the 
  ''' requested server-side DataPortal method.
  ''' </summary>
  Public Shared Event OnDataPortalInvoke(ByVal e As DataPortalEventArgs)
  ''' <summary>
  ''' Raised by DataPortal after the requested 
  ''' server-side DataPortal method call is complete.
  ''' </summary>
  Public Shared Event OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

#End Region

#Region " Data Access methods "

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  Public Shared Function Create(ByVal Criteria As Object) As Object

    Dim result As Server.DataPortalResult

    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Create")

    Dim forceLocal As Boolean = RunLocal(method)
    Dim isRemotePortal As Boolean = mPortalRemote AndAlso Not forceLocal
    Dim dpContext As New Server.DataPortalContext(GetPrincipal, isRemotePortal)

    RaiseEvent OnDataPortalInvoke(New DataPortalEventArgs(dpContext))

    Try
      If IsTransactionalMethod(method) Then
        result = CType(ServicedPortal(forceLocal).Create(Criteria, dpContext), Server.DataPortalResult)

      Else
        result = CType(Portal(forceLocal).Create(Criteria, dpContext), Server.DataPortalResult)
      End If

    Catch ex As Server.DataPortalException
      result = ex.Result
      If isRemotePortal Then
        RestoreContext(result)
      End If
      Throw New DataPortalException("DataPortal.Create " & GetResourceString("Failed"), ex.InnerException, result.ReturnObject)
    End Try

    If isRemotePortal Then
      RestoreContext(result)
      Serialization.SerializationNotification.OnDeserialized(result.ReturnObject)
    End If

    RaiseEvent OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext))

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  Public Shared Function Fetch(ByVal Criteria As Object) As Object

    Dim result As Server.DataPortalResult

    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Fetch")

    Dim forceLocal As Boolean = RunLocal(method)
    Dim isRemotePortal As Boolean = mPortalRemote AndAlso Not forceLocal
    Dim dpContext As New Server.DataPortalContext(GetPrincipal, isRemotePortal)

    RaiseEvent OnDataPortalInvoke(New DataPortalEventArgs(dpContext))

    Try
      If IsTransactionalMethod(method) Then
        result = CType(ServicedPortal(forceLocal).Fetch(Criteria, dpContext), Server.DataPortalResult)

      Else
        result = CType(Portal(forceLocal).Fetch(Criteria, dpContext), Server.DataPortalResult)
      End If

    Catch ex As Server.DataPortalException
      result = ex.Result
      If isRemotePortal Then
        RestoreContext(result)
      End If
      Throw New DataPortalException("DataPortal.Fetch " & GetResourceString("Failed"), ex.InnerException, result.ReturnObject)
    End Try

    If isRemotePortal Then
      RestoreContext(result)
      Serialization.SerializationNotification.OnDeserialized(result.ReturnObject)
    End If

    RaiseEvent OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext))

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by the <see cref="M:CSLA.BusinessBase.Save" /> method to
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
  Public Shared Function Update(ByVal obj As Object) As Object

    Dim result As Server.DataPortalResult

    Dim method As MethodInfo = GetMethod(obj.GetType, "DataPortal_Update")

    Dim forceLocal As Boolean = RunLocal(method)
    Dim isRemotePortal As Boolean = mPortalRemote AndAlso Not forceLocal
    Dim dpContext As New Server.DataPortalContext(GetPrincipal, isRemotePortal)

    RaiseEvent OnDataPortalInvoke(New DataPortalEventArgs(dpContext))

    If isRemotePortal Then
      Serialization.SerializationNotification.OnSerializing(obj)
    End If

    Try
      If IsTransactionalMethod(method) Then
        result = CType(ServicedPortal(forceLocal).Update(obj, dpContext), Server.DataPortalResult)

      Else
        result = CType(Portal(forceLocal).Update(obj, dpContext), Server.DataPortalResult)
      End If

    Catch ex As Server.DataPortalException
      result = ex.Result
      If isRemotePortal Then
        RestoreContext(result)
      End If
      Throw New DataPortalException("DataPortal.Update " & GetResourceString("Failed"), ex.InnerException, result.ReturnObject)
    End Try

    If isRemotePortal Then
      RestoreContext(result)
      Serialization.SerializationNotification.OnSerialized(obj)
      Serialization.SerializationNotification.OnDeserialized(result.ReturnObject)
    End If

    RaiseEvent OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext))

    Return result.ReturnObject

  End Function

  ''' <summary>
  ''' Called by a <c>Shared</c> method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  Public Shared Sub Delete(ByVal Criteria As Object)

    Dim result As Server.DataPortalResult

    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Delete")

    Dim forceLocal As Boolean = RunLocal(method)
    Dim isRemotePortal As Boolean = mPortalRemote AndAlso Not forceLocal
    Dim dpContext As New Server.DataPortalContext(GetPrincipal, isRemotePortal)

    RaiseEvent OnDataPortalInvoke(New DataPortalEventArgs(dpContext))

    Try
      If IsTransactionalMethod(method) Then
        result = CType(ServicedPortal(forceLocal).Delete(Criteria, dpContext), Server.DataPortalResult)

      Else
        result = CType(Portal(forceLocal).Delete(Criteria, dpContext), Server.DataPortalResult)
      End If

    Catch ex As Server.DataPortalException
      result = ex.Result
      If isRemotePortal Then
        RestoreContext(result)
      End If
      Throw New DataPortalException("DataPortal.Delete " & GetResourceString("Failed"), ex.InnerException, result.ReturnObject)
    End Try

    If isRemotePortal Then
      RestoreContext(result)
    End If

    RaiseEvent OnDataPortalInvokeComplete(New DataPortalEventArgs(dpContext))

  End Sub

#End Region

#Region " Server-side DataPortal "

  Private Shared mPortal As Server.DataPortal
  Private Shared mServicedPortal As Server.ServicedDataPortal.DataPortal
  Private Shared mRemotePortal As Server.DataPortal
  Private Shared mRemoteServicedPortal As Server.ServicedDataPortal.DataPortal

  Private Shared Function Portal(ByVal forceLocal As Boolean) As Server.DataPortal

    If Not forceLocal AndAlso mPortalRemote Then
      ' return remote instance
      If mRemotePortal Is Nothing Then
        mRemotePortal = CType(Activator.GetObject(GetType(Server.DataPortal), PORTAL_SERVER), _
          Server.DataPortal)
      End If
      Return mRemotePortal

    Else
      ' return local instance
      If mPortal Is Nothing Then
        mPortal = New Server.DataPortal
      End If
      Return mPortal
    End If

  End Function

  Private Shared Function ServicedPortal(ByVal forceLocal As Boolean) As Server.ServicedDataPortal.DataPortal

    If Not forceLocal AndAlso mPortalRemote Then
      ' return remote instance
      If mRemoteServicedPortal Is Nothing Then
        mRemoteServicedPortal = _
          CType(Activator.GetObject(GetType(Server.ServicedDataPortal.DataPortal), _
                                    SERVICED_PORTAL_SERVER), _
          Server.ServicedDataPortal.DataPortal)
      End If
      Return mRemoteServicedPortal

    Else
      ' return local instance
      If mServicedPortal Is Nothing Then
        mServicedPortal = New Server.ServicedDataPortal.DataPortal
      End If
      Return mServicedPortal
    End If

  End Function

  Private Shared Function PORTAL_SERVER() As String
    Return ConfigurationSettings.AppSettings("PortalServer")
  End Function

  Private Shared Function SERVICED_PORTAL_SERVER() As String
    Return ConfigurationSettings.AppSettings("ServicedPortalServer")
  End Function

#End Region

#Region " Security "

  Private Shared Function AUTHENTICATION() As String
    Return ConfigurationSettings.AppSettings("Authentication")
  End Function

  Private Shared Function GetPrincipal() As System.Security.Principal.IPrincipal
    If AUTHENTICATION() = "Windows" Then
      ' Windows integrated security 
      Return Nothing

    Else
      ' we assume using the CSLA framework security
      Return System.Threading.Thread.CurrentPrincipal
    End If
  End Function

#End Region

#Region " Helper methods "

  Private Shared Sub RestoreContext(ByVal result As Object)

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.GlobalContext")
    Threading.Thread.SetData(slot, CType(result, Server.DataPortalResult).GlobalContext)

  End Sub

  Private Shared Function IsTransactionalMethod(ByVal Method As MethodInfo) As Boolean

    Return Attribute.IsDefined(Method, GetType(TransactionalAttribute))

    'Dim attrib() As Object = Method.GetCustomAttributes(GetType(TransactionalAttribute), True)
    'Return (UBound(attrib) > -1)

  End Function

  Private Shared Function RunLocal(ByVal Method As MethodInfo) As Boolean

    Return Attribute.IsDefined(Method, GetType(RunLocalAttribute))

  End Function

  Private Shared Function GetMethod(ByVal ObjectType As Type, ByVal method As String) As MethodInfo
    Return ObjectType.GetMethod(method, BindingFlags.FlattenHierarchy Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
  End Function

  Private Shared Function GetObjectType(ByVal criteria As Object) As Type

    If criteria.GetType.IsSubclassOf(GetType(CriteriaBase)) Then
      ' get the type of the actual business object
      ' from CriteriaBase (using the new scheme)
      Return CType(criteria, CriteriaBase).ObjectType

    Else
      ' get the type of the actual business object
      ' based on the nested class scheme in the book
      Return criteria.GetType.DeclaringType
    End If

  End Function

  Shared Sub New()
    ' see if we need to configure remoting at all
    If Len(PORTAL_SERVER) > 0 OrElse Len(SERVICED_PORTAL_SERVER) > 0 Then
      mPortalRemote = True
      ' create and register our custom HTTP channel
      ' that uses the binary formatter
      Dim properties As New Hashtable
      properties("name") = "HttpBinary"

      If AUTHENTICATION() = "Windows" Then
        ' make sure we pass the user's Windows credentials
        ' to the server
        properties("useDefaultCredentials") = True
      End If

      Dim formatter As New BinaryClientFormatterSinkProvider

      Dim channel As New HttpChannel(properties, formatter, Nothing)

      ChannelServices.RegisterChannel(channel)

      '' register the data portal types as being remote
      'If Len(PORTAL_SERVER) > 0 Then
      '  RemotingConfiguration.RegisterWellKnownClientType( _
      '    GetType(Server.DataPortal), PORTAL_SERVER)
      'End If
      'If Len(SERVICED_PORTAL_SERVER) > 0 Then
      '  RemotingConfiguration.RegisterWellKnownClientType( _
      '    GetType(Server.ServicedDataPortal.DataPortal), SERVICED_PORTAL_SERVER)
      'End If
    End If

  End Sub

#End Region

End Class
