using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Linq
{
  internal interface IBalancedSearch<T>
  {
    IEnumerable<T> ItemsLessThan(object pivot);
    IEnumerable<T> ItemsGreaterThan(object pivot);
    IEnumerable<T> ItemsLessThanOrEqualTo(object pivot);
    IEnumerable<T> ItemsGreaterThanOrEqualTo(object pivot);
    IEnumerable<T> ItemsEqualTo(object pivot);
  }
}
