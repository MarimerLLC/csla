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

//    /// <summary>
//    /// Gets a value indicating whether the specified method
//    /// returns a Task of object.
//    /// </summary>
//    /// <param name="methodName">Name of the method.</param>
//    /// <returns>True if the method returns a Task of object.</returns>
//    public bool IsMethodAsync(string methodName)
//    {
//      var info = this.Instance.GetType().GetMethod(methodName);
//#if NETFX_CORE
//      var isgeneric = info.ReturnType.IsGenericType();
//#else
//     var isgeneric = info.ReturnType.IsGenericType;
//#endif
//      return (info.ReturnType.Equals(typeof(Task)));
//    }

    ///// <summary>
    ///// Uses reflection to dynamically invoke a method,
    ///// throwing an exception if it is not
    ///// implemented on the target object.
    ///// </summary>
    ///// <param name="method">
    ///// Name of the method.
    ///// </param>
    //public async Task CallMethodAsync(string method)
    //{
    //  try
    //  {
    //    await (Task)MethodCaller.CallMethod(this.Instance, method);
    //  }
    //  catch (InvalidCastException ex)
    //  {
    //    throw new NotSupportedException(
    //      string.Format(Resources.TaskOfObjectException, this.Instance.GetType().Name + "." + method),
    //      ex);
    //  }
    //}

    ///// <summary>
    ///// Uses reflection to dynamically invoke a method,
    ///// throwing an exception if it is not
    ///// implemented on the target object.
    ///// </summary>
    ///// <param name="method">
    ///// Name of the method.
    ///// </param>
    ///// <param name="parameters">
    ///// Parameters to pass to method.
    ///// </param>
    //public async Task CallMethodAsync(string method, params object[] parameters)
    //{
    //  try
    //  {
    //    await (Task)MethodCaller.CallMethod(this.Instance, method, parameters);
    //  }
    //  catch (InvalidCastException ex)
    //  {
    //    throw new NotSupportedException(
    //      string.Format(Resources.TaskOfObjectException, this.Instance.GetType().Name + "." + method),
    //      ex);
    //  }
    //}

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
      catch (Csla.Reflection.CallMethodException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new Csla.Reflection.CallMethodException(Instance.GetType().Name + "." + methodName + " " + Resources.MethodCallFailed, ex);
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
      catch (Csla.Reflection.CallMethodException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new Csla.Reflection.CallMethodException(Instance.GetType().Name + "." + methodName + " " + Resources.MethodCallFailed, ex);
      }
    }
  }
}