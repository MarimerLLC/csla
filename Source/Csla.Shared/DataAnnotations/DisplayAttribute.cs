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
    /// <summary>
    /// User-friendly property name.
    /// </summary>
    public string Name { get; set; }

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
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to
    /// autogenerate the field.
    /// </summary>
    public bool AutoGenerateField { get; set; }
    /// <summary>
    /// Gets or sets a value that indicates whether filtering UI is automatically displayed
    /// for this field.
    /// </summary>
    public bool AutoGenerateFilter { get; set; }
    /// <summary>
    /// Gets or sets a value that is used to group fields in the UI.
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// Gets or sets the order weight of the column.
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// Gets or sets a value that will be used to set the watermark for prompts in the UI.
    /// </summary>
    public string Prompt { get; set; }
    /// <summary>
    /// Gets or sets the type that contains the resources for the System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName,
    /// System.ComponentModel.DataAnnotations.DisplayAttribute.Name, System.ComponentModel.DataAnnotations.DisplayAttribute.Prompt,
    /// and System.ComponentModel.DataAnnotations.DisplayAttribute.Description properties.
    /// </summary>
    public Type ResourceType { get; set; }
    /// <summary>
    /// Gets or sets a value that is used for the grid column label.
    /// </summary>
    public string ShortName { get; set; }
  }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute))]
#endif