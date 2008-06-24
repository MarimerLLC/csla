using System;

namespace Csla.DataPortalClient
{
  internal interface IDataPortalTarget
  {
    void MarkAsChild();
    void MarkNew();
    void MarkOld();
  }
}
