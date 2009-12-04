using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Web.Mvc
{
  public interface IModelCreator
  {
    object CreateModel(Type modelType);
  }
}
