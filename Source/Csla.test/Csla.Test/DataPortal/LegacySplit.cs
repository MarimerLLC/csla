using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortalTest
{
  [Serializable]
  class LegacySplit : LegacySplitBase<LegacySplit>
  {
    private LegacySplit()
    { /* Require use of factory methods */ }
  }
}
