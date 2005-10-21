Imports System.Reflection
Imports System.Security.Principal
Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Implements the server-side DataPortal as discussed
  ''' in Chapter 5.
  ''' </summary>
  Public Class SimpleDataPortal

    Implements IDataPortalServer

#Region " Data Access "

    ''' <summary>
    ''' Called by the client-side DataPortal to create a new object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A populated business object.</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")> _
    Public Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create

      Dim obj As Object = Nothing

      Try
        ' create an instance of the business object
        obj = Activator.CreateInstance(objectType, True)

        ' tell the business object we're about to make a DataPortal_xyz call
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvoke", New DataPortalEventArgs(context))

        ' tell the business object to fetch its data
        CallMethod(obj, "DataPortal_Create", criteria)

        ' mark the object as new
        CallMethodIfImplemented(obj, "MarkNew")

        ' tell the business object the DataPortal_xyz call is complete
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvokeComplete", New DataPortalEventArgs(context))

        ' return the populated business object as a result
        Return New DataPortalResult(obj)

      Catch ex As Exception
        Try
          ' tell the business object there was an exception
          CallMethodIfImplemented(obj, "DataPortal_OnDataPortalException", New DataPortalEventArgs(context), ex)
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New DataPortalException("DataPortal.Create " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult(obj))
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataProtal to retrieve an object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Object containing context data from client.</param>
    ''' <returns>A populated business object.</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")> _
    Public Function Fetch(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch

      Dim obj As Object = Nothing

      Try
        ' create an instance of the business object
        obj = CreateBusinessObject(criteria)

        ' tell the business object we're about to make a DataPortal_xyz call
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvoke", New DataPortalEventArgs(context))

        ' tell the business object to fetch its data
        CallMethod(obj, "DataPortal_Fetch", criteria)

        ' mark the object as old
        CallMethodIfImplemented(obj, "MarkOld")

        ' tell the business object the DataPortal_xyz call is complete
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvokeComplete", New DataPortalEventArgs(context))

        ' return the populated business object as a result
        Return New DataPortalResult(obj)

      Catch ex As Exception
        Try
          ' tell the business object there was an exception
          CallMethodIfImplemented(obj, "DataPortal_OnDataPortalException", New DataPortalEventArgs(context), ex)
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New DataPortalException("DataPortal.Fetch " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult(obj))
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to update an object.
    ''' </summary>
    ''' <param name="obj">A reference to the object being updated.</param>
    ''' <param name="context">Context data from the client.</param>
    ''' <returns>A reference to the newly updated object.</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")> _
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update

      Try
        ' tell the business object we're about to make a DataPortal_xyz call
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvoke", New DataPortalEventArgs(context))

        ' tell the business object to update itself
        If TypeOf obj Is Core.BusinessBase Then
          Dim busObj As Core.BusinessBase = DirectCast(obj, Core.BusinessBase)
          If busObj.IsDeleted Then
            If Not busObj.IsNew Then
              ' tell the object to delete itself
              CallMethod(busObj, "DataPortal_DeleteSelf")
            End If
            ' mark the object as new
            CallMethodIfImplemented(busObj, "MarkNew")

          Else
            If busObj.IsNew Then
              ' tell the object to insert itself
              CallMethod(busObj, "DataPortal_Insert")

            Else
              ' tell the object to update itself
              CallMethod(busObj, "DataPortal_Update")
            End If
            ' mark the object as old
            CallMethodIfImplemented(busObj, "MarkOld")
          End If

        ElseIf TypeOf obj Is CommandBase Then
          ' tell the object to update itself
          CallMethod(obj, "DataPortal_Execute")

        Else
          ' this is an updatable collection or some other
          ' non-BusinessBase type of object
          ' tell the object to update itself
          CallMethod(obj, "DataPortal_Update")
          ' mark the object as old
          CallMethodIfImplemented(obj, "MarkOld")
        End If

        ' tell the business object the DataPortal_xyz call is complete
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvokeComplete", New DataPortalEventArgs(context))

        Return New DataPortalResult(obj)

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Update " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult(obj))
      End Try

    End Function

    ''' <summary>
    ''' Called by the client-side DataPortal to delete an object.
    ''' </summary>
    ''' <param name="criteria">Object-specific criteria.</param>
    ''' <param name="context">Context data from the client.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    Public Function Delete(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete

      Dim obj As Object

      Try
        ' create an instance of the business object
        obj = CreateBusinessObject(criteria)

        ' tell the business object we're about to make a DataPortal_xyz call
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvoke", New DataPortalEventArgs(context))

        ' tell the business object to delete itself
        CallMethod(obj, "DataPortal_Delete", criteria)

        ' tell the business object the DataPortal_xyz call is complete
        CallMethodIfImplemented(obj, "DataPortal_OnDataPortalInvokeComplete", New DataPortalEventArgs(context))

        Return New DataPortalResult

      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Delete " & _
          My.Resources.FailedOnServer, ex, New DataPortalResult)
      End Try

    End Function

#End Region

#Region " Creating the business object "

    Private Shared Function CreateBusinessObject(ByVal criteria As Object) As Object

      Dim businessType As Type

      If criteria.GetType.IsSubclassOf(GetType(CriteriaBase)) Then
        ' get the type of the actual business object
        ' from CriteriaBase (using the new scheme)
        businessType = CType(criteria, CriteriaBase).ObjectType

      Else
        ' get the type of the actual business object
        ' based on the nested class scheme in the book
        businessType = criteria.GetType.DeclaringType
      End If

      ' create an instance of the business object
      Return Activator.CreateInstance(businessType, True)

    End Function

#End Region

#Region " Calling a method "

    Private Shared Function CallMethodIfImplemented(ByVal obj As Object, _
      ByVal method As String, ByVal ParamArray parameters() As Object) As Object

      Dim info As MethodInfo = GetMethod(obj.GetType, method)
      If info IsNot Nothing Then
        Return CallMethod(obj, info, parameters)

      Else
        Return Nothing
      End If

    End Function

    Private Shared Function CallMethod(ByVal obj As Object, _
      ByVal method As String, ByVal ParamArray parameters() As Object) As Object

      Dim info As MethodInfo = GetMethod(obj.GetType, method)
      If info Is Nothing Then
        Throw New NotImplementedException( _
          method & " " & My.Resources.MethodNotImplemented)
      End If

      Return CallMethod(obj, info, parameters)

    End Function

    Private Shared Function CallMethod(ByVal obj As Object, _
      ByVal info As MethodInfo, ByVal ParamArray parameters() As Object) As Object

      ' call a private method on the object
      Dim result As Object
      Try
        result = info.Invoke(obj, parameters)

      Catch e As Exception
        Throw New CallMethodException( _
          info.Name & " " & My.Resources.MethodCallFailed, _
          e.InnerException)
      End Try
      Return result

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