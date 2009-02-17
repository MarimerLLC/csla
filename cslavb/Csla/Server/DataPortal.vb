Imports System.Reflection
Imports System.Security.Principal
Imports System.Collections.Specialized
Imports Csla.Reflection

Namespace Server

  ''' <summary>
  ''' Implements the server-side DataPortal 
  ''' message router as discussed
  ''' in Chapter 4.
  ''' </summary>
  Public Class DataPortal

    Implements IDataPortalServer

#Region "Constructors"

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
      Me.New("CslaAuthorizationProvider")
    End Sub

    ''' <summary>
    ''' This construcor accepts the App Setting name for the Csla Authorization Provider,
    ''' therefore getting the provider type from configuration file
    ''' </summary>
    ''' <param name="cslaAuthorizationProviderAppSettingName"></param>
    ''' <remarks></remarks>
    Protected Sub New(ByVal cslaAuthorizationProviderAppSettingName As String)
      Me.New(GetAuthProviderType(cslaAuthorizationProviderAppSettingName))
    End Sub

    ''' <summary>
    ''' This constructor accepts the Authorization Provider Type as a parameter.
    ''' </summary>
    ''' <param name="authProviderType"></param>
    ''' <remarks></remarks>
    Protected Sub New(ByVal authProviderType As Type)

      If authProviderType Is Nothing Then
        Throw New ArgumentNullException("authProviderType", My.Resources.CslaAuthenticationProviderNotSet)
      End If

      If Not GetType(IAuthorizeDataPortal).IsAssignableFrom(authProviderType) Then
        Throw New ArgumentException(My.Resources.AuthenticationProviderDoesNotImplementIAuthorizeDataPortal, "authProviderType")
      End If

      'only construct the type if it was not constructed already
      If _authorizer Is Nothing Then
        SyncLock _syncRoot
          If _authorizer Is Nothing Then
            _authorizer = DirectCast(Activator.CreateInstance(authProviderType), IAuthorizeDataPortal)
          End If
        End SyncLock

      End If

    End Sub

    Private Shared Function GetAuthProviderType(ByVal cslaAuthorizationProviderAppSettingName As String) As Type
      If cslaAuthorizationProviderAppSettingName Is Nothing Then
        Throw New ArgumentNullException("cslaAuthorizationProviderAppSettingName", My.Resources.AuthorizationProviderNameNotSpecified)
      End If

      'not yet instantiated
      If _authorizer Is Nothing Then
        Dim authProvider = ConfigurationManager.AppSettings(cslaAuthorizationProviderAppSettingName)

        If String.IsNullOrEmpty(authProvider) Then
          Return GetType(NullAuthorizer)
        Else
          Return Type.GetType(authProvider, True)
        End If
      Else
        Return _authorizer.GetType()
      End If

    End Function

#End Region

