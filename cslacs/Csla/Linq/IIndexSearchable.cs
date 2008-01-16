using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Csla.Linq
{
  internal interface IIndexSearchable<T>
  {
    IEnumerable<T> SearchByExpression(Expression<Func<T, bool>> expr);
  }
}
