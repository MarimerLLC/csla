Imports System.Security.Principal
Imports System.Collections.Specialized
Imports Csla.Reflection

Namespace Server

  ''' <summary>
  ''' Implements the server-side DataPortal as discussed
  ''' in Chapter 4.
  ''' </summary>
  Public Class SimpleDataPortal

    Implements IDataPortalServer

#Region "Data Access"

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")> _
    Public Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create

      Dim obj As LateBoundObject = Nothing
      Dim target As IDataPortalTarget = Nothing
      Dim eventArgs = New DataPortalEventArgs(context, objectType, DataPortalOperations.Create)
      Try
        ' create an instance of the business object.
        obj = New LateBoundObject(objectType)

        target = TryCast(obj.Instance, IDataPortalTarget)

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvoke(eventArgs)
          target.MarkNew()
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs)
          obj.CallMethodIfImplemented("MarkNew")
        End If

        ' tell the business object to create its data
        If TypeOf criteria Is Integer Then
          obj.CallMethod("DataPortal_Create")
        Else
          obj.CallMethod("DataPortal_Create", criteria)
        End If

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs)
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs)
        End If

        ' return the populated business object as a result
        Return New DataPortalResult(obj.Instance)
      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.DataPortal_OnDataPortalException(eventArgs, ex)
          Else
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Dim outval As Object = Nothing
        If obj IsNot Nothing Then
          outval = obj.Instance
        End If
        Throw New DataPortalException("DataPortal.Create " & My.Resources.FailedOnServer, ex, New DataPortalResult(outval))
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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")> _
    Public Function Fetch(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch

      Dim obj As LateBoundObject = Nothing
      Dim target As IDataPortalTarget = Nothing
      Dim eventArgs = New DataPortalEventArgs(context, objectType, DataPortalOperations.Fetch)
      Try
        ' create an instance of the business object.
        obj = New LateBoundObject(objectType)

        target = TryCast(obj.Instance, IDataPortalTarget)

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvoke(eventArgs)
          target.MarkOld()
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs)
          obj.CallMethodIfImplemented("MarkOld")
        End If

        ' tell the business object to fetch its data
        If TypeOf criteria Is Integer Then
          obj.CallMethod("DataPortal_Fetch")
        Else
          obj.CallMethod("DataPortal_Fetch", criteria)
        End If

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs)
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs)
        End If

        ' return the populated business object as a result
        Return New DataPortalResult(obj.Instance)
      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.DataPortal_OnDataPortalException(eventArgs, ex)
          Else
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Dim outval As Object = Nothing
        If obj IsNot Nothing Then
          outval = obj.Instance
        End If
        Throw New DataPortalException("DataPortal.Fetch " & My.Resources.FailedOnServer, ex, New DataPortalResult(outval))
      End Try

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update

      Dim operation As DataPortalOperations = DataPortalOperations.Update
      Dim objectType As Type = obj.GetType()
      Dim target = TryCast(obj, IDataPortalTarget)
      Dim lb As LateBoundObject = New LateBoundObject(obj)
      Try
        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvoke(New DataPortalEventArgs(context, objectType, operation))
        Else
          lb.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", New DataPortalEventArgs(context, objectType, operation))
        End If

        ' tell the business object to update itself
        If TypeOf obj Is Core.BusinessBase Then
          Dim busObj As Core.BusinessBase = CType(obj, Core.BusinessBase)
          If busObj.IsDeleted Then
            If (Not busObj.IsNew) Then
              ' tell the object to delete itself
              lb.CallMethod("DataPortal_DeleteSelf")
            End If
            If target IsNot Nothing Then
              target.MarkNew()
            Else
              lb.CallMethodIfImplemented("MarkNew")
            End If
          Else
            If busObj.IsNew Then
              ' tell the object to insert itself
              lb.CallMethod("DataPortal_Insert")
            Else
              ' tell the object to update itself
              lb.CallMethod("DataPortal_Update")
            End If
            If target IsNot Nothing Then
              target.MarkOld()
            Else
              lb.CallMethodIfImplemented("MarkOld")
            End If
          End If
        ElseIf TypeOf obj Is CommandBase Then
          operation = DataPortalOperations.Execute
          ' tell the object to update itself
          lb.CallMethod("DataPortal_Execute")
        Else
          ' this is an updatable collection or some other
          ' non-BusinessBase type of object
          ' tell the object to update itself
          lb.CallMethod("DataPortal_Update")
          If target IsNot Nothing Then
            target.MarkOld()
          Else
            lb.CallMethodIfImplemented("MarkOld")
          End If
        End If

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvokeComplete(New DataPortalEventArgs(context, objectType, operation))
        Else
          lb.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", New DataPortalEventArgs(context, objectType, operation))
        End If

        Return New DataPortalResult(obj)
      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.DataPortal_OnDataPortalException(New DataPortalEventArgs(context, objectType, operation), ex)
          Else
            lb.CallMethodIfImplemented("DataPortal_OnDataPortalException", New DataPortalEventArgs(context, objectType, operation), ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New DataPortalException("DataPortal.Update " & My.Resources.FailedOnServer, ex, New DataPortalResult(obj))
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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId:="Csla.Server.DataPortalException.#ctor(System.String,System.Exception,Csla.Server.DataPortalResult)")> _
    Public Function Delete( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete

      Dim obj As LateBoundObject = Nothing
      Dim target As IDataPortalTarget = Nothing
      Dim eventArgs = New DataPortalEventArgs(context, objectType, DataPortalOperations.Delete)
      Try
        ' create an instance of the business objet
        obj = New LateBoundObject(objectType)

        target = TryCast(obj.Instance, IDataPortalTarget)

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvoke(eventArgs)
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvoke", eventArgs)
        End If

        ' tell the business object to delete itself
        obj.CallMethod("DataPortal_Delete", criteria)

        If target IsNot Nothing Then
          target.DataPortal_OnDataPortalInvokeComplete(eventArgs)
        Else
          obj.CallMethodIfImplemented("DataPortal_OnDataPortalInvokeComplete", eventArgs)
        End If

        Return New DataPortalResult()
      Catch ex As Exception
        Try
          If target IsNot Nothing Then
            target.DataPortal_OnDataPortalException(eventArgs, ex)
          Else
            obj.CallMethodIfImplemented("DataPortal_OnDataPortalException", eventArgs, ex)
          End If
        Catch
          ' ignore exceptions from the exception handler
        End Try
        Throw New DataPortalException("DataPortal.Delete " & My.Resources.FailedOnServer, ex, New DataPortalResult())
      End Try

    End Function

#End Region
  End Class

End Namespace