using System;
using System.Collections;
using System.Linq;
using System.Windows;
using Csla;
using Csla.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Events;

namespace Rolodex.Silverlight.ViewModels
{
    public abstract class RolodexViewModel<TModel> : Csla.Xaml.ViewModelBase<TModel>, IRolodexViewModel
        where TModel : class, ITrackStatus
    {

        #region Properties

        protected IEventAggregator Aggregator { get; private set; }
        protected IUnityContainer Container { get; private set; }
        protected IRegionManager RegionManager { get; private set; }


        private bool _savingAfterDelete;
        private bool _savingAtClose = false;

        protected bool SavingAfterDelete
        {
            get { return _savingAfterDelete; }
            private set { _savingAfterDelete = value; }
        }

        private bool _isBoundToList;

        public bool IsBoundToList
        {
            get
            {
                return _isBoundToList;
            }
            set
            {
                _isBoundToList = value;
                OnPropertyChanged("IsBoundToList");
            }
        }

        private object _selectedItem;

        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        _selectedItem = value;
                        OnPropertyChanged("SelectedItem");
                        OnPropertyChanged("HasSelectedItem");
                    });
            }
        }
        private int _selectedIndex = -1;
        public bool HasSelectedItem
        {
            get
            {
                return _selectedItem != null;
            }
        }

        public virtual bool IsDirty
        {
            get
            {
                if (Model != null)
                {
                    if (Model is ITrackStatus)
                    {
                        return (Model as ITrackStatus).IsDirty;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected override void OnModelChanged(TModel oldValue, TModel newValue)
        {
            base.OnModelChanged(oldValue, newValue);
            if (Model != null && Model is IEnumerable)
            {
                IsBoundToList = true;
            }
            else
            {
                IsBoundToList = false;
            }
            OnPropertyChanged("SelectedItem");
            OnPropertyChanged("HasSelectedItem");
        }

        #endregion

        #region Constructors


        ~RolodexViewModel()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Disposed of view model of type " + GetType().Name);
#endif
        }

        protected RolodexViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            Aggregator = eventAggregator;
            Container = unityContainer;
            RegionManager = regionManager;
            CreateCommands();
        }

        protected virtual void CreateCommands()
        {
            SaveCommand = new DelegateCommand<object>(SaveMethod, CanSaveMethod);
            DeleteCommand = new DelegateCommand<object>(DeleteMethod, CanDeleteMethod);
            CancelCommand = new DelegateCommand<object>(CancelMethod, CanCancelMethod);
            CreateCommand = new DelegateCommand<object>(CreateMethod, CanCreateMethod);
            RemoveCommand = new DelegateCommand<object>(RemoveMethod, CanRemoveMethod);
            AddCommand = new DelegateCommand<object>(AddMethod, CanAddMethod);
            CloseCommand = new DelegateCommand<object>(CloseMethod);
        }


        #endregion

        #region Commands

        public DelegateCommand<object> SaveCommand { get; private set; }

        public virtual void SaveMethod(object parameter)
        {
            BeginSave();
        }

        public virtual bool CanSaveMethod(object parameter)
        {
            return CanSave;
        }


        public DelegateCommand<object> CancelCommand { get; private set; }

        public virtual void CancelMethod(object parameter)
        {
            ITrackStatus trackable = Model as ITrackStatus;
            if (trackable != null)
            {
                _selectedIndex = -1;
                if (_selectedItem != null && IsBoundToList)
                {
                    IEnumerable list = Model as IEnumerable;
                    _selectedIndex = (from object one in list select one).ToList().IndexOf(_selectedItem);
                }
                DoCancel();
                if (trackable.IsNew)
                {
                    OnAfterCancelNew();
                }
                if (_selectedIndex >= 0)
                {
                    var itemList = (from object one in ((IEnumerable)Model) select one).ToList();
                    if (itemList.Count > _selectedIndex)
                    {
                        SelectedItem = itemList.Skip(_selectedIndex).Take(1).First();
                    }
                    else
                    {
                        SelectFirstItem();
                    }
                }
                else
                {
                    SelectFirstItem();
                }
            }
            RefreshCommands();
        }

        public virtual bool CanCancelMethod(object parameter)
        {
            return CanCancel;
        }


        public DelegateCommand<object> DeleteCommand { get; private set; }

        public virtual void DeleteMethod(object parameter)
        {
            if (MessageBox.Show("Delete?", "Rolodex", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _savingAfterDelete = true;
                DoDelete();
                BeginSave();
            }
        }

        public virtual bool CanDeleteMethod(object parameter)
        {
            return CanDelete;
        }

        public DelegateCommand<object> RemoveCommand { get; private set; }

        public virtual void RemoveMethod(object parameter)
        {
            DoRemove(parameter);
        }

        public virtual bool CanRemoveMethod(object parameter)
        {
            return CanRemove;
        }


        public DelegateCommand<object> AddCommand { get; private set; }

        public virtual void AddMethod(object parameter)
        {
            BeginAddNew();
            SelectLastItem();
        }

        protected void SelectLastItem()
        {
            if (IsBoundToList)
            {
                var itemList = (from object one in ((IEnumerable)Model) select one).ToList();
                if (itemList.Count > 0)
                {
                    SelectedItem = itemList.Skip(itemList.Count - 1).Take(1).First();
                }
            }
        }

        protected void SelectFirstItem()
        {
            if (IsBoundToList)
            {
                var itemList = (from object one in ((IEnumerable)Model) select one).ToList();
                if (itemList.Count > 0)
                {
                    SelectedItem = itemList.First();
                }
            }
        }

        public virtual bool CanAddMethod(object parameter)
        {
            return CanAddNew;
        }

        public DelegateCommand<object> CreateCommand { get; private set; }

        public virtual void CreateMethod(object parameter)
        {
            OnCreateNew(parameter);
        }

        protected virtual void OnCreateNew(object parameter)
        {
            throw new Exception("Create method must be overridden");
        }

        public virtual bool CanCreateMethod(object parameter)
        {
            return CanCreate;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "CanSave")
                SaveCommand.RaiseCanExecuteChanged();
            else if (propertyName == "CanCancel")
                CancelCommand.RaiseCanExecuteChanged();
            else if (propertyName == "CanDelete")
                DeleteCommand.RaiseCanExecuteChanged();
            else if (propertyName == "CanRemove")
                RemoveCommand.RaiseCanExecuteChanged();
            else if (propertyName == "CanAddNew")
                AddCommand.RaiseCanExecuteChanged();
            else if (propertyName == "IsBusy")
            {
                RefreshCommands();
            }
        }

        private void RefreshCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            CreateCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
        }

        protected override void BeginSave()
        {
            _selectedIndex = -1;
            if (Model != null && Model is ITrackStatus)
            {
                ITrackStatus trackable = Model as ITrackStatus;
                if (trackable.IsValid)
                {
                    if (_selectedItem != null && IsBoundToList)
                    {
                        IEnumerable list = Model as IEnumerable;
                        _selectedIndex = (from object one in list select one).ToList().IndexOf(_selectedItem);
                    }
                    base.BeginSave();
                }
                else
                {
                    MessageBox.Show("Please correct invalid values.", "Rolodex", MessageBoxButton.OK);
                    _savingAtClose = false;
                }

            }
        }

        protected override void OnSaved()
        {
            if (Error != null)
            {
                _savingAtClose = false;
                //TODO: log errors
                MessageBox.Show(Error.Message, "Rolodex", MessageBoxButton.OK);
            }
            else
            {
                if (_savingAfterDelete)
                {
                    OnAfterDeleteSave();
                    _savingAfterDelete = false;
                }
                if (_selectedIndex >= 0)
                {
                    var itemList = (from object one in ((IEnumerable)Model) select one).ToList();
                    if (itemList.Count >= _selectedIndex)
                    {
                        SelectedItem = itemList.Skip(_selectedIndex).Take(1).First();
                    }
                    else
                    {
                        SelectFirstItem();
                    }
                }
                if (_savingAtClose)
                {
                    CleanupAtClose();
                }
            }
            AfterSave();
            RefreshCommands();
        }

        protected virtual void OnAfterDeleteSave()
        { }

        protected virtual void OnAfterCancelNew()
        { }

        public DelegateCommand<object> CloseCommand { get; private set; }

        protected virtual void CloseMethod(object parameter)
        {
            bool closeView = true;
            if (Model != null)
            {
                if (Model.IsDirty)
                {
                    closeView = false;
                    if (MessageBox.Show("Save changers?", "Rolodex", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        SaveMethod(null);
                        CleanupAtClose();
                    }
                }
            }
            if (closeView)
            {
                Aggregator.GetEvent<CloseRequested>().Publish(this);
                CleanupAtClose();
            }
        }

        private void CleanupAtClose()
        {
            Cleanup();
        }

        public virtual void Cleanup()
        {
            Model = default(TModel);
        }

        #endregion

        #region Error Handling

        protected virtual bool HasNoException(IDataPortalResult result)
        {
            bool returnValue = (result.Error == null);

            if (!returnValue)
            {
                //TODO: log errors
                MessageBox.Show(result.Error.Message, "Rolodex", MessageBoxButton.OK);
            }
            return returnValue;
        }

        #endregion

        #region Bases Methods

        public virtual void Activated()
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void Initialize(object parameter)
        {

        }

        public virtual void AfterSave()
        {

        }

        #endregion

    }
}
