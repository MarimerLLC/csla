using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  interface IPositionMappable<T>
  {
    int PositionOf(T item);
  }
}
