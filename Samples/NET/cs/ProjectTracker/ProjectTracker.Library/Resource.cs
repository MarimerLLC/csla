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
  public class Resource : BusinessBase<Resource>
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
      Csla.Rules.BusinessRules.AddRule(typeof(Resource), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(Resource), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "ProjectManager"));
      Csla.Rules.BusinessRules.AddRule(typeof(Resource), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "ProjectManager", "Administrator"));
    }

#if !SILVERLIGHT
    public static Resource NewResource()
    {
      return DataPortal.Create<Resource>();
    }

    public static Resource GetResource(int id)
    {
      return DataPortal.Fetch<Resource>(id);
    }

    public static void DeleteResource(int id)
    {
      DataPortal.Delete<Resource>(id);
    }
#endif

    #region  Exists

#if !SILVERLIGHT
    public static bool Exists(int id)
    {
      var cmd = new ExistsCommand(id);
      cmd = DataPortal.Execute<ExistsCommand>(cmd);
      return cmd.ResourceExists;
    }
#endif

    public static void Exists(int id, Action<bool> result)
    {
      var cmd = new ExistsCommand(id);
      DataPortal.BeginExecute<ExistsCommand>(cmd, (o, e) =>
        {
          if (e.Error != null)
            throw e.Error;
          else
            result(e.Object.ResourceExists);
        });
    }

    [Serializable()]
    private class ExistsCommand : CommandBase<ExistsCommand>
    {
      public ExistsCommand(int id)
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
    }
    #endregion
  }
}