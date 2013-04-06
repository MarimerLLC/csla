using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core.FieldManager
{
  internal class PropertyInfoList : List<IPropertyInfo>
  {
    public bool IsLocked { get; set; }

    public PropertyInfoList()
    { }

    public PropertyInfoList(IList<IPropertyInfo> list)
      : base(list)
    { }
  }
}
