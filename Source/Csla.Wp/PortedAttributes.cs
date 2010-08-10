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
  internal class BrowsableAttribute : Attribute
  {
    public BrowsableAttribute(bool flag)
    { }
  }

  internal class DisplayAttribute : Attribute
  {
    public string Name { get; set; }
    public bool AutoGenerateField { get; set; }
    public DisplayAttribute(bool AutoGenerateField = false, string Name = "")
    {
      this.AutoGenerateField = AutoGenerateField;
      this.Name = Name;
    }
  }
}
