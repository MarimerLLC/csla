using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Server
{
  public interface IDataPortalServer
  {
    DataPortalResult Create(Type objectType, object criteria, DataPortalContext context);
    DataPortalResult Fetch(object criteria, DataPortalContext context);
    DataPortalResult Update(object obj, DataPortalContext context);
    DataPortalResult Delete(object criteria, DataPortalContext context);
  }
}
