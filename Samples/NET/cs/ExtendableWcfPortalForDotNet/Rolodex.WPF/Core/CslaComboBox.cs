using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Csla.Xaml;
using System.Windows.Data;

namespace Rolodex.Silverlight.Core
{
    public class CslaComboBox : ComboBox
    {
        public CslaComboBox()
        {
            DefaultStyleKey = typeof(CslaComboBox);
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "DataContext")
            {
                SetStatusControl();
            }
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
                        if (binding.Source == null)
                        {
                            binding.Source = DataContext;
                        }
                        status.SetBinding(PropertyStatus.PropertyProperty, binding);
                        status.TargetControl = this;
                    }
                }
            }
        }
    }
}
