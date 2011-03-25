using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectExistsCommand : CommandBase<ProjectExistsCommand>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    private int Id
    {
      get { return ReadProperty(IdProperty); }
      set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<bool> ResultProperty = RegisterProperty<bool>(c => c.ProjectExists);
    public bool ProjectExists
    {
      get { return ReadProperty(ResultProperty); }
      private set { LoadProperty(ResultProperty, value); }
    }

    public ProjectExistsCommand(int id)
    {
      Id = id;
    }
  }
}
