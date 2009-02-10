Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core

  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <param name="parameters">
  ''' Parameters passed to the factory method.
  ''' </param>
  Public Delegate Sub AsyncFactoryDelegate(ByVal completed As [Delegate], ByVal parameters As Object())

  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  Public Delegate Sub AsyncFactoryDelegate(Of R)(ByVal completed As EventHandler(Of DataPortalResult(Of R)))
  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <typeparam name="T">Type of argument</typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <param name="arg">Argument to method.</param>
  Public Delegate Sub AsyncFactoryDelegate(Of R, T)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg As T)
  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <typeparam name="T1">Type of argument</typeparam>
  ''' <param name="arg1">Argument to method.</param>
  ''' <typeparam name="T2">Type of argument</typeparam>
  ''' <param name="arg2">Argument to method.</param>
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2)
  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <typeparam name="T1">Type of argument</typeparam>
  ''' <param name="arg1">Argument to method.</param>
  ''' <typeparam name="T2">Type of argument</typeparam>
  ''' <param name="arg2">Argument to method.</param>
  ''' <typeparam name="T3">Type of argument</typeparam>
  ''' <param name="arg3">Argument to method.</param>
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3)
  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <typeparam name="T1">Type of argument</typeparam>
  ''' <param name="arg1">Argument to method.</param>
  ''' <typeparam name="T2">Type of argument</typeparam>
  ''' <param name="arg2">Argument to method.</param>
  ''' <typeparam name="T3">Type of argument</typeparam>
  ''' <param name="arg3">Argument to method.</param>
  ''' <typeparam name="T4">Type of argument</typeparam>
  ''' <param name="arg4">Argument to method.</param>
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3, T4)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3, ByVal arg4 As T4)
  ''' <summary>
  ''' Delegate for an asynchronous business object 
  ''' factory method with n parameters.
  ''' </summary>
  ''' <typeparam name="R">
  ''' Type of business object to be created.
  ''' </typeparam>
  ''' <param name="completed">
  ''' Delegate pointer to callback method.
  ''' </param>
  ''' <typeparam name="T1">Type of argument</typeparam>
  ''' <param name="arg1">Argument to method.</param>
  ''' <typeparam name="T2">Type of argument</typeparam>
  ''' <param name="arg2">Argument to method.</param>
  ''' <typeparam name="T3">Type of argument</typeparam>
  ''' <param name="arg3">Argument to method.</param>
  ''' <typeparam name="T4">Type of argument</typeparam>
  ''' <param name="arg4">Argument to method.</param>
  ''' <typeparam name="T5">Type of argument</typeparam>
  ''' <param name="arg5">Argument to method.</param>
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3, T4, T5)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3, ByVal arg4 As T4, ByVal arg5 As T5)
End Namespace