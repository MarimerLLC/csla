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

        ' route to Enterprise Services if requested
        Dim portal As IDataPortalServer
        If IsTransactionalMethod(method) Then
          portal = New ServicedDataPortal

        Else
          portal = New SimpleDataPortal
        End If

        result = portal.Create(objectType, criteria, context)

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

        ' route to Enterprise Services if requested
        Dim portal As IDataPortalServer
        If IsTransactionalMethod(method) Then
          portal = New ServicedDataPortal

        Else
          portal = New SimpleDataPortal
        End If

        result = portal.Fetch(criteria, context)

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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update

      Try
        SetContext(context)

        Dim result As DataPortalResult

        Dim method As MethodInfo = GetMethod(obj.GetType, "DataPortal_Update")

        ' route to Enterprise Services if requested
        Dim portal As IDataPortalServer
        If IsTransactionalMethod(method) Then
          portal = New ServicedDataPortal

        Else
          portal = New SimpleDataPortal
        End If

        result = portal.Update(obj, context)

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

        ' route to Enterprise Services if requested
        Dim portal As IDataPortalServer
        If IsTransactionalMethod(method) Then
          portal = New ServicedDataPortal

        Else
          portal = New SimpleDataPortal
        End If

        result = portal.Delete(criteria, context)

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
      Csla.ApplicationContext.SetContext(context.ClientContext, context.GlobalContext)

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

