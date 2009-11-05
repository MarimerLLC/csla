//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;
using Csla.Validation;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif

namespace DataServices.Business
{
  [Serializable()]
  public class Company : BusinessBase<Company>
  {


#if SILVERLIGHT
    public Company()
    {
    }
#else
    private Company()
    {
    }
#endif

    public static Uri GetServiceUri()
    {
      return new Uri("CompanyService.svc", UriKind.Relative);
    }


    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
    }

    private static PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));
    public string CompanyName
    {
      get
      {
        return GetProperty(CompanyNameProperty);
      }
      set
      {
        SetProperty(CompanyNameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> DateAddedProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("DateAdded", "Date Added"));
    public string DateAdded
    {
      get
      {
        return GetProperty(DateAddedProperty).Text;
      }
      set
      {
        SmartDate test = new SmartDate();
        if (SmartDate.TryParse(value, ref test))
        {
          SetProperty(DateAddedProperty, test);
        }
      }
    }

    internal object DateAddedValue
    {
      get
      {
        return GetProperty(DateAddedProperty).DBValue;
      }
    }

    protected override void AddAuthorizationRules()
    {
      base.AddAuthorizationRules();
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Company), admin);
      AuthorizationRules.AllowDelete(typeof(Company), admin);
      AuthorizationRules.AllowEdit(typeof(Company), canWrite);
      AuthorizationRules.AllowGet(typeof(Company), canRead);
      AuthorizationRules.AllowWrite(CompanyNameProperty, canWrite);
      AuthorizationRules.AllowWrite(DateAddedProperty, canWrite);
      AuthorizationRules.AllowRead(CompanyNameProperty, canRead);
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead);
      AuthorizationRules.AllowRead(DateAddedProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      ValidationRules.AddRule(CommonRules.StringRequired, new RuleArgs(CompanyNameProperty));
      ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(CompanyNameProperty, 50));
      ValidationRules.AddRule<Company>(IsDateValid, DateAddedProperty);

    }

    private static bool IsDateValid(Company target, RuleArgs e)
    {
      SmartDate dateAdded = target.GetProperty(DateAddedProperty);
      if (!dateAdded.IsEmpty)
      {
        if (dateAdded.Date < new System.DateTime(2000, 1, 1))
        {
          e.Description = "Date must be greater that 1/1/2000!";
          return false;
        }
        else if (dateAdded.Date > DateTime.Today)
        {
          e.Description = "Date cannot be greater than today!";
          return false;
        }
      }
      else
      {
        e.Description = "Date added is required!";
        return false;
      }
      return true;
    }

    internal void SetID(int companyId)
    {
      LoadProperty(CompanyIdProperty, companyId);
    }

    public static void GetCompany(int companyId, EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new SingleCriteria<Company, int>(companyId));
    }

    public static void CreateCompany(EventHandler<DataPortalResult<Company>> handler)
    {
      DataPortal<Company> dp = new DataPortal<Company>();
      dp.CreateCompleted += handler;
      dp.BeginCreate();
    }

