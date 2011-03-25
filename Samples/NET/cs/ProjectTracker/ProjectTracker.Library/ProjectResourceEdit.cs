using Csla;
using System.ComponentModel.DataAnnotations;
using System;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectResourceEdit : BusinessBase<ProjectResourceEdit>
  {
    public static readonly PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    private byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    public static readonly PropertyInfo<int> ResourceIdProperty = 
      RegisterProperty<int>(c => c.ResourceId);
    [Display(Name = "Resource id")]
    public int ResourceId
    {
      get { return GetProperty(ResourceIdProperty); }
      private set { LoadProperty(ResourceIdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty =
      RegisterProperty<string>(c => c.FirstName);
    [Display(Name = "First name")]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      private set { LoadProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty =
      RegisterProperty<string>(c => c.LastName);
    [Display(Name = "Last name")]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      private set { LoadProperty(LastNameProperty, value); }
    }

    public string FullName
    {
      get { return string.Format("{0}, {1}", LastName, FirstName); }
    }

    public static readonly PropertyInfo<SmartDate> AssignedProperty =
      RegisterProperty<SmartDate>(c => c.Assigned);
    [Display(Name = "Date assigned")]
    public string Assigned
    {
      get { return GetPropertyConvert<SmartDate, string>(AssignedProperty); }
      private set { LoadPropertyConvert<SmartDate, string>(AssignedProperty, value); }
    }

    public static readonly PropertyInfo<int> RoleProperty = 
      RegisterProperty<int>(c => c.Role);
    [Display(Name = "Role assigned")]
    public int Role
    {
      get { return GetProperty(RoleProperty); }
      set { SetProperty(RoleProperty, value); }
    }

    public override string ToString()
    {
      return ResourceId.ToString();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Assignment.ValidRole(RoleProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, RoleProperty, "ProjectManager"));
    }
  }
}