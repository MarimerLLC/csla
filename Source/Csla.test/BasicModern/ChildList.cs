using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Serialization;

namespace Csla.Test.BasicModern
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
  }
}
