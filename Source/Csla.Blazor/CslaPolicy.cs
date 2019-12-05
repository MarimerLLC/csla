//-----------------------------------------------------------------------
// <copyright file="CslaPolicy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains methods to manage CSLA permission policy information</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Blazor
{
  /// <summary>
  /// Contains methods to manage CSLA permission policy information.
  /// </summary>
  public static class CslaPolicy
  {
    private static string PolicyPrefix = "Csla:";

    /// <summary>
    /// Gets a string representing a CSLA permissions policy
    /// </summary>
    /// <param name="action">Authorization action</param>
    /// <param name="objectType">Business object type</param>
    /// <returns></returns>
    public static string GetPolicy(Rules.AuthorizationActions action, Type objectType)
    {
      var actionName = action.ToString();
      var typeName = objectType.AssemblyQualifiedName;
      return $"{PolicyPrefix}{actionName}|{typeName}";
    }

    /// <summary>
    /// Gets a permission requirement object representing
    /// a CSLA permissions policy
    /// </summary>
    /// <param name="policy">Permissions policy string</param>
    /// <param name="requirement">Permission requirement object</param>
    /// <returns>True if a requirement object was created</returns>
    public static bool TryGetPermissionRequirement(string policy, out CslaPermissionRequirement requirement)
    {
      if (policy.StartsWith(PolicyPrefix))
      {
        var parts = policy.Substring(PolicyPrefix.Length).Split('|');
        var action = (Rules.AuthorizationActions)Enum.Parse(typeof(Rules.AuthorizationActions), parts[0]);
        var type = Type.GetType(parts[1]);
        requirement = new CslaPermissionRequirement(action, type);
        return true;
      }
      else
      {
        requirement = null;
        return false;
      }
    }
  }
}
