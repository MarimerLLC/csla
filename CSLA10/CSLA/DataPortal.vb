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

#Region " Data Access methods "

  ''' <summary>
  ''' Called by a factory method in a business class to create 
  ''' a new object, which is loaded with default
  ''' values from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <returns>A new object, populated with default values.</returns>
  Public Shared Function Create(ByVal Criteria As Object) As Object

    Dim obj As Object
    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Create")

    Dim forceLocal As Boolean = RunLocal(method)

    If IsTransactionalMethod(method) Then
      obj = ServicedPortal(forceLocal).Create(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))

    Else
      obj = Portal(forceLocal).Create(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))
    End If

    If mPortalRemote AndAlso Not forceLocal Then
      Serialization.SerializationNotification.OnDeserialized(obj)
    End If
    Return obj

  End Function

  ''' <summary>
  ''' Called by a factory method in a business class to retrieve
  ''' an object, which is loaded with values from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <returns>An object populated with values from the database.</returns>
  Public Shared Function Fetch(ByVal Criteria As Object) As Object

    Dim obj As Object

    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Fetch")

    Dim forceLocal As Boolean = RunLocal(method)

    If IsTransactionalMethod(method) Then
      obj = ServicedPortal(forceLocal).Fetch(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))

    Else
      obj = Portal(forceLocal).Fetch(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))
    End If

    If mPortalRemote AndAlso Not forceLocal Then
      Serialization.SerializationNotification.OnDeserialized(obj)
    End If
    Return obj

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

    Dim updated As Object

    Dim method As MethodInfo = GetMethod(obj.GetType, "DataPortal_Update")

    Dim forceLocal As Boolean = RunLocal(method)

    If mPortalRemote AndAlso Not forceLocal Then
      Serialization.SerializationNotification.OnSerializing(obj)
    End If

    If IsTransactionalMethod(method) Then
      updated = ServicedPortal(forceLocal).Update(obj, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote))

    Else
      updated = Portal(forceLocal).Update(obj, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote))
    End If

    If mPortalRemote AndAlso Not forceLocal Then
      Serialization.SerializationNotification.OnSerialized(obj)
      Serialization.SerializationNotification.OnDeserialized(updated)
    End If
    Return updated

  End Function

  ''' <summary>
  ''' Called by a <c>Shared</c> method in the business class to cause
  ''' immediate deletion of a specific object from the database.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  Public Shared Sub Delete(ByVal Criteria As Object)

    Dim method As MethodInfo = GetMethod(GetObjectType(Criteria), "DataPortal_Delete")

    Dim forceLocal As Boolean = RunLocal(method)

    If IsTransactionalMethod(method) Then
      ServicedPortal(forceLocal).Delete(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))

    Else
      Portal(forceLocal).Delete(Criteria, _
        New Server.DataPortalContext(GetPrincipal, mPortalRemote AndAlso Not forceLocal))
    End If

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
        mPortal = New Server.DataPortal()
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
        mServicedPortal = New Server.ServicedDataPortal.DataPortal()
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
      Dim properties As New Hashtable()
      properties("name") = "HttpBinary"

      Dim formatter As New BinaryClientFormatterSinkProvider()

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
