Imports System.Reflection
Imports System.Security.Principal
Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Implements the server-side DataPortal 
  ''' message router as discussed
  ''' in Chapter 5.
  ''' </summary>
  Public Class DataPortal

    Implements IDataPortalServer

#Region " Data Access "

    ''' <summary>
    ''' Called by the client-side DataPortal to create a new object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A populated business object.</returns>
    Public Function Create( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult _
      Implements IDataPortalServer.Create

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo = _
          MethodCaller.GetMethod(objectType, "DataPortal_Create", criteria)

        Select Case TransactionalType(method)
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
            Dim portal As New SimpleDataPortal
            result = portal.Create(objectType, criteria, context)
        End Select

        ClearContext(context)
        Return result

      Catch
        ClearContext(context)
        Throw
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataProtal to retrieve an object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Object containing context data from client.</param>
    ''' <returns>A populated business object.</returns>
    Public Function Fetch(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo = _
          MethodCaller.GetMethod(MethodCaller.GetObjectType(criteria), "DataPortal_Fetch", criteria)

        Select Case TransactionalType(method)
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Fetch(criteria, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Fetch(criteria, context)

          Case Else
            Dim portal As New SimpleDataPortal
            result = portal.Fetch(criteria, context)
        End Select

        ClearContext(context)
        Return result

      Catch
        ClearContext(context)
        Throw
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to update an object.
    ''' </summary>
    ''' <param name="obj">A reference to the object being updated.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A reference to the newly updated object.</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Function Update(ByVal obj As Object, _
      ByVal context As DataPortalContext) As DataPortalResult _
      Implements IDataPortalServer.Update

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo
        Dim methodName As String
        If TypeOf obj Is CommandBase Then
          methodName = "DataPortal_Execute"

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

        Select Case TransactionalType(method)
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
            Dim portal As New SimpleDataPortal
            result = portal.Update(obj, context)
        End Select

        ClearContext(context)
        Return result

      Catch
        ClearContext(context)
        Throw
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to delete an object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    Public Function Delete(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo = _
          MethodCaller.GetMethod(MethodCaller.GetObjectType(criteria), "DataPortal_Delete", criteria)

        Select Case TransactionalType(method)
          Case TransactionalTypes.EnterpriseServices
            Dim portal As New ServicedDataPortal
            Try
              result = portal.Delete(criteria, context)

            Finally
              portal.Dispose()
            End Try

          Case TransactionalTypes.TransactionScope
            Dim portal As New TransactionalDataPortal
            result = portal.Delete(criteria, context)

          Case Else
            Dim portal As New SimpleDataPortal
            result = portal.Delete(criteria, context)
        End Select

        ClearContext(context)
        Return result

      Catch
        ClearContext(context)
        Throw
      End Try

    End Function

#End Region

#Region " Context "

    Private Shared Sub SetContext(ByVal context As DataPortalContext)

      ' if the dataportal is not remote then
      ' do nothing
      If Not context.IsRemotePortal Then Exit Sub

      ' set the app context to the value we got from the
      ' client
      ApplicationContext.SetContext(context.ClientContext, context.GlobalContext)

      ' set the context value so everyone knows the
      ' code is running on the server
      ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server)

      ' set the thread's culture to match the client
      System.Threading.Thread.CurrentThread.CurrentCulture = _
        New System.Globalization.CultureInfo(context.ClientCulture)
      System.Threading.Thread.CurrentThread.CurrentUICulture = _
        New System.Globalization.CultureInfo(context.ClientUICulture)

      If ApplicationContext.AuthenticationType = "Windows" Then
        ' When using integrated security, Principal must be Nothing 
        If context.Principal Is Nothing Then
          ' Set .NET to use integrated security 
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal)
          Exit Sub

        Else
          Throw New System.Security.SecurityException( _
            My.Resources.NoPrincipalAllowedException)
        End If
      End If

      ' We expect the Principal to be of the type BusinessPrincipal
      If context.Principal IsNot Nothing Then
        If TypeOf context.Principal Is Security.BusinessPrincipalBase Then
          System.Threading.Thread.CurrentPrincipal = context.Principal

        Else
          Throw New System.Security.SecurityException( _
            My.Resources.BusinessPrincipalException & " " & CType(context.Principal, Object).ToString())
        End If

      Else
        Throw New System.Security.SecurityException( _
          My.Resources.BusinessPrincipalException & " Nothing")
      End If

    End Sub

    Private Shared Sub ClearContext(ByVal context As DataPortalContext)

      ' if the dataportal is not remote then
      ' do nothing
      If Not context.IsRemotePortal Then Exit Sub

      ApplicationContext.Clear()
      If ApplicationContext.AuthenticationType <> "Windows" Then
        System.Threading.Thread.CurrentPrincipal = Nothing
      End If

    End Sub

#End Region

#Region " Helper methods "

    Private Shared Function IsTransactionalMethod(ByVal method As MethodInfo) As Boolean

      Return Attribute.IsDefined(method, GetType(TransactionalAttribute))

    End Function

    Private Shared Function TransactionalType(ByVal method As MethodInfo) As TransactionalTypes

      Dim result As TransactionalTypes
      If IsTransactionalMethod(method) Then
        Dim attrib As TransactionalAttribute = _
          DirectCast(Attribute.GetCustomAttribute(method, GetType(TransactionalAttribute)), _
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

