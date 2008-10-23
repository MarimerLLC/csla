using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Linq
{
  public interface IRangeTestableIndex<T> : IIndex<T>
  {
    IEnumerable<T> WhereLessThan(object pivotVal);
    IEnumerable<T> WhereGreaterThan(object pivotVal);
    IEnumerable<T> WhereLessThanOrEqualTo(object pivotVal);
    IEnumerable<T> WhereGreaterThanOrEqualTo(object pivotVal);
  }
}
