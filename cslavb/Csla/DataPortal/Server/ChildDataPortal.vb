Imports System.Reflection
Imports System.Security.Principal
Imports System.Collections.Specialized
Imports Csla.Reflection

Namespace Server

  ''' <summary>
  ''' Invoke data portal methods on child
  ''' objects.
  ''' </summary>
  Public Class ChildDataPortal

#Region " Data Access "

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="parameters">
    ''' Criteria parameters passed from caller.
    ''' </param>
    Public Function Create( _
      ByVal objectType As System.Type, _
      ByVal ParamArray parameters() As Object) As Object

      Dim obj As Object = Nothing

      Try
        ' create an instance of the business object
        obj = Activator.CreateInstance(objectType, True)

        ' mark the object as a child
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild")

        ' tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvoke", _
          New DataPortalEventArgs(Nothing, DataPortalOperations.Create))

        ' tell the business object to fetch its data
        Dim method As MethodInfo = MethodCaller.GetMethod(objectType, "Child_Create", parameters)
        MethodCaller.CallMethod(obj, method, parameters)

        ' mark the object as new
        MethodCaller.CallMethodIfImplemented(obj, "MarkNew")

        ' tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvokeComplete", _
          New DataPortalEventArgs(Nothing, DataPortalOperations.Create))

        ' return the populated business object as a result
        Return obj

      Catch ex As Exception
        Try
          ' tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented( _
            obj, "Child_OnDataPortalException", _
            New DataPortalEventArgs(Nothing, DataPortalOperations.Create), ex)
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Create " & _
          My.Resources.FailedOnServer, ex, obj)
      End Try

    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to retrieve.</param>
    ''' <param name="parameters">
    ''' Criteria parameters passed from caller.
    ''' </param>
    Public Function Fetch( _
      ByVal objectType As Type, _
      ByVal ParamArray parameters() As Object) As Object

      Dim obj As Object = Nothing

      Try
        ' create an instance of the business object
        obj = Activator.CreateInstance(objectType, True)

        ' mark the object as a child
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild")

        ' tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvoke", _
          New DataPortalEventArgs(Nothing, DataPortalOperations.Fetch))

        ' mark the object as old
        MethodCaller.CallMethodIfImplemented(obj, "MarkOld")

        ' tell the business object to fetch its data
        Dim method As MethodInfo = MethodCaller.GetMethod(objectType, "Child_Fetch", parameters)
        MethodCaller.CallMethod(obj, method, parameters)

        ' tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvokeComplete", _
          New DataPortalEventArgs(Nothing, DataPortalOperations.Fetch))

        ' return the populated business object as a result
        Return obj

      Catch ex As Exception
        Try
          ' tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented( _
            obj, "Child_OnDataPortalException", _
            New DataPortalEventArgs(Nothing, DataPortalOperations.Fetch), ex)
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Fetch " & _
          My.Resources.FailedOnServer, ex, obj)
      End Try

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="parameters">
    ''' Parameters passed to method.
    ''' </param>
    Public Sub Update( _
      ByVal obj As Object, _
      ByVal ParamArray parameters() As Object)

      Dim operation = DataPortalOperations.Update

      Try
        ' tell the business object we're about to make a Child_xyz call
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvoke", _
          New DataPortalEventArgs(Nothing, operation))

        ' tell the business object to update itself
        If TypeOf obj Is Core.BusinessBase Then
          Dim busObj As Core.BusinessBase = DirectCast(obj, Core.BusinessBase)
          If busObj.IsDeleted Then
            If Not busObj.IsNew Then
              ' tell the object to delete itself
              MethodCaller.CallMethod(busObj, "Child_DeleteSelf", parameters)
            End If
            ' mark the object as new
            MethodCaller.CallMethodIfImplemented(busObj, "MarkNew")

          Else
            If busObj.IsNew Then
              ' tell the object to insert itself
              MethodCaller.CallMethod(busObj, "Child_Insert", parameters)

            Else
              ' tell the object to update itself
              MethodCaller.CallMethod(busObj, "Child_Update", parameters)
            End If
            ' mark the object as old
            MethodCaller.CallMethodIfImplemented(busObj, "MarkOld")
          End If

        ElseIf TypeOf obj Is CommandBase Then
          ' tell the object to update itself
          MethodCaller.CallMethod(obj, "Child_Execute", parameters)
          operation = DataPortalOperations.Execute

        Else
          ' this is an updatable collection or some other
          ' non-BusinessBase type of object
          ' tell the object to update itself
          MethodCaller.CallMethod(obj, "Child_Update", parameters)
          ' mark the object as old
          MethodCaller.CallMethodIfImplemented(obj, "MarkOld")
        End If

        ' tell the business object the Child_xyz call is complete
        MethodCaller.CallMethodIfImplemented( _
          obj, "Child_OnDataPortalInvokeComplete", _
          New DataPortalEventArgs(Nothing, operation))

      Catch ex As Exception
        Try
          ' tell the business object there was an exception
          MethodCaller.CallMethodIfImplemented( _
            obj, "Child_OnDataPortalException", _
            New DataPortalEventArgs(Nothing, operation), ex)
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Update " & _
          My.Resources.FailedOnServer, ex, obj)
      End Try

    End Sub

#End Region

  End Class

End Namespace
