using System;
using System.Linq;
using Csla;
using Csla.Data;
using Rolodex.Business.Data;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class DuplicateCompanyCommand : CommandBase<DuplicateCompanyCommand>
  {
    public static readonly PropertyInfo<int> CompanyIDProperty = RegisterProperty(typeof(DuplicateCompanyCommand),
      new PropertyInfo<int>("CompanyID"));

    public int CompanyID
    {
      get { return ReadProperty(CompanyIDProperty); }
      set { LoadProperty(CompanyIDProperty, value); }
    }

    public static readonly PropertyInfo<string> CompanyNameProperty = RegisterProperty(typeof(DuplicateCompanyCommand),
      new PropertyInfo<string>("CompanyName"));

    public string CompanyName
    {
      get { return ReadProperty(CompanyNameProperty); }
      set { LoadProperty(CompanyNameProperty, value); }
    }

    public static readonly PropertyInfo<bool> IsDuplicateProperty = RegisterProperty(typeof(DuplicateCompanyCommand),
      new PropertyInfo<bool>("IsDuplicate"));

    public bool IsDuplicate
    {
      get { return ReadProperty(IsDuplicateProperty); }
    }

    public static DuplicateCompanyCommand Execute(int companyID, string companyName)
    {
      var cmd = new DuplicateCompanyCommand();
      cmd.CompanyID = companyID;
      cmd.CompanyName = companyName;

      cmd = DataPortal.Execute<DuplicateCompanyCommand>(cmd);
      return cmd;
    }

    public static void BeginExecute(int companyID, string companyName,
      EventHandler<DataPortalResult<DuplicateCompanyCommand>> callback)
    {
      var cmd = new DuplicateCompanyCommand();
      cmd.CompanyID = companyID;
      cmd.CompanyName = companyName;

      DataPortal.BeginExecute<DuplicateCompanyCommand>(cmd, callback);
    }

    protected override void DataPortal_Execute()
    {
      using (var manager =
        ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var companyId = ReadProperty(CompanyIDProperty);
        var companyName = ReadProperty(CompanyNameProperty);
        var company = (from oneCompany in manager.ObjectContext.Companies
          where
          oneCompany.CompanyId != companyId &&
          oneCompany.CompanyName == companyName
          select oneCompany).FirstOrDefault();
        if (company != null)
          LoadProperty(IsDuplicateProperty, true);
        else
          LoadProperty(IsDuplicateProperty, false);
      }
    }
  }
}