using System;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;

namespace Rolodex.Silverlight.DataBinding
{
    public class DataContextProxy : FrameworkElement, INotifyPropertyChanged
    {
        public string BindingPropertyName { get; set; }
        public BindingMode BindingMode { get; set; }

        public Object DataSource
        {
            get { return GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(object), typeof(DataContextProxy), new PropertyMetadata(DataSourceChanged));

        private static void DataSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var dataContextProxy = source as DataContextProxy;
            if (dataContextProxy != null) dataContextProxy.OnPropertyChanged("DataSource");
        }

        public DataContextProxy()
        {
            Loaded += DataContextProxyLoaded;
        }

        void DataContextProxyLoaded(object sender, RoutedEventArgs e)
        {
            var source = DataContext as INotifyPropertyChanged;
            if (source != null && !string.IsNullOrEmpty(BindingPropertyName))
            {
                source.PropertyChanged += SourcePropertyChanged;
            }
            CreateBinding();
        }

        private void CreateBinding()
        {
            var binding = new Binding();
            if (!string.IsNullOrEmpty(BindingPropertyName))
            {
                binding.Path = new PropertyPath(BindingPropertyName);
            }
            binding.Source = DataContext;
            binding.Mode = BindingMode;
            SetBinding(DataSourceProperty, binding);
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BindingPropertyName)
            {
                CreateBinding();
                OnPropertyChanged("DataSource");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
