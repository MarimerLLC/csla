using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Security;
using Csla.Data;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceEdit : BusinessBase<ResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty = 
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    [Required]
    [StringLength(50)]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty = 
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    [Required]
    [StringLength(50)]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    [Display(Name = "Full name")]
    public string FullName
    {
      get { return LastName + ", " + FirstName; }
    }

    public static readonly PropertyInfo<ResourceAssignments> AssignmentsProperty =
      RegisterProperty<ResourceAssignments>(c => c.Assignments);
    public ResourceAssignments Assignments
    {
      get
      {
        if (!(FieldManager.FieldExists(AssignmentsProperty)))
          LoadProperty(AssignmentsProperty, DataPortal.CreateChild<ResourceAssignments>());
        return GetProperty(AssignmentsProperty);
      }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, LastNameProperty, "ProjectManager"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, FirstNameProperty, "ProjectManager"));
    }

    protected static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(ResourceEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "ProjectManager", "Administrator"));
    }

    public static void GetResource(int id, EventHandler<DataPortalResult<ResourceEdit>> callback)
    {
      DataPortal.BeginFetch<ResourceEdit>(id, callback);
    }

    public static void Exists(int id, Action<bool> result)
    {
      var cmd = new ResourceExistsCommand(id);
      DataPortal.BeginExecute<ResourceExistsCommand>(cmd, (o, e) =>
      {
        if (e.Error != null)
          throw e.Error;
        else
          result(e.Object.ResourceExists);
      });
    }

#if SILVERLIGHT
    public static void NewResource(EventHandler<DataPortalResult<ResourceEdit>> callback)
    {
      DataPortal.BeginCreate<ResourceEdit>(callback, DataPortal.ProxyModes.LocalOnly);
    }
#else
    public static void NewResource(EventHandler<DataPortalResult<ResourceEdit>> callback)
    {
      DataPortal.BeginCreate<ResourceEdit>(callback);
    }

    public static ResourceEdit NewResource()
    {
      return DataPortal.Create<ResourceEdit>();
    }

    public static ResourceEdit GetResource(int id)
    {
      return DataPortal.Fetch<ResourceEdit>(id);
    }

    public static void DeleteResource(int id)
    {
      DataPortal.Delete<ResourceEdit>(id);
    }

    public static bool Exists(int id)
    {
      var cmd = new ResourceExistsCommand(id);
      cmd = DataPortal.Execute<ResourceExistsCommand>(cmd);
      return cmd.ResourceExists;
    }

    [RunLocal]
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(int id)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        var data = dal.Fetch(id);
        using (BypassPropertyChecks)
        {
          Id = data.Id;
          FirstName = data.FirstName;
          LastName = data.LastName;
          TimeStamp = data.LastChanged;
        }
      }
    }

    protected override void DataPortal_Insert()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        using (BypassPropertyChecks)
        {
          var item = new ProjectTracker.Dal.ResourceDto
          {
            FirstName = this.FirstName,
            LastName = this.LastName
          };
          dal.Insert(item);
          Id = item.Id;
          TimeStamp = item.LastChanged;
        }
      }
    }

    protected override void DataPortal_Update()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        using (BypassPropertyChecks)
        {
          var item = new ProjectTracker.Dal.ResourceDto
          {
            Id = this.Id,
            FirstName = this.FirstName,
            LastName = this.LastName,
            LastChanged = this.TimeStamp
          };
          dal.Update(item);
          TimeStamp = item.LastChanged;
        }
      }
    }

    protected override void DataPortal_DeleteSelf()
    {
      using (BypassPropertyChecks)
        DataPortal_Delete(this.Id);
    }

    private void DataPortal_Delete(int id)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IResourceDal>();
        dal.Delete(id);
      }
    }
#endif
  }
}