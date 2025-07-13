//-----------------------------------------------------------------------
// <copyright file="CslaPolicy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains methods to manage CSLA permission policy information</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Blazor
{
  /// <summary>
  /// Contains methods to manage CSLA permission policy information.
  /// </summary>
  public static class CslaPolicy
  {
    private const string PolicyPrefix = "Csla:";

    /// <summary>
    /// Gets a string representing a CSLA permissions policy
    /// </summary>
    /// <param name="action">Authorization action</param>
    /// <param name="objectType">Business object type</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static string GetPolicy(Rules.AuthorizationActions action, Type objectType)
    {
      ArgumentNullException.ThrowIfNull(objectType);

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
    /// <exception cref="ArgumentNullException"><paramref name="policy"/> is <see langword="null"/>.</exception>
    public static bool TryGetPermissionRequirement(string policy, [NotNullWhen(true)] out CslaPermissionRequirement? requirement)
    {
      ArgumentNullException.ThrowIfNull(policy);

      if (policy.StartsWith(PolicyPrefix))
      {
        var parts = policy.Substring(PolicyPrefix.Length).Split('|');
        if (parts.Length < 2)
        {
          requirement = null;
          return false;
        }

        var action = (Rules.AuthorizationActions)Enum.Parse(typeof(Rules.AuthorizationActions), parts[0]);
        var type = Type.GetType(parts[1]);
        if (type is null)
        {
          requirement = null;
          return false;
        }

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
