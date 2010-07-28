using System;

namespace Csla
{
  internal class BrowsableAttribute : Attribute
  {
    public BrowsableAttribute(bool flag)
    { }
  }

  internal class DisplayAttribute : Attribute
  { }

  internal class DataMemberAttribute : Attribute
  { }

  internal class DataContractAttribute : Attribute
  { }
}