#if SILVERLIGHT

    private LocalProxy<Company>.CompletedHandler _handler;
    private SingleCriteria<Company, int> _criteria;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void DataPortal_Fetch(SingleCriteria<Company, int> criteria, LocalProxy<Company>.CompletedHandler handler)
    {
      try
      {
        _handler = handler;
        _criteria = criteria;
        CompanyServiceReference.CompanyEntities context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri()).DataServiceContext;

        var query =
          from oneCompany in context.Companies
          where oneCompany.CompanyId == criteria.Value
          select oneCompany;
        global::System.Data.Services.Client.DataServiceQuery<CompanyServiceReference.Companies> companyQuery = (global::System.Data.Services.Client.DataServiceQuery<global::DataServices.Business.CompanyServiceReference.Companies>)query;
        companyQuery.BeginExecute(EndFetch, companyQuery);
      }
      catch (Exception ex)
      {
        _handler(null, ex);
        _handler = null;
        _criteria = null;
      }

    }

    private void EndFetch(IAsyncResult e)
    {
      try
      {
        var queryCompany = (System.Data.Services.Client.DataServiceQuery<CompanyServiceReference.Companies>)e.AsyncState;
        if (queryCompany != null)
        {
          var aCompany = queryCompany.EndExecute(e).FirstOrDefault();
          if (aCompany != null)
          {
            LoadProperty(CompanyIdProperty, aCompany.CompanyId);
            LoadProperty(CompanyNameProperty, aCompany.CompanyName);
            LoadProperty(DateAddedProperty, new SmartDate(aCompany.DateAdded));
            _handler(this, null);
          }
          else
          {
            _handler(null, new ArgumentException("Company with Id of " + _criteria.Value.ToString() + " is not found."));
          }
        }
        else
        {
          _handler(null, new ArgumentException("Company with Id of " + _criteria.Value.ToString() + " is not found."));
        }
      }
      catch (Exception ex)
      {
        _handler(null, ex);
      }
      finally
      {
        _handler = null;
        _criteria = null;
      }

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DataPortal_DeleteSelf(LocalProxy<Company>.CompletedHandler handler)
    {
      if (!IsNew)
      {
        _handler = handler;
        CompanyServiceReference.CompanyEntities context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri()).DataServiceContext;
        CompanyServiceReference.Companies company = GetCompany();
        context.DeleteObject(company);
        context.BeginSaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch, EndUpdate, null);
      }
      else
      {
        _handler(this, null);
      }

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DataPortal_Insert(LocalProxy<Company>.CompletedHandler handler)
    {
      try
      {
        _handler = handler;
        CompanyServiceReference.CompanyEntities context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri()).DataServiceContext;
        CompanyServiceReference.Companies company = new CompanyServiceReference.Companies();
        company.CompanyId = ReadProperty(CompanyIdProperty);
        company.CompanyName = ReadProperty(CompanyNameProperty);
        company.DateAdded = ReadProperty(DateAddedProperty);
        context.AddToCompanies(company);
        context.BeginSaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch, EndUpdate, null);
      }
      catch (Exception ex)
      {
        _handler(this, ex);
      }

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DataPortal_Update(LocalProxy<Company>.CompletedHandler handler)
    {

      try
      {
        if (IsDirty)
        {
          _handler = handler;
          CompanyServiceReference.CompanyEntities context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri()).DataServiceContext;
          CompanyServiceReference.Companies company = GetCompany();
          company.CompanyId = ReadProperty(CompanyIdProperty);
          company.CompanyName = ReadProperty(CompanyNameProperty);
          company.DateAdded = ReadProperty(DateAddedProperty);
          context.UpdateObject(company);
          context.BeginSaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch, EndUpdate, null);
        }
        else
        {
          _handler(this, null);
        }
      }
      catch (Exception ex)
      {
        _handler(this, ex);
      }

    }

    private void EndUpdate(IAsyncResult e)
    {
      try
      {
        CompanyServiceReference.CompanyEntities context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri()).DataServiceContext;
        System.Data.Services.Client.DataServiceResponse response = context.EndSaveChanges(e);
        Exception anError = null;
        foreach (System.Data.Services.Client.OperationResponse oneResponse in response)
        {
          if (oneResponse.Error != null)
          {
            anError = oneResponse.Error;
          }
          else
          {
            if (!IsDeleted)
            {
              System.Data.Services.Client.ChangeOperationResponse details = (System.Data.Services.Client.ChangeOperationResponse)oneResponse;
              CompanyServiceReference.Companies company = (CompanyServiceReference.Companies)(((System.Data.Services.Client.EntityDescriptor)details.Descriptor).Entity);
              LoadProperty(CompanyIdProperty, company.CompanyId);
            }
          }
        }
        if (anError == null)
        {
          if (!IsDeleted)
          {
            MarkOld();
          }
          _handler(this, null);
          _handler = null;
        }
        else
        {
          _handler(this, anError);
        }
      }
      catch (Exception ex)
      {
        _handler(this, ex);
      }

    }

    private CompanyServiceReference.Companies GetCompany()
    {
      CompanyServiceReference.Companies returnValue = null;
      var manager = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(GetServiceUri());
      returnValue = manager.GetEntity<CompanyServiceReference.Companies>(CompanyIdProperty.Name, ReadProperty(CompanyIdProperty));
      return returnValue;
    }

#endif

  }

} //end of root namespace