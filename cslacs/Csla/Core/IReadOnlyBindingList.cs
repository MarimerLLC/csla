using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  internal interface IReadOnlyBindingList
  {
    bool IsReadOnly { get; set; }
  }
}
