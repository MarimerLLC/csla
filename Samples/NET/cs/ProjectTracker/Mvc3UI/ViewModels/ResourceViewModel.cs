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
      ModelObject = ResourceEdit.NewResource();
    }

    public ResourceViewModel(int resourceId)
    {
      ModelObject = ResourceEdit.GetResource(resourceId);
    }

    public bool Save(ModelStateDictionary modelState, bool forceUpdate)
    {
      try
      {
        ModelObject = ModelObject.Save(forceUpdate);
        return true;
      }
      catch (Csla.DataPortalException ex)
      {
        if (ex.BusinessException != null)
          modelState.AddModelError("", ex.BusinessException.Message);
        else
          modelState.AddModelError("", ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        modelState.AddModelError("", ex.Message);
        return false;
      }
    }
  }
}