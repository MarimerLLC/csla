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
