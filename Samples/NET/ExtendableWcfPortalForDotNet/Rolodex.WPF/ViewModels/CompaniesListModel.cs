using System;
using Csla.Rules;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Rolodex.Business.BusinessClasses;
using Rolodex.Silverlight.Core;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.ViewModels
{
  public class CompaniesListModel : ViewModel<ReadOnlyCompanyList, CompaniesListView>
  {
    private IEventAggregator _eventAggregator;

    public CompaniesListModel(IEventAggregator eventAggregator) :
      base(eventAggregator)
    {
      EditCompanyCommand = new DelegateCommand<ReadOnlyCompany>(EditCompany, CanEditCompany);
      AddCompanyCommand = new DelegateCommand<object>(AddCompany, CanAddCompany);
      View = new CompaniesListView();
      BeginRefresh("GetCompanyList");
      _eventAggregator = eventAggregator;
    }

    public DelegateCommand<ReadOnlyCompany> EditCompanyCommand { get; private set; }

    private void EditCompany(ReadOnlyCompany parameter)
    {
      _eventAggregator.GetEvent<EditCompanyEvent>().Publish(new EditCompanyEventArgs(parameter.CompanyId));
    }

    private bool CanEditCompany(ReadOnlyCompany parameter)
    {
      if (parameter == null)
      {
        return false;
      }
      return true;
    }

    public DelegateCommand<object> AddCompanyCommand { get; private set; }

    private void AddCompany(object parameter)
    {
      _eventAggregator.GetEvent<AddCompanyEvent>().Publish(EventArgs.Empty);
    }

    private bool CanAddCompany(object parameter)
    {
      if (BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(Company)))
        return true;

      return false;
    }
  }
}