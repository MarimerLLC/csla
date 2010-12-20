using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ResourceAssignment : BusinessBase<ResourceAssignment>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<Guid> ProjectIdProperty =
      RegisterProperty<Guid>(c => c.ProjectId, null, Guid.NewGuid());
    [Display(Name = "Project id")]
    public Guid ProjectId
    {
      get { return GetProperty(ProjectIdProperty); }
      private set { SetProperty(ProjectIdProperty, value); }
    }

    public static readonly PropertyInfo<string> ProjectNameProperty = 
      RegisterProperty<string>(c => c.ProjectName);
    [Display(Name = "Project name")]
    public string ProjectName
    {
      get { return GetProperty(ProjectNameProperty); }
      private set { SetProperty(ProjectNameProperty, value); }
    }

    public static readonly PropertyInfo<SmartDate> AssignedProperty = 
      RegisterProperty<SmartDate>(c => c.Assigned);
    public string Assigned
    {
      get { return GetPropertyConvert<SmartDate, string>(AssignedProperty); }
    }

    public static readonly PropertyInfo<int> RoleProperty = RegisterProperty<int>(c => c.Role);
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    public override string ToString()
    {
      return ProjectId.ToString();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Assignment.ValidRole(RoleProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, "ProjectManager"));
    }
  }
}