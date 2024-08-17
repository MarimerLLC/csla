//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that contains cached metadata about data portal</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Csla.Server;

namespace Csla.Reflection
{
  /// <summary>
  /// Class that contains cached metadata about data portal
  /// method to be invoked
  /// </summary>
  public class ServiceProviderMethodInfo
  {
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(DynamicMethod), nameof(Parameters), nameof(IsInjected), nameof(DataPortalMethodInfo))]
#endif
    private bool Initialized { get; set; }

    /// <summary>
    /// Gets or sets the MethodInfo object for the method
    /// </summary>
    public System.Reflection.MethodInfo MethodInfo { get; }

    /// <summary>
    /// Gets delegate representing an expression that
    /// can invoke the method
    /// </summary>
    public DynamicMethodDelegate? DynamicMethod { get; private set; }
    /// <summary>
    /// Gets the parameters for the method
    /// </summary>
    public ParameterInfo[]? Parameters { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the method takes
    /// a param array as its parameter
    /// </summary>
    public bool TakesParamArray { get; private set; }
    /// <summary>
    /// Gets an array of values indicating which
    /// parameters need to be injected
    /// </summary>
    public bool[]? IsInjected { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the method
    /// returns type Task
    /// </summary>
    public bool IsAsyncTask { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the method
    /// returns a Task of T
    /// </summary>
    public bool IsAsyncTaskObject { get; set; }
    /// <summary>
    /// Gets the DataPortalInfo for the method
    /// </summary>
    internal DataPortalMethodInfo? DataPortalMethodInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ServiceProviderMethodInfo"/>-object.
    /// </summary>
    /// <param name="methodInfo">The represented method info.</param>
    /// <exception cref="ArgumentNullException"><paramref name="methodInfo"/> is <see langword="null"/>.</exception>
    public ServiceProviderMethodInfo(System.Reflection.MethodInfo methodInfo)
    {
      MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
    }

    /// <summary>
    /// Initializes and caches the metastate values
    /// necessary to invoke the method
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(DynamicMethod), nameof(Parameters), nameof(IsInjected), nameof(DataPortalMethodInfo))]
#endif
    public void PrepForInvocation()
    {
      if (!Initialized)
      {
        lock (MethodInfo)
        {
          if (!Initialized)
          {
            DynamicMethod = DynamicMethodHandlerFactory.CreateMethod(MethodInfo);
            Parameters = MethodInfo.GetParameters();
            TakesParamArray = (Parameters.Length == 1 && Parameters[0].ParameterType.Equals(typeof(object[])));
            IsInjected = new bool[Parameters.Length];

            int index = 0;
            foreach (var item in Parameters)
            {
              if (item.GetCustomAttributes<InjectAttribute>().Any())
                IsInjected[index] = true;
              index++;
            }
            IsAsyncTask = (MethodInfo.ReturnType == typeof(Task));
            IsAsyncTaskObject = (MethodInfo.ReturnType.IsGenericType && (MethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
            DataPortalMethodInfo = new DataPortalMethodInfo(MethodInfo);

            Initialized = true;
          }
        }
      }
    }
  }
}
