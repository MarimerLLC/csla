//-----------------------------------------------------------------------
// <copyright file="LateBoundObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Enables simple invocation of methods</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.Properties;

namespace Csla.Reflection
{
  /// <summary>
  /// Enables simple invocation of methods
  /// against the contained object using 
  /// late binding.
  /// </summary>
  public class LateBoundObject
  {
    /// <summary>
    /// Object instance managed by LateBoundObject.
    /// </summary>
    public object Instance { get; private set; }

    /// <summary>
    /// Creates an instance of the specified
    /// type and contains it within a new
    /// LateBoundObject.
    /// </summary>
    /// <param name="objectType">
    /// Type of object to create.
    /// </param>
    /// <remarks>
    /// The specified type must implement a
    /// default constructor.
    /// </remarks>
    public LateBoundObject(Type objectType)
      : this(MethodCaller.CreateInstance(objectType))
    { }

    /// <summary>
    /// Contains the provided object within
    /// a new LateBoundObject.
    /// </summary>
    /// <param name="instance">
    /// Object to contain.
    /// </param>
    public LateBoundObject(object instance)
    {
      this.Instance = instance;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public object CallMethodIfImplemented(string method)
    {
      return MethodCaller.CallMethodIfImplemented(this.Instance, method);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public object CallMethodIfImplemented(string method, params object[] parameters)
    {
      return MethodCaller.CallMethodIfImplemented(this.Instance, method, parameters);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public object CallMethod(string method)
    {
      return MethodCaller.CallMethod(this.Instance, method);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public object CallMethod(string method, params object[] parameters)
    {
      return MethodCaller.CallMethod(this.Instance, method, parameters);
    }

    /// <summary>
    /// Invokes a method using the await keyword
    /// if the method returns Task,
    /// otherwise synchronously invokes the method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns></returns>
    public async Task CallMethodTryAsync(string methodName)
    {
      try
      {
        await MethodCaller.CallMethodTryAsync(this.Instance, methodName);
      }
      catch (CallMethodException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new CallMethodException(Instance.GetType().Name + "." + methodName + " " + Resources.MethodCallFailed, ex);
      }
    }

    /// <summary>
    /// Invokes a method using the await keyword
    /// if the method returns Task,
    /// otherwise synchronously invokes the method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public async Task CallMethodTryAsync(string methodName, params object[] parameters)
    {
      try
      {
        await MethodCaller.CallMethodTryAsync(this.Instance, methodName, parameters);
      }
      catch (CallMethodException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new CallMethodException(Instance.GetType().Name + "." + methodName + " " + Resources.MethodCallFailed, ex);
      }
    }

    /// <summary>
    /// Invokes a method using the await keyword
    /// if the method returns Task,
    /// otherwise synchronously invokes the method.
    /// </summary>
    /// <param name="isSync">Is client calling this synchronously</param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public async Task CallMethodTryAsyncDI<T>(bool isSync, params object[] parameters)
      where T : DataPortalOperationAttribute
    {
      var method = ServiceProviderMethodCaller.FindDataPortalMethod<T>(
        Instance, parameters);
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, method.MethodInfo);
        await ServiceProviderMethodCaller.CallMethodTryAsync(Instance, method, parameters).ConfigureAwait(false);
      }
      catch (CallMethodException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new CallMethodException(Instance.GetType().Name + "." + method.MethodInfo.Name + " " + Resources.MethodCallFailed, ex);
      }
    }
  }
}