using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.DataPortalTest
{
  [Serializable]
  class SplitOverload : SplitOverloadBase<SplitOverload>
  {
  }
}
