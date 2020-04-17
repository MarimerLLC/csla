using System;
using Csla;
using ProjectTracker.Dal;

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

    public static readonly PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(c => c.ResourceId);
    public int ResourceId
    {
      get { return ReadProperty(ResourceIdProperty); }
      private set { LoadProperty(ResourceIdProperty, value); }
    }

    public static readonly PropertyInfo<bool> ResourceExistsProperty = RegisterProperty<bool>(c => c.ResourceExists);
    public bool ResourceExists
    {
      get { return ReadProperty(ResourceExistsProperty); }
      private set { LoadProperty(ResourceExistsProperty, value); }
    }

    [Execute]
    private void Execute([Inject] IResourceDal dal)
    {
      ResourceExists = dal.Exists(ResourceId);
    }
  }
}
