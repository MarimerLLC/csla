using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Csla.Linq
{
  internal interface IIndexSearchable<T>
  {
    IEnumerable<T> SearchByExpression(Expression<Func<T, bool>> expr);
  }
}
