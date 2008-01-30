Imports Csla.Reflection

Namespace Server

  ''' <summary>
  ''' Invoke data portal methods on child
  ''' objects.
  ''' </summary>
  Public Class ChildDataPortal

#Region " Data Access"

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="parameters">
    ''' Criteria parameters passed from caller.
    ''' </param>
    Public Function Create(ByVal objectType As System.Type, ByVal ParamArray parameters() As Object) As Object
      Dim obj As LateBoundObject = Nothing
      Dim target As IDataPortalTarget = Nothing
      Dim eventArgs = New DataPortalEventArgs(Nothing, objectType, DataPortalOperations.Create)
      Try
        ' create an instance of the business object
        obj = New LateBoundObject(objectType)

        target = TryCast(obj.Instance, IDataPortalTarget)

        If target IsNot Nothing Then
          target.Child_OnDataPortalInvoke(eventArgs)
          target.MarkAsChild()
          target.MarkNew()
        Else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvoke", eventArgs)
          obj.CallMethodIfImplemented("MarkAsChild")
          obj.CallMethodIfImplemented("MarkNew")
        End If


        ' tell the business object to fetch its data
        obj.CallMethod("Child_Create", parameters)

        If target IsNot Nothing Then
          target.Child_OnDataPortalInvokeComplete(eventArgs)
        Else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete", eventArgs)
        End If

        ' return the populated business object as a result
        Return obj.Instance

      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.Child_OnDataPortalException(eventArgs, ex)
          Else
            obj.CallMethodIfImplemented("Child_OnDataPortalException", eventArgs, ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Create " & My.Resources.FailedOnServer, ex, obj.Instance)
      End Try

    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to retrieve.</param>
    ''' <param name="parameters">
    ''' Criteria parameters passed from caller.
    ''' </param>
    Public Function Fetch(ByVal objectType As Type, ByVal ParamArray parameters() As Object) As Object

      Dim obj As LateBoundObject = Nothing
      Dim target As IDataPortalTarget = Nothing
      Dim eventArgs = New DataPortalEventArgs(Nothing, objectType, DataPortalOperations.Fetch)
      Try
        ' create an instance of the business object
        obj = New LateBoundObject(objectType)

        target = TryCast(obj.Instance, IDataPortalTarget)

        If target IsNot Nothing Then
          target.Child_OnDataPortalInvoke(eventArgs)
          target.MarkAsChild()
          target.MarkOld()
        Else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvoke", eventArgs)
          obj.CallMethodIfImplemented("MarkAsChild")
          obj.CallMethodIfImplemented("MarkOld")
        End If

        ' tell the business object to fetch its data
        obj.CallMethod("Child_Fetch", parameters)

        If target IsNot Nothing Then
          target.Child_OnDataPortalInvokeComplete(eventArgs)
        Else
          obj.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete", eventArgs)
        End If

        ' return the populated business object as a result
        Return obj.Instance

      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.Child_OnDataPortalException(eventArgs, ex)
          Else
            obj.CallMethodIfImplemented("Child_OnDataPortalException", eventArgs, ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Fetch " & My.Resources.FailedOnServer, ex, obj.Instance)
      End Try

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="parameters">
    ''' Parameters passed to method.
    ''' </param>
    Public Sub Update(ByVal obj As Object, ByVal ParamArray parameters() As Object)

      Dim busObj = TryCast(obj, Core.BusinessBase)
      If busObj IsNot Nothing AndAlso busObj.IsDirty = False Then
        ' if the object isn't dirty, then just exit
        Exit Sub
      End If

      Dim operation = DataPortalOperations.Update
      Dim objectType As Type = obj.GetType()
      Dim target As IDataPortalTarget = TryCast(obj, IDataPortalTarget)
      Dim lb As LateBoundObject = New LateBoundObject(obj)

      Try
        If target IsNot Nothing Then
          target.Child_OnDataPortalInvoke(New DataPortalEventArgs(Nothing, objectType, operation))
        Else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvoke", New DataPortalEventArgs(Nothing, objectType, operation))
        End If

        ' tell the business object to update itself
        If busObj IsNot Nothing Then
          If busObj.IsDeleted Then
            If (Not busObj.IsNew) Then
              ' tell the object to delete itself
              lb.CallMethod("Child_DeleteSelf", parameters)
            End If
            If target IsNot Nothing Then
              target.MarkNew()
            Else
              lb.CallMethodIfImplemented("MarkNew")
            End If

          Else
            If busObj.IsNew Then
              ' tell the object to insert itself
              lb.CallMethod("Child_Insert", parameters)

            Else
              ' tell the object to update itself
              lb.CallMethod("Child_Update", parameters)
            End If
            If target IsNot Nothing Then
              target.MarkOld()
            Else
              lb.CallMethodIfImplemented("MarkOld")
            End If
          End If

        ElseIf TypeOf obj Is CommandBase Then
          ' tell the object to update itself
          lb.CallMethod("Child_Execute", parameters)
          operation = DataPortalOperations.Execute

        Else
          ' this is an updatable collection or some other
          ' non-BusinessBase type of object
          ' tell the object to update itself
          lb.CallMethod("Child_Update", parameters)
          If target IsNot Nothing Then
            target.MarkOld()
          Else
            lb.CallMethodIfImplemented("MarkOld")
          End If
        End If

        If target IsNot Nothing Then
          target.Child_OnDataPortalInvokeComplete(New DataPortalEventArgs(Nothing, objectType, operation))
        Else
          lb.CallMethodIfImplemented("Child_OnDataPortalInvokeComplete", New DataPortalEventArgs(Nothing, objectType, operation))
        End If

      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.Child_OnDataPortalException(New DataPortalEventArgs(Nothing, objectType, operation), ex)
          Else
            lb.CallMethodIfImplemented("Child_OnDataPortalException", New DataPortalEventArgs(Nothing, objectType, operation), ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New Csla.DataPortalException("ChildDataPortal.Update " & My.Resources.FailedOnServer, ex, obj)
      End Try

    End Sub

#End Region

  End Class

End Namespace