using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public class GenRoot : GenRootBase
    {
      private string _data;

        private GenRoot()
        {
            //prevent direct creation
        }

    }
}
