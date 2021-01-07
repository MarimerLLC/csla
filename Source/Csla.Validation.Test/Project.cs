using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Security;

namespace Csla.Validation.Test
{
  [Serializable()]
  public class Project : Csla.Validation.BusinessBase<Project>
  {
    #region  Business Methods

    private byte[] _timestamp = new byte[8];

    private static PropertyInfo<Guid> IdProperty = RegisterProperty(typeof (Project), new PropertyInfo<Guid>("Id"));

    [System.ComponentModel.DataObjectField(true, true)]
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
    }

    private static PropertyInfo<string> NameProperty =
      RegisterProperty(typeof (Project), new PropertyInfo<string>("Name", "Project name"));

    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<SmartDate> StartedProperty =
      RegisterProperty(typeof (Project), new PropertyInfo<SmartDate>("Started"));

    public string Started
    {
      get { return GetPropertyConvert<SmartDate, string>(StartedProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartedProperty, value); }
    }

    private static PropertyInfo<SmartDate> EndedProperty =
      RegisterProperty(typeof (Project),
                       new PropertyInfo<SmartDate>("Ended", new SmartDate(SmartDate.EmptyValue.MaxDate)));

    public string Ended
    {
      get { return GetPropertyConvert<SmartDate, string>(EndedProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndedProperty, value); }
    }

    private static PropertyInfo<string> DescriptionProperty =
      RegisterProperty(typeof (Project), new PropertyInfo<string>("Description"));

    public string Description
    {
      get { return GetProperty(DescriptionProperty); }
      set { SetProperty(DescriptionProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    #endregion

    #region  Validation Rules

    protected override void AddBusinessRules()
    {
      AuthorizationRules.AllowWrite(NameProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(StartedProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(EndedProperty, "ProjectManager");
      AuthorizationRules.AllowWrite(DescriptionProperty, "Administrator");

      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired,
                              new Csla.Validation.RuleArgs(NameProperty));
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        new Csla.Validation.CommonRules.MaxLengthRuleArgs(NameProperty, 50));

      var args = new Csla.Validation.DecoratedRuleArgs(NameProperty);
      args["MaxLength"] = 50;
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength,
        args);


      ValidationRules.AddRule<Project>(StartDateGTEndDate<Project>, StartedProperty);
      ValidationRules.AddRule<Project>(StartDateGTEndDate<Project>, EndedProperty);

      ValidationRules.AddDependentProperty(StartedProperty, EndedProperty, true);
    }

    private static bool StartDateGTEndDate<T>(T target, Csla.Validation.RuleArgs e) where T : Project
    {
      if (target.ReadProperty(StartedProperty) > target.ReadProperty(EndedProperty))
      {
        e.Description = "Start date can't be after end date";
        return false;
      }
      else
      {
        return true;
      }
    }

    #endregion

    #region  Authorization Rules

    protected static void AddObjectAuthorizationRules()
    {
      // add object-level authorization rules here
      Csla.Security.AuthorizationRules.AllowCreate(typeof (Project), "ProjectManager");
      Csla.Security.AuthorizationRules.AllowGet(typeof(Project), "Administrator");
      Csla.Security.AuthorizationRules.AllowEdit(typeof (Project), "ProjectManager");
      Csla.Security.AuthorizationRules.AllowDelete(typeof(Project), "ProjectManager", "Administrator");
    }

    #endregion

    #region  Factory Methods

    public static Project NewProject()
    {
      return DataPortal.Create<Project>();
    }

    public static Project GetProject(Guid id)
    {
      return DataPortal.Fetch<Project>(id);
    }

    public static void DeleteProject(Guid id)
    {
      DataPortal.Delete<Project>(id);
    }

    #endregion

    protected override void DataPortal_Create()
    {
      
    }

    protected void DataPortal_Fetch(Guid criteria)
    {
      
    }

    protected void DataPortal_Delete(Guid criteria)
    {

    }
  }
}
