//-----------------------------------------------------------------------
// <copyright file="PortedAttributes.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Dummy implementations of .NET attributes missing in WP7.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// Mock attribute ported from .NET/SL.
  /// </summary>
  public class BrowsableAttribute : Attribute
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="flag">Flag value.</param>
    public BrowsableAttribute(bool flag)
    { }
  }

  /// <summary>
  /// Defines a user-friendly display name for the property.
  /// </summary>
  public class DisplayAttribute : Attribute
  {
    private string _name;
    /// <summary>
    /// User-friendly property name.
    /// </summary>
    public string Name 
    {
      get { return _name; }
      set { _name = value; }
    }
    /// <summary>
    /// User-friendly property name.
    /// </summary>
    public string Description
    {
      get { return _name; }
      set { _name = value; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to
    /// autogenerate the field.
    /// </summary>
    public bool AutoGenerateField { get; set; }
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="AutoGenerateField">AutoGenerateField value.</param>
    /// <param name="Name">Friendly name.</param>
    public DisplayAttribute(bool AutoGenerateField = false, string Name = "")
    {
      this.AutoGenerateField = AutoGenerateField;
      this.Name = Name;
    }
  }
}
