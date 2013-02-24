using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectTracker.Library;
using System.Web.Mvc;

namespace Mvc3UI.ViewModels
{
  public class ResourceViewModel : Csla.Web.Mvc.ViewModelBase<ResourceEdit>
  {
    public ResourceViewModel()
    {
      ModelObject = ResourceEdit.NewResourceEdit();
    }

    public ResourceViewModel(int resourceId)
    {
      ModelObject = ResourceEdit.GetResourceEdit(resourceId);
    }
  }
}