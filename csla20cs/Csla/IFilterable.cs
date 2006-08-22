using System;
using System.Collections.Generic;
using System.Text;

namespace Csla
{
  interface IFilterable
  {
    bool MatchesFilter(string filter);
    bool MatchesFilter(FilterProvider filter, object state);
  }
}
