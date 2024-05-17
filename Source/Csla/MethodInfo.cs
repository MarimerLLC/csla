﻿//-----------------------------------------------------------------------
// <copyright file="MethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a method.</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a method.
  /// </summary>
  public class MethodInfo : Core.IMemberInfo
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    public MethodInfo(string name)
    {
      Name = name;
    }

    /// <summary>
    /// Gets the member name value.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines the equality of two objects.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    public override bool Equals(object obj)
    {
      if (obj is MethodInfo other)
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