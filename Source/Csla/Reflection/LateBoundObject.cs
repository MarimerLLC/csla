//-----------------------------------------------------------------------
// <copyright file="LateBoundObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Enables simple invocation of methods</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Properties;

namespace Csla.Reflection
{
  /// <summary>
  /// Enables simple invocation of methods
  /// against the contained object using 
  /// late binding.
  /// </summary>
  public class LateBoundObject : Core.IUseApplicationContext
  {
    private ApplicationContext? _applicationContext;

    /// <inheritdoc />
    ApplicationContext Core.IUseApplicationContext.ApplicationContext
    {
      get
      {
        ThrowInvalidOperationExceptionWhenApplicationContextIsNull();
        return _applicationContext!;
      }

      set => _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
    }

    /// <summary>
    /// Object instance managed by LateBoundObject.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// Contains the provided object within
    /// a new LateBoundObject.
    /// </summary>
    /// <param name="instance">
    /// Object to contain.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
    public LateBoundObject(object instance)
    {
      Instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="method"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? CallMethodIfImplemented(string method)
    {
      if (string.IsNullOrWhiteSpace(method))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(method)), nameof(method));

      return MethodCaller.CallMethodIfImplemented(Instance, method);
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
    /// <exception cref="ArgumentException"><paramref name="method"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? CallMethodIfImplemented(string method, params object[] parameters)
    {
      if (string.IsNullOrWhiteSpace(method))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(method)), nameof(method));

      return MethodCaller.CallMethodIfImplemented(Instance, method, parameters);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="method"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? CallMethod(string method)
    {
      if (string.IsNullOrWhiteSpace(method))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(method)), nameof(method));

      return MethodCaller.CallMethod(Instance, method);
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
    /// <exception cref="ArgumentException"><paramref name="method"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public object? CallMethod(string method, params object?[]? parameters)
    {
      if (string.IsNullOrWhiteSpace(method))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(method)), nameof(method));

      return MethodCaller.CallMethod(Instance, method, parameters);
    }

    /// <summary>
    /// Invokes a method using the await keyword
    /// if the method returns Task,
    /// otherwise synchronously invokes the method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public async Task CallMethodTryAsync(string methodName)
    {
      if (string.IsNullOrWhiteSpace(methodName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(methodName)), nameof(methodName));

      try
      {
        await MethodCaller.CallMethodTryAsync(Instance, methodName);
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
    /// <exception cref="ArgumentException"><paramref name="methodName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public async Task CallMethodTryAsync(string methodName, params object?[]? parameters)
    {
      if (string.IsNullOrWhiteSpace(methodName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(methodName)), nameof(methodName));

      try
      {
        await MethodCaller.CallMethodTryAsync(Instance, methodName, parameters);
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

    private ServiceProviderMethodCaller? _serviceProviderMethodCaller;
#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(_serviceProviderMethodCaller))]
#endif
    private ServiceProviderMethodCaller ServiceProviderMethodCaller
    {
      get
      {
        if (_serviceProviderMethodCaller == null)
        {
          ThrowInvalidOperationExceptionWhenApplicationContextIsNull();
          _serviceProviderMethodCaller = (ServiceProviderMethodCaller)_applicationContext!.CreateInstanceDI(typeof(ServiceProviderMethodCaller));
        }
        return _serviceProviderMethodCaller;
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
    /// <exception cref="InvalidOperationException"><see cref="Csla.Core.IUseApplicationContext.ApplicationContext"/> is <see langword="null"/>.</exception>
    public async Task CallMethodTryAsyncDI<T>(bool isSync, params object?[]? parameters)
      where T : DataPortalOperationAttribute
    {
      ThrowInvalidOperationExceptionWhenApplicationContextIsNull();

      var method = ServiceProviderMethodCaller.FindDataPortalMethod<T>(Instance, parameters);
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(_applicationContext!, isSync, method.MethodInfo);
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

    [MemberNotNull(nameof(_applicationContext))]
    private void ThrowInvalidOperationExceptionWhenApplicationContextIsNull()
    {
      if (_applicationContext is null)
        Throw();

      [DoesNotReturn]
      static void Throw() => throw new InvalidOperationException($"{nameof(ApplicationContext)} == null");
    }
  }
}