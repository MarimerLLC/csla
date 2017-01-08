using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Practices.Composite.Events;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.ViewModels
{
    public class WaitWindowViewModel : DependencyObject, INotifyPropertyChanged
    {
        #region Private members

        IFocusable view;
        #endregion

        #region Constructor
        public WaitWindowViewModel(IEventAggregator eventAggregator, IFocusable view)
        {
            this.view = view;
            eventAggregator.GetEvent<WaitWindowEvent>().Subscribe(HandleIsRunningChanged);
        }
        #endregion

        #region Dependency Properties
        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }


        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(WaitWindowViewModel), new PropertyMetadata(false));

        #endregion

        #region Event handling

        public void HandleIsRunningChanged(bool isRunning)
        {
            IsRunning = isRunning;
            OnPropertyChanged("IsRunning");
            Dispatcher.BeginInvoke(new Action(delegate() { view.Focus(); }));
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
