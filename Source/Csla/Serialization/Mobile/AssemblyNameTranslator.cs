//-----------------------------------------------------------------------
// <copyright file="AssemblyNameTranslator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Translates assembly names to and from short</summary>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Csla.Properties;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Translates assembly names to and from short
  /// code values for serialization in MobileFormatter
  /// </summary>
  public static class AssemblyNameTranslator
  {
    private static string? _coreLibAssembly = null;
    private static string CoreLibAssembly
    {
      get
      {
        if (_coreLibAssembly == null)
        {
          _coreLibAssembly = typeof(object).AssemblyQualifiedName!;
          _coreLibAssembly = _coreLibAssembly.Substring(_coreLibAssembly.IndexOf(", ") + 2);
        }
        return _coreLibAssembly;
      }
    }
    private static string? _cslaLibAssembly = null;
    private static string CslaLibAssembly
    {
      get
      {
        if (_cslaLibAssembly == null)
        {
          _cslaLibAssembly = typeof(NullPlaceholder).AssemblyQualifiedName!;
          _cslaLibAssembly = _cslaLibAssembly.Substring(_cslaLibAssembly.IndexOf(", ") + 2);
        }
        return _cslaLibAssembly;
      }
    }

    private static readonly string CORELIB = "/n";
    private static readonly string CSLALIB = "/c";

    /// <summary>
    /// Gets the assembly qualified type name with any use
    /// of common assemblies translated to
    /// a short code.
    /// </summary>
    /// <param name="type">Original type.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static string GetAssemblyQualifiedName(Type type)
    {
      return GetSerializationName(type, true);
    }

    /// <summary>
    /// Gets the assembly qualified type name with any use
    /// of common assemblies translated to
    /// a short code.
    /// </summary>
    /// <param name="type">Original type.</param>
    /// <param name="useStrongName">Use strong name as type name</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static string GetSerializationName(Type type, bool useStrongName)
    {
      if (type is null)
        throw new ArgumentNullException(nameof(type));

      if (useStrongName)
      {
        var result = type.AssemblyQualifiedName ?? throw new InvalidOperationException(string.Format(Resources.TypeAssemblyQualifiedNameIsNull, type));
        result = result.Replace(CoreLibAssembly, CORELIB);
        result = result.Replace(CslaLibAssembly, CSLALIB);
        return result;
      }
      else
      {
        var assemblyName = type.Assembly == typeof(object).Assembly ? "/n" :
            type.Assembly == typeof(NullPlaceholder).Assembly ? "/c" :
            type.Assembly.GetName().Name;
        return type.FullName + ", " + assemblyName;
      }
    }

    /// <summary>
    /// Gets the assembly qualified type name after
    /// translating the assembly name codes to the platform-
    /// specific assembly name.
    /// </summary>
    /// <param name="typeName"></param>
    public static string GetAssemblyQualifiedName(string typeName)
    {
      var result = typeName;
      result = result.Replace(CORELIB, CoreLibAssembly);
      result = result.Replace(CSLALIB, CslaLibAssembly);
      return result;
    }
  }
}
