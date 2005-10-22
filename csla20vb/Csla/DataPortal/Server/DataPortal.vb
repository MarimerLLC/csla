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
    Public Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo = GetMethod(objectType, "DataPortal_Create")

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

        Dim method As MethodInfo = GetMethod(GetObjectType(criteria), "DataPortal_Fetch")

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
        If TypeOf obj Is CommandBase Then
          method = GetMethod(obj.GetType, "DataPortal_Execute")

        Else
          method = GetMethod(obj.GetType, "DataPortal_Update")
        End If

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

        Dim method As MethodInfo = GetMethod(GetObjectType(criteria), "DataPortal_Delete")

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
          ' See if our current principal is different from the caller's principal 
          If Not ReferenceEquals(context.Principal, _
              System.Threading.Thread.CurrentPrincipal) Then

            ' The caller had a different principal, so change ours to match the 
            ' caller's, so all our objects use the caller's security. 
            System.Threading.Thread.CurrentPrincipal = context.Principal
          End If

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

    Private Shared Function GetMethod(ByVal objectType As Type, _
      ByVal method As String) As MethodInfo

      Return objectType.GetMethod(method, _
        BindingFlags.FlattenHierarchy Or _
        BindingFlags.Instance Or _
        BindingFlags.Public Or _
        BindingFlags.NonPublic)

    End Function

#End Region

  End Class

End Namespace

