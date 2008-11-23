Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Reflection

Namespace Server

  ''' <summary>
  ''' Base class to be used when creating a data portal
  ''' factory object.
  ''' </summary>
  ''' <remarks></remarks>
  Public MustInherit Class ObjectFactory

    ''' <summary>
    ''' Calls the MarkOld method on the specified
    ''' object, if possible.
    ''' </summary>
    ''' <param name="obj">
    ''' Object on which to call the method.
    ''' </param>
    Protected Sub MarkOld(ByVal obj As Object)
      Dim target = DirectCast(obj, IDataPortalTarget)
      If target IsNot Nothing Then
        target.MarkOld()
      Else
        MethodCaller.CallMethodIfImplemented(obj, "MarkOld", Nothing)
      End If

    End Sub

    ''' <summary>
    ''' Calls the MarkNew method on the specified
    ''' object, if possible.
    ''' </summary>
    ''' <param name="obj">
    ''' Object on which to call the method.
    ''' </param>
    Protected Sub MarkNew(ByVal obj As Object)
      Dim target = DirectCast(obj, IDataPortalTarget)
      If target IsNot Nothing Then
        target.MarkNew()
      Else
        MethodCaller.CallMethodIfImplemented(obj, "MarkNew", Nothing)
      End If
    End Sub

    ''' <summary>
    ''' Calls the MarkAsChild method on the specified
    ''' object, if possible.
    ''' </summary>
    ''' <param name="obj">
    ''' Object on which to call the method.
    ''' </param>
    Protected Sub MarkAsChild(ByVal obj As Object)
      Dim target = DirectCast(obj, IDataPortalTarget)
      If target IsNot Nothing Then
        target.MarkAsChild()
      Else
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild", Nothing)
      End If
    End Sub

    ''' <summary>
    ''' By wrapping this property inside Using block
    ''' you can set property values on 
    ''' <paramref name="businessObject">business object</paramref>
    ''' without raising PropertyChanged events
    ''' and checking user rights.
    ''' </summary>
    ''' <param name="businessObject">
    ''' Object on with you would like to set property values
    ''' </param>
    ''' <returns>
    ''' An instance of IDisposable object that allows
    ''' bypassing of normal authorization checks during
    ''' property setting.
    ''' </returns>
    Protected Function BypassPropertyChecks(ByVal businessObject As Core.BusinessBase) As IDisposable
      Return businessObject.BypassPropertyChecks
    End Function

  End Class

End Namespace

