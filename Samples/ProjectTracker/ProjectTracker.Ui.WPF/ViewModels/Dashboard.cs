using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace WpfUI.ViewModels
{
  public class Dashboard : ViewModel<ProjectTracker.Library.Dashboard>
  {
    public Dashboard()
    {
      RefreshAsync<ProjectTracker.Library.Dashboard>(
        async () => await DataPortal.CreateAsync<ProjectTracker.Library.Dashboard>());
    }
  }
}
