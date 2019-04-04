//-----------------------------------------------------------------------
// <copyright file="MethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a method.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a method.
  /// </summary>
  public class MethodInfo : Csla.Core.IMemberInfo
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    public MethodInfo(string name)
    {
      Name = name;
    }

    /// <summary>
    /// Gets the member name value.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Determines the equality of two objects.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    public override bool Equals(object obj)
    {
      var other = obj as MethodInfo;
      if (other != null)
        return Name == other.Name;
      else
        return false;
    }

    /// <summary>
    /// Gets the hash code of this object.
    /// </summary>
    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }
}