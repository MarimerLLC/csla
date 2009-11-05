using System;
using System.Windows;
using Rolodex.Silverlight.Modules;
using Microsoft.Practices.Composite.Presentation.Commands;
using Csla.Core;
using Csla;
using System.Collections.Generic;
using System.Windows.Data;

#if SILVERLIGHT
using System.Windows.Controls.Theming;
#endif

namespace Rolodex.Silverlight.Core
{
#if SILVERLIGHT
    public abstract class ViewModel<TModel, TView> : Csla.Silverlight.ViewModelBase<TModel>
#else
    public abstract class ViewModel<TModel, TView> : Csla.Wpf.ViewModelBase<TModel>
#endif
    where TView : FrameworkElement
    {

        #region View and Model

        private TView _view;
        public TView View
        {
            get { return _view; }
            protected set
            {
                _view = value;
                Binding binding = new Binding();
                binding.Source = this;
                _view.SetBinding(FrameworkElement.DataContextProperty, binding);
                ApplyTheme();
            }
        }

        public TModel TypedModel
        {
            get
            {
                if (Model is TModel)
                {
                    return (TModel)Model;
                }
                else
                    return default(TModel);
            }
        }


        protected ViewModel()
        {
            SaveCommand = new DelegateCommand<object>(SaveMethod, CanSaveMethod);
            CancelCommand = new DelegateCommand<object>(CancelMethod, CanCancelMethod);
            DeleteCommand = new DelegateCommand<object>(DeleteMethod, CanDeleteMethod);
            AddCommand = new DelegateCommand<object>(AddMethod, CanAddMethod);
            RemoveCommand = new DelegateCommand<object>(RemoveMethod, CanRemoveMethod);
        }

        #endregion

        #region Theming

        protected virtual void ApplyTheme()
        {
            if (_view != null)
            {
#if SILVERLIGHT
                string themeName = "ShinyBlue";
                if (Csla.ApplicationContext.LocalContext.ContainsKey("theme"))
                {
                    themeName = Csla.ApplicationContext.LocalContext["theme"].ToString();
                }
                Uri uri = new Uri(string.Concat("UserThemes/", themeName, ".xaml"), UriKind.Relative);
                ImplicitStyleManager.SetResourceDictionaryUri(_view, uri);
                ImplicitStyleManager.SetApplyMode(_view, ImplicitStylesApplyMode.Auto);
                ImplicitStyleManager.Apply(_view);
#else
                string themeName = "ShinyBlue";
                if (Csla.ApplicationContext.LocalContext.Contains("theme"))
                {
                    themeName = Csla.ApplicationContext.LocalContext["theme"].ToString();
                }
                
                Uri uri = new Uri(string.Concat(@"/Rolodex.WPF;component/UserThemes/", themeName, ".xaml"), UriKind.Relative);
                ResourceDictionary dictionary = Application.LoadComponent(uri) as ResourceDictionary;
                _view.Resources.MergedDictionaries.Clear();
                _view.Resources.MergedDictionaries.Add(dictionary);
#endif
               
            }
        }

        #endregion

        #region Wait Message

        protected void ShowPleaseWaitMessage()
        {
            PleaseWaitModule.ShowPleaseWaitMessage();
        }


        protected void HidePleaseWaitMessage()
        {
            PleaseWaitModule.HidePleaseWaitMessage();
        }

        #endregion

        #region Verbs

        private bool _savingAfterDelete = false;
        protected override void OnRefreshed()
        {
            HidePleaseWaitMessage();
            if (Error != null)
                ErrorHandler.HandleException(Error);
        }

        protected override void OnSaved()
        {
            HidePleaseWaitMessage();
            if (Error != null)
                ErrorHandler.HandleException(Error);
            if (_savingAfterDelete)
            {
                OnAfterDeleteSave();
                _savingAfterDelete = false;
            }
        }

        protected override void DoSave()
        {
            if (Model != null && Model is ITrackStatus)
            {
                ITrackStatus trackable = Model as ITrackStatus;
                if (trackable.IsValid)
                {
                    ShowPleaseWaitMessage();
                    base.DoSave();
                }
                else
                {
                    MessageBox.Show("Please correct invalid values before saving.", "Error", MessageBoxButton.OK);
                }

            }
        }

        protected override void DoRefresh(string factoryMethod, params object[] factoryParameters)
        {
            ShowPleaseWaitMessage();
            base.DoRefresh(factoryMethod, factoryParameters);
        }

        #endregion

        #region Commands

        public DelegateCommand<object> SaveCommand { get; private set; }

        private void SaveMethod(object parameter)
        {
            DoSave();
        }

        private bool CanSaveMethod(object parameter)
        {
            return CanSave;
        }


        public DelegateCommand<object> CancelCommand { get; private set; }

        private void CancelMethod(object parameter)
        {
            ITrackStatus trackable = Model as ITrackStatus;
            if (trackable != null)
            {
                DoCancel();
                if (trackable.IsNew)
                {
                    OnAfterCancelNew();
                }
            }
        }

        private bool CanCancelMethod(object parameter)
        {
            return CanCancel;
        }


        public DelegateCommand<object> DeleteCommand { get; private set; }

        private void DeleteMethod(object parameter)
        {
            if (MessageBox.Show("Delete this record?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _savingAfterDelete = true;
                DoDelete();
                DoSave();
            }
        }

        private bool CanDeleteMethod(object parameter)
        {
            return CanDelete;
        }

        public DelegateCommand<object> RemoveCommand { get; private set; }

        private void RemoveMethod(object parameter)
        {
            DoRemove(parameter);
        }

        private bool CanRemoveMethod(object parameter)
        {
            return CanRemove;
        }


        public DelegateCommand<object> AddCommand { get; private set; }

        private void AddMethod(object parameter)
        {
            DoAddNew();
        }

        private bool CanAddMethod(object parameter)
        {
            return CanAddNew;
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
                SaveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        protected virtual void OnAfterDeleteSave()
        { }

        protected virtual void OnAfterCancelNew()
        { }
        #endregion

        #region Data Loading

        protected virtual void OnAllSecondaryDataLoaded()
        { }

        protected EventHandler<DataPortalResult<T>> GetDataLoadedHandler<T>()
        {
            return Handler<T>;
        }

        protected void SetSecondaryDataCounter(int counter)
        {
            _secondaryDataCounter = counter;
            ShowPleaseWaitMessage();
        }
        private int _secondaryDataCounter = 0;
        private int _loadedCounter = 0;
        protected delegate void HandleLoad<T>(object sender, DataPortalResult<T> result);

        private void Handler<T>(object sender, DataPortalResult<T> result)
        {
            _loadedCounter += 1;
            if (result.Error != null)
            {
                ErrorHandler.HandleException(result.Error);
            }
            OnDataLoaded<T>(result);
            if (_loadedCounter >= _secondaryDataCounter)
            {
                OnAllSecondaryDataLoaded();
            }
        }

        protected virtual void OnDataLoaded<T>(DataPortalResult<T> result)
        { }

        #endregion


    }
}
