using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class ListContainerList : BusinessListBase<ListContainerList, ContainsList>
  {
  }
}
