using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
#if SILVERLIGHT
using Csla.Silverlight;
#else
using Csla.Wpf;
#endif

namespace Rolodex.Silverlight.Core
{
    public class CslaComboBox : ComboBox
    {
        private bool _updating = false;
        public CslaComboBox()
        {
            DefaultStyleKey = typeof(CslaComboBox);
            SetBinding(MyDataContextProperty, new Binding());
            SelectionChanged += CslaComboBox_SelectionChanged;
        }

        protected void CslaComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            BindingExpression expression = GetBindingExpression(CslaComboBox.SelectedValueProperty);
            if (expression != null &&
                expression.ParentBinding != null &&
                expression.ParentBinding.Path != null)
            {
                _updating = true;
                if (SelectedItem != null)
                {
                    object value = SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(SelectedItem, null);
                    SelectedValue = value;
                    expression.UpdateSource();
                }
                else
                {
                    SelectedValue = null;
                    expression.UpdateSource();
                }
                _updating = false;
            }
        }

        private static readonly DependencyProperty MyDataContextProperty =
        DependencyProperty.Register("MyDataContext",
                                    typeof(Object),
                                    typeof(CslaComboBox),
                                    new PropertyMetadata(DataContextChanged));

        private static void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CslaComboBox myControl = (CslaComboBox)sender;
            myControl.SetStatusControl();
        }


        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
             DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(CslaComboBox), new PropertyMetadata(SelectedValuePathChanged));

        private static void SelectedValuePathChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CslaComboBox myControl = (CslaComboBox)sender;
            myControl.SetStatusControl();
        }

        public static readonly DependencyProperty SelectedValueProperty =
             DependencyProperty.Register("SelectedValue", typeof(object), typeof(CslaComboBox), new PropertyMetadata(SelectedValueChanged));

        private static void SelectedValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CslaComboBox myControl = (CslaComboBox)sender;
            if (!myControl._updating && myControl.ItemsSource != null)
            {
                BindingExpression expression = myControl.GetBindingExpression(CslaComboBox.SelectedValueProperty);
                if (expression != null &&
                    expression.ParentBinding != null &&
                    expression.ParentBinding.Path != null)
                {
                    object source = myControl.DataContext;
                    if (expression.ParentBinding.Source != null)
                    {
                        source = expression.ParentBinding.Source;
                    }
                    PropertyInfo prop = null;
                    object value = null;
                    foreach (object item in myControl.ItemsSource)
                    {
                        if (prop == null)
                        {
                            prop = item.GetType().GetProperty(myControl.SelectedValuePath);
                        }
                        value = prop.GetValue(item, null);
                        if (value.Equals(myControl.SelectedValue))
                        {
                            myControl.SelectedItem = item;
                            break;
                        }
                    }
                }

            }
        }

        public object SelectedValue
        {
            get
            {
                return GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetStatusControl();
        }


        private void SetStatusControl()
        {
            if (Template != null && DesignModeHelper.IsInDesignMode == false)
            {
                PropertyStatus status = GetTemplateChild("PART_PropertyStatus") as PropertyStatus;
                if (status != null)
                {
                    BindingExpression expression = GetBindingExpression(CslaComboBox.SelectedValueProperty);
                    if (expression != null &&
                        expression.ParentBinding != null &&
                        expression.ParentBinding.Path != null)
                    {
                        Binding binding =
                            new Binding(expression.ParentBinding.Path.Path) { Source = expression.ParentBinding.Source };
                        status.SetBinding(PropertyStatus.PropertyProperty, binding);
                        status.TargetControl = this;
                    }
                }
            }
        }

    }
}
