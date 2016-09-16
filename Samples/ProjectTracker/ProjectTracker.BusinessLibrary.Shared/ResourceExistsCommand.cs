using System;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceExistsCommand : CommandBase<ResourceExistsCommand>
  {
    public ResourceExistsCommand()
    { }

#pragma warning disable CSLA0004
    public ResourceExistsCommand(int id)
    {
      ResourceId = id;
    }

    public static PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(c => c.ResourceId);
    public int ResourceId
    {
      get { return ReadProperty(ResourceIdProperty); }
      private set { LoadProperty(ResourceIdProperty, value); }
    }

    public static PropertyInfo<bool> ResourceExistsProperty = RegisterProperty<bool>(c => c.ResourceExists);
    public bool ResourceExists
    {
      get { return ReadProperty(ResourceExistsProperty); }
      private set { LoadProperty(ResourceExistsProperty, value); }
    }

#if FULL_DOTNET
    protected override void DataPortal_Execute()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        ResourceExists = dal.Exists(ResourceId);
      }
    }
#endif
  }
}
