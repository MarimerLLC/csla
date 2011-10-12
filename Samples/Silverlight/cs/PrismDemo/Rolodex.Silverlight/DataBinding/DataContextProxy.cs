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
            get { return (Object)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(object), typeof(DataContextProxy), new PropertyMetadata(DataSourceChanged));

        private static void DataSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as DataContextProxy).OnPropertyChanged("DataSource");
        }

        public DataContextProxy()
        {
            this.Loaded += new RoutedEventHandler(DataContextProxy_Loaded);
        }

        void DataContextProxy_Loaded(object sender, RoutedEventArgs e)
        {
            INotifyPropertyChanged source = this.DataContext as INotifyPropertyChanged;
            if (source != null && !string.IsNullOrEmpty(BindingPropertyName))
            {
                source.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
            }
            CreateBinding();
        }

        private void CreateBinding()
        {
            Binding binding = new Binding();
            if (!string.IsNullOrEmpty(BindingPropertyName))
            {
                binding.Path = new PropertyPath(BindingPropertyName);
            }
            binding.Source = this.DataContext;
            binding.Mode = BindingMode;
            this.SetBinding(DataContextProxy.DataSourceProperty, binding);
        }

        private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
