using System;
using Csla;

namespace ProjectTracker.Library
{
  // This should fail because it doesn't have a no-arg ctor
  [Serializable]
  public class ResourceExistsCommand : CommandBase<ResourceExistsCommand>
  {
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

    protected override void DataPortal_Execute()
    {
    }
  }

  [Serializable]
  public class Data
    : BusinessBase<Data>
  {
    public readonly static PropertyInfo<string> MyTextProperty = 
      RegisterProperty<string>(c => c.MyText);

    //public string MyText { get; set; }
    public string MyText
    {
      get { return this.GetProperty(MyTextProperty); }
      set { this.SetProperty(MyTextProperty, value); }
    }
  }
}