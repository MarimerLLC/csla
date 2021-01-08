//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that contains cached metadata about data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Csla.Server;

namespace Csla.Reflection
{
  /// <summary>
  /// Class that contains cached metadata about data portal
  /// method to be invoked
  /// </summary>
  public class ServiceProviderMethodInfo
  {
    private bool _initialized;

    /// <summary>
    /// Gets or sets the MethodInfo object for the method
    /// </summary>
    public System.Reflection.MethodInfo MethodInfo { get; set; }
    /// <summary>
    /// Gets delegate representing an expression that
    /// can invoke the method
    /// </summary>
    public DynamicMethodDelegate DynamicMethod { get; private set; }
    /// <summary>
    /// Gets the parameters for the method
    /// </summary>
    public ParameterInfo[] Parameters { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the method takes
    /// a param array as its parameter
    /// </summary>
    public bool TakesParamArray { get; private set; }
    /// <summary>
    /// Gets an array of values indicating which
    /// parameters need to be injected
    /// </summary>
    public bool[] IsInjected { get; private set; }
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
    internal DataPortalMethodInfo DataPortalMethodInfo { get; private set; }

    /// <summary>
    /// Initializes and caches the metastate values
    /// necessary to invoke the method
    /// </summary>
    public void PrepForInvocation()
    {
      if (!_initialized)
      {
        lock (MethodInfo)
        {
          if (!_initialized)
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

            _initialized = true;
          }
        }
      }
    }
  }
}
