//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that contains cached metadata about data portal</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
    [MemberNotNullWhen(true, nameof(DynamicMethod), nameof(Parameters), nameof(IsInjected), nameof(AllowNull), nameof(DataPortalMethodInfo))]
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
    /// Gets an array of values indicating which
    /// injected parameters allow null values
    /// </summary>
    public bool[]? AllowNull { get; private set; }
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
    [MemberNotNull(nameof(DynamicMethod), nameof(Parameters), nameof(IsInjected), nameof(AllowNull), nameof(DataPortalMethodInfo))]
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
            AllowNull = new bool[Parameters.Length];

            int index = 0;
            foreach (var item in Parameters)
            {
              var injectAttribute = item.GetCustomAttributes<InjectAttribute>().FirstOrDefault();
              if (injectAttribute != null)
              {
                IsInjected[index] = true;
                AllowNull[index] = injectAttribute.AllowNull || ParameterAllowsNull(item);
              }
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

    private static bool ParameterAllowsNull(ParameterInfo parameter)
    {
      if (Nullable.GetUnderlyingType(parameter.ParameterType) != null)
        return true;

      if (!parameter.ParameterType.IsValueType)
        return IsNullableReference(parameter);

      return false;
    }

    private static bool IsNullableReference(ParameterInfo parameter)
    {
      if (TryGetNullableAttribute(parameter.CustomAttributes, out var nullableFlag))
        return nullableFlag == NullableFlag;

      var context = GetNullableContext(parameter);
      return context == NullableFlag;
    }

    private static byte? GetNullableContext(ParameterInfo parameter)
    {
      var member = parameter.Member;
      if (member != null)
      {
        if (TryGetNullableContext(member.CustomAttributes, out var memberFlag))
          return memberFlag;

        var declaringType = member.DeclaringType;
        while (declaringType != null)
        {
          if (TryGetNullableContext(declaringType.CustomAttributes, out var typeFlag))
            return typeFlag;

          declaringType = declaringType.DeclaringType;
        }

        var assembly = member.Module?.Assembly;
        if (assembly != null && TryGetNullableContext(assembly.CustomAttributes, out var assemblyFlag))
          return assemblyFlag;
      }

      return null;
    }

    private static bool TryGetNullableContext(IEnumerable<CustomAttributeData> attributes, out byte flag)
    {
      foreach (var attribute in attributes)
      {
        if (attribute.AttributeType.FullName == NullableContextAttributeName &&
            attribute.ConstructorArguments.Count == 1 &&
            attribute.ConstructorArguments[0].ArgumentType == typeof(byte))
        {
          flag = (byte)attribute.ConstructorArguments[0].Value!;
          return true;
        }
      }

      flag = default;
      return false;
    }

    private static bool TryGetNullableAttribute(IEnumerable<CustomAttributeData> attributes, out byte flag)
    {
      foreach (var attribute in attributes)
      {
        if (attribute.AttributeType.FullName == NullableAttributeName &&
            TryExtractFlag(attribute, out flag))
        {
          return true;
        }
      }

      flag = default;
      return false;
    }

    private static bool TryExtractFlag(CustomAttributeData attribute, out byte flag)
    {
      if (attribute.ConstructorArguments.Count == 1)
      {
        var argument = attribute.ConstructorArguments[0];
        if (argument.ArgumentType == typeof(byte))
        {
          flag = (byte)argument.Value!;
          return true;
        }

        if (argument.Value is IEnumerable<CustomAttributeTypedArgument> nestedArguments)
        {
          foreach (var nested in nestedArguments)
          {
            if (nested.ArgumentType == typeof(byte))
            {
              flag = (byte)nested.Value!;
              return true;
            }
            break;
          }
        }
      }

      flag = default;
      return false;
    }

    private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
    private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";
    private const byte NullableFlag = 2;
  }
}
