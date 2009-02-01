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
  Public MustInherit Class ObjectFactory

    ''' <summary>
    ''' Calls the ValidationRules.CheckRules() method 
    ''' on the specified object, if possible.
    ''' </summary>
    ''' <param name="obj">
    ''' Object on which to call the method.
    ''' </param>
    Protected Sub CheckRules(ByVal obj As Object)
      Dim target As IDataPortalTarget = DirectCast(obj, IDataPortalTarget)
      If target IsNot Nothing Then
        target.CheckRules()
      Else
        MethodCaller.CallMethodIfImplemented(obj, "CheckRules", Nothing)
      End If
    End Sub

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
    ''' Loads a property's managed field with the 
    ''' supplied value calling PropertyHasChanged 
    ''' if the value does change.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of the property.
    ''' </typeparam>
    ''' <param name="obj">
    ''' Object on which to call the method. 
    ''' </param>
    ''' <param name="propertyInfo">
    ''' PropertyInfo object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <remarks>
    ''' No authorization checks occur when this method is called,
    ''' and no PropertyChanging or PropertyChanged events are raised.
    ''' Loading values does not cause validation rules to be
    ''' invoked.
    ''' </remarks>
    Protected Sub LoadProperty(Of P)(ByVal obj As Object, ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)
      Dim target As Core.IManageProperties = DirectCast(obj, Core.IManageProperties)
      If target IsNot Nothing Then
        target.LoadProperty(Of P)(propertyInfo, newValue)
      Else
        Throw New ArgumentException(My.Resources.IManagePropertiesRequiredException)
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

