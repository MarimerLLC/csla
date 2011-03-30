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

    public static PropertyInfo<bool> ProjectExistsProperty = RegisterProperty<bool>(c => c.ProjectExists);
    public bool ProjectExists
    {
      get { return ReadProperty(ProjectExistsProperty); }
      private set { LoadProperty(ProjectExistsProperty, value); }
    }

    public ProjectExistsCommand(int id)
    {
      Id = id;
    }

    protected override void DataPortal_Execute()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IProjectDal>();
        ProjectExists = dal.Exists(Id);
      }
    }
  }
}
