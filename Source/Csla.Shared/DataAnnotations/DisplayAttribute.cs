#if NETFX_PHONE
//-----------------------------------------------------------------------
// <copyright file="DisplayAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines a user-friendly display name for the property.</summary>
//-----------------------------------------------------------------------
using System;

namespace System.ComponentModel.DataAnnotations
{
  /// <summary>
  /// Defines a user-friendly display name for the property.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
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
    /// Gets the Name property value.
    /// </summary>
    public string GetName()
    {
      return Name;
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
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute))]
#endif