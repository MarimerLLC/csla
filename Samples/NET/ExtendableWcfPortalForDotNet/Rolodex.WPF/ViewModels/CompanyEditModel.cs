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
  public class CompanyEditModel : ViewModel<Company, CompanyEditView>
  {
    private int _companyID;
    private CompanyContact _selectedContact;
    private CompanyContactPhone _selectedContactPhone;

    public CompanyEditModel(IEventAggregator eventAggregator)
      : base(eventAggregator)
    {
      Initialize();
    }

    public CompanyEditModel(int companyID, IEventAggregator eventAggregator) :
      base(eventAggregator)
    {
      _companyID = companyID;
      Initialize();
    }

    private void Initialize()
    {
      CloseCommand = new DelegateCommand<object>(CloseMethod, CanCloseMethod);
      SelectContactCommand = new DelegateCommand<object>(SelectContactMethod, CanSelectContactMethod);
      RemoveContactCommand = new DelegateCommand<object>(RemoveContactMethod, CanRemoveContactMethod);
      AddContactCommand = new DelegateCommand<object>(AddContactMethod, CanAddContactMethod);
      RemoveContactPhoneCommand = new DelegateCommand<object>(RemoveContactPhoneMethod, CanRemoveContactPhoneMethod);
      AddContactPhoneCommand = new DelegateCommand<object>(AddContactPhoneMethod, CanAddContactPhoneMethod);
      SelectContactPhoneCommand = new DelegateCommand<object>(SelectContactPhoneMethod, CanSelectContactPhoneMethod);
      View = new CompanyEditView();
      SetSecondaryDataCounter(1);
      Ranks.GetRanks(GetDataLoadedHandler<Ranks>());
    }

    protected override void OnDataLoaded<T>(Csla.DataPortalResult<T> result)
    {
      if (typeof(Ranks) == typeof(T))
      {
        ((SecondaryModel) View.Resources["RanksModel"]).Model = result.Object;
      }
    }

    protected override void OnAllSecondaryDataLoaded()
    {
      if (_companyID == 0)
      {
        BeginRefresh("CreateCompany");
      }
      else
      {
        BeginRefresh("GetCompany", _companyID);
      }
    }

    public DelegateCommand<object> CloseCommand { get; private set; }

    private void CloseMethod(object parameter)
    {
      CurrentEventAggregator.GetEvent<CloseEditViewEvent>().Publish(EventArgs.Empty);
    }

    private bool CanCloseMethod(object parameter)
    {
      return !CanCancel;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
      base.OnPropertyChanged(propertyName);
      if (CloseCommand != null)
        CloseCommand.RaiseCanExecuteChanged();
    }

    public DelegateCommand<object> SelectContactCommand { get; private set; }

    private void SelectContactMethod(object parameter)
    {
      _selectedContact = parameter as CompanyContact;
      if (_selectedContact != null)
        View.ContactsPhonesGrid.ItemsSource = _selectedContact.ContactPhones;
      else
        View.ContactsPhonesGrid.ItemsSource = null;

      RemoveContactCommand.RaiseCanExecuteChanged();
      AddContactPhoneCommand.RaiseCanExecuteChanged();
      RemoveContactPhoneCommand.RaiseCanExecuteChanged();
    }

    private bool CanSelectContactMethod(object parameter)
    {
      return true;
    }

    public DelegateCommand<object> SelectContactPhoneCommand { get; private set; }

    private void SelectContactPhoneMethod(object parameter)
    {
      _selectedContactPhone = parameter as CompanyContactPhone;
      RemoveContactPhoneCommand.RaiseCanExecuteChanged();
    }

    private bool CanSelectContactPhoneMethod(object parameter)
    {
      return _selectedContact != null;
    }

    public DelegateCommand<object> RemoveContactCommand { get; private set; }

    private void RemoveContactMethod(object parameter)
    {
      if (_selectedContact != null)
      {
        Model.Contacts.Remove(_selectedContact);
        _selectedContact = null;
        AddContactPhoneCommand.RaiseCanExecuteChanged();
        RemoveContactPhoneCommand.RaiseCanExecuteChanged();

        // needed becuase SelectionChanged event is not raised by the grid 
        // properly
        if (Model.Contacts.Count > 0)
        {
          View.ContactsGrid.SelectedItem = null;
          View.ContactsGrid.SelectedItem = Model.Contacts[0];
        }
      }
    }

    private bool CanRemoveContactMethod(object parameter)
    {
      return _selectedContact != null &&
             BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(CompanyContact));
    }

    public DelegateCommand<object> AddContactCommand { get; private set; }

    private void AddContactMethod(object parameter)
    {
      Model.Contacts.AddNew();
    }

    private bool CanAddContactMethod(object parameter)
    {
      return BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(CompanyContact));
    }

    public DelegateCommand<object> RemoveContactPhoneCommand { get; private set; }

    private void RemoveContactPhoneMethod(object parameter)
    {
      if (_selectedContactPhone != null && _selectedContact != null)
      {
        _selectedContact.ContactPhones.Remove(_selectedContactPhone);
        _selectedContactPhone = null;

        // needed because SelectionChanged event is not raised by the grid 
        // properly
        if (_selectedContact != null && _selectedContact.ContactPhones.Count > 0)
        {
          View.ContactsPhonesGrid.SelectedItem = null;
          View.ContactsPhonesGrid.SelectedItem = _selectedContact.ContactPhones[0];
        }
        RemoveContactPhoneCommand.RaiseCanExecuteChanged();
      }
    }

    private bool CanRemoveContactPhoneMethod(object parameter)
    {
      return _selectedContactPhone != null &&
             BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(CompanyContactPhone));
    }

    public DelegateCommand<object> AddContactPhoneCommand { get; private set; }

    private void AddContactPhoneMethod(object parameter)
    {
      _selectedContact.ContactPhones.AddNew();
    }

    private bool CanAddContactPhoneMethod(object parameter)
    {
      return _selectedContact != null &&
             BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof(CompanyContactPhone));
    }

    protected override void OnAfterDeleteSave()
    {
      base.OnAfterDeleteSave();
      CloseMethod(null);
    }

    protected override void OnAfterCancelNew()
    {
      base.OnAfterCancelNew();
      CloseMethod(null);
    }
  }
}