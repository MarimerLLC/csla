using System;

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
  }
}