#Region " Data Access "

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create( _
      ByVal objectType As System.Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Try
        SetContext(context)

        Authorize(New AuthorizeRequest(objectType, criteria, DataPortalOperations.Create))

        Dim result As DataPortalResult

        Dim method = DataPortalMethodCache.GetCreateMethod(objectType, criteria)

        Select Case method.TransactionalType
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Create(objectType, criteria, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Create(objectType, criteria, context)

          Case Else
            Dim portal As New DataPortalSelector()
            result = portal.Create(objectType, criteria, context)
        End Select

        Return result

      Catch ex As Csla.Server.DataPortalException
        Throw

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Create " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult)

      Finally
        ClearContext(context)
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
    Public Function Fetch( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Try
        SetContext(context)

        Authorize(New AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch))

        Dim result As DataPortalResult

        Dim method = Server.DataPortalMethodCache.GetFetchMethod(objectType, criteria)

        Select Case method.TransactionalType
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Fetch(objectType, criteria, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Fetch(objectType, criteria, context)

          Case Else
            Dim portal As New SimpleDataPortal
            result = portal.Fetch(objectType, criteria, context)
        End Select

        Return result

      Catch ex As Csla.Server.DataPortalException
        Dim tmp As Exception = ex
        Throw

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Fetch " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult)

      Finally
        ClearContext(context)
      End Try

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Try
        SetContext(context)

        Authorize(New AuthorizeRequest(obj.GetType(), obj, DataPortalOperations.Update))

        Dim result As DataPortalResult
        Dim method As DataPortalMethodInfo

        Dim factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType())
        If factoryInfo IsNot Nothing Then
          Dim factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName)
          Dim bbase = DirectCast(obj, Core.BusinessBase)

          If bbase IsNot Nothing AndAlso bbase.IsDeleted Then
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, New Object() {obj})
          Else
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, New Object() {obj})
          End If
        Else
          Dim methodName As String
          Dim bbase = DirectCast(obj, Core.BusinessBase)

          If bbase IsNot Nothing Then
            If bbase.IsDeleted Then
              methodName = "DataPortal_DeleteSelf"
            Else
              If bbase.IsNew Then
                methodName = "DataPortal_Insert"
              Else
                methodName = "DataPortal_Update"
              End If
            End If
          ElseIf TypeOf obj Is CommandBase Then
            methodName = "DataPortal_Execute"
          Else
            methodName = "DataPortal_Update"
          End If

          method = DataPortalMethodCache.GetMethodInfo(obj.GetType(), methodName)
          
        End If

        context.TransactionalType = method.TransactionalType
        Select Case method.TransactionalType
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Update(obj, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Update(obj, context)

          Case Else
            Dim portal As New DataPortalSelector()
            result = portal.Update(obj, context)
        End Select

        Return result

      Catch ex As Csla.Server.DataPortalException
        Dim tmp As Exception = ex
        Throw

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Update " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult)

      Finally
        ClearContext(context)
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
    Public Function Delete( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Try
        SetContext(context)

        Authorize(New AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete))

        Dim result As DataPortalResult

        Dim method = Server.DataPortalMethodCache.GetMethodInfo( _
          objectType, "DataPortal_Delete", criteria)

        Select Case method.TransactionalType
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Delete(objectType, criteria, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Delete(objectType, criteria, context)

          Case Else
            Dim portal As New DataPortalSelector
            result = portal.Delete(objectType, criteria, context)
        End Select

        Return result

      Catch ex As Csla.Server.DataPortalException
        Dim tmp As Exception = ex
        Throw

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Delete " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult)

      Finally
        ClearContext(context)
      End Try

    End Function

#End Region

#Region " Context "

    Private Shared Sub SetContext(ByVal context As DataPortalContext)

      ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server)

      ' if the dataportal is not remote then
      ' do nothing
      If Not context.IsRemotePortal Then Exit Sub

      ' set the context value so everyone knows the
      ' code is running on the server
      ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server)

      ' set the app context to the value we got from the
      ' client
      ApplicationContext.SetContext( _
        context.ClientContext, context.GlobalContext)

      ' set the thread's culture to match the client
      System.Threading.Thread.CurrentThread.CurrentCulture = _
        New System.Globalization.CultureInfo(context.ClientCulture)
      System.Threading.Thread.CurrentThread.CurrentUICulture = _
        New System.Globalization.CultureInfo(context.ClientUICulture)

      If ApplicationContext.AuthenticationType = "Windows" Then

        ' When using integrated security, Principal must be Nothing 
        If context.Principal IsNot Nothing Then

          Dim ex As System.Security.SecurityException = New System.Security.SecurityException(My.Resources.NoPrincipalAllowedException)
          ex.Action = System.Security.Permissions.SecurityAction.Deny
          Throw ex

        Else
          'Set .NET to use integrated security
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal)
        End If
      Else
        ' We expect the Principal to be of the type BusinessPrincipal
        If context.Principal IsNot Nothing Then
          If TypeOf context.Principal Is Security.BusinessPrincipalBase Then
            ApplicationContext.User = context.Principal

          Else
            Dim ex As New System.Security.SecurityException( _
              My.Resources.BusinessPrincipalException & " " & _
              CType(context.Principal, Object).ToString())
            ex.Action = System.Security.Permissions.SecurityAction.Deny
            Throw ex
          End If

        Else
          Dim ex As New System.Security.SecurityException( _
            My.Resources.BusinessPrincipalException & " Nothing")
          ex.Action = System.Security.Permissions.SecurityAction.Deny
          Throw ex
        End If
      End If

    End Sub

    Private Shared Sub ClearContext(ByVal context As DataPortalContext)

      ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Client)

      ' if the dataportal is not remote then
      ' do nothing
      If Not context.IsRemotePortal Then Exit Sub

      ApplicationContext.Clear()
      If ApplicationContext.AuthenticationType <> "Windows" Then
        ApplicationContext.User = Nothing
      End If

    End Sub

#End Region

#Region "Authorize"

    Private Shared _syncRoot As Object = New Object()
    Private Shared _authorizer As IAuthorizeDataPortal = Nothing

    ''' <summary>
    ''' Gets or sets a reference to the current authorizer.
    ''' </summary>
    Protected Shared Property Authorizer() As IAuthorizeDataPortal
      Get
        Return _authorizer
      End Get
      Set(ByVal value As IAuthorizeDataPortal)
        _authorizer = value
      End Set
    End Property

    Private Shared Sub Authorize(ByVal clientRequest As AuthorizeRequest)
      _authorizer.Authorize(clientRequest)
    End Sub

    ''' <summary>
    ''' Default implementation of the authorizer that
    ''' allows all data portal calls to pass.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Class NullAuthorizer
      Implements IAuthorizeDataPortal

      ''' <summary>
      ''' Creates an instance of the type.
      ''' </summary>
      ''' <param name="clientRequest">
      ''' Client request information.
      ''' </param>
      Public Sub Authorize(ByVal clientRequest As AuthorizeRequest) Implements IAuthorizeDataPortal.Authorize
        'default is to allow all requests
      End Sub

    End Class


#End Region

#Region " Helper methods "

    Private Shared Function IsTransactionalMethod(ByVal method As MethodInfo) As Boolean

      Return Attribute.IsDefined(method, GetType(TransactionalAttribute))

    End Function

    Private Shared Function TransactionalType( _
      ByVal method As MethodInfo) As TransactionalTypes

      Dim result As TransactionalTypes
      If IsTransactionalMethod(method) Then
        Dim attrib As TransactionalAttribute = _
          DirectCast(Attribute.GetCustomAttribute( _
            method, GetType(TransactionalAttribute)),  _
            TransactionalAttribute)
        result = attrib.TransactionType

      Else
        result = TransactionalTypes.Manual
      End If
      Return result

    End Function

#End Region

  End Class

End Namespace

