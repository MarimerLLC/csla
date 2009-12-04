//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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

namespace WcfService.Business.Client
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
		string[] canWrite = new string[] {"AdminUser", "RegularUser"};
		string[] canRead = new string[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
		string[] admin = new string[] {"AdminUser"};
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
		if (! dateAdded.IsEmpty)
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
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.GetCompanyCompleted += EndFetch;
		  client.GetCompanyAsync(criteria.Value);
		}
		catch (Exception ex)
		{
		  _handler(null, ex);
		}

	  }

	  private void EndFetch(object sender, CompanyServiceReference.GetCompanyCompletedEventArgs e)
	  {
		try
		{
		  CompanyServiceReference.CompanyInfo company = e.Result;
		  if (company != null)
		  {
			LoadProperty(CompanyIdProperty, company.CompanyId);
			LoadProperty(CompanyNameProperty, company.CompanyName);
			LoadProperty(DateAddedProperty, new SmartDate(company.DateAdded));
			_handler(this, null);
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
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.GetCompanyCompleted -= EndFetch;
		}

	  }

	  [EditorBrowsable(EditorBrowsableState.Never)]
	  public new void DataPortal_DeleteSelf(LocalProxy<Company>.CompletedHandler handler)
	  {
		if (! IsNew)
		{
		  _handler = handler;
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.DeleteCompanyCompleted += EndDelete;
		  client.DeleteCompanyAsync(ReadProperty(CompanyIdProperty));
		}
		else
		{
		  _handler(this, null);
		}

	  }

	  private void EndDelete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
	  {
		try
		{
		  if (e.Error == null)
		  {
			_handler(this, null);
		  }
		  else
		  {
			_handler(this, e.Error);
		  }

		}
		catch (Exception ex)
		{
		  _handler(null, ex);
		}
		finally
		{
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.DeleteCompanyCompleted -= EndDelete;
		}

	  }

	  [EditorBrowsable(EditorBrowsableState.Never)]
	  public new void DataPortal_Insert(LocalProxy<Company>.CompletedHandler handler)
	  {
		try
		{
		  _handler = handler;
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  CompanyServiceReference.CompanyInfo newCompany = new CompanyServiceReference.CompanyInfo();
		newCompany.CompanyId = ReadProperty(CompanyIdProperty);
		newCompany.CompanyName = ReadProperty(CompanyNameProperty);
		newCompany.DateAdded = ReadProperty(DateAddedProperty).Text;
		  client.InsertCompanyCompleted += EndInsert;
		  client.InsertCompanyAsync(newCompany);
		}
		catch (Exception ex)
		{
		  _handler(this, ex);
		}

	  }

	  private void EndInsert(object sender, CompanyServiceReference.InsertCompanyCompletedEventArgs e)
	  {
		try
		{
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  if (e.Error == null)
		  {
			int newId = e.Result;
			if (newId > 0)
			{
			  LoadProperty(CompanyIdProperty, newId);
			  MarkOld();
			  _handler(this, null);
			}
			else
			{
			  _handler(this, new ArgumentException("Cannot insert company."));
			}
		  }
		  else
		  {
			_handler(this, e.Error);
		  }

		}
		catch (Exception ex)
		{
		  _handler(null, ex);
		}
		finally
		{
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.InsertCompanyCompleted -= EndInsert;
		}

	  }

	  [EditorBrowsable(EditorBrowsableState.Never)]
	  public new void DataPortal_Update(LocalProxy<Company>.CompletedHandler handler)
	  {

		try
		{
		  _handler = handler;
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  CompanyServiceReference.CompanyInfo existingCompany = new CompanyServiceReference.CompanyInfo();
		existingCompany.CompanyId = ReadProperty(CompanyIdProperty);
		existingCompany.CompanyName = ReadProperty(CompanyNameProperty);
		existingCompany.DateAdded = ReadProperty(DateAddedProperty).Text;
		  client.UpdateCompanyCompleted += EndUpdate;
		  client.UpdateCompanyAsync(existingCompany);
		}
		catch (Exception ex)
		{
		  _handler(this, ex);

		}

	  }

	  private void EndUpdate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
	  {
		try
		{
		  if (e.Error == null)
		  {
			MarkOld();
			_handler(this, null);
		  }
		  else
		  {
			_handler(this, e.Error);
		  }

		}
		catch (Exception ex)
		{
		  _handler(null, ex);
		}
		finally
		{
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.UpdateCompanyCompleted -= EndUpdate;
		}

	  }

#endif

  }

} //end of root namespace