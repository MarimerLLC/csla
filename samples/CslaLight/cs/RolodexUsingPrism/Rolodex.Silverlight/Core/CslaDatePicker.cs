using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Csla.Silverlight;
#if !SILVERLIGHT
using Microsoft.Windows.Controls;
using Csla.Wpf;
#endif
namespace Rolodex.Silverlight.Core
{

    public class CslaDatePicker : DatePicker
    {
        public CslaDatePicker()
            : base()
        {
            DefaultStyleKey = typeof(CslaDatePicker);
            SetBinding(MyDataContextProperty, new Binding());
        }

        private static readonly DependencyProperty MyDataContextProperty =
        DependencyProperty.Register("MyDataContext",
                                    typeof(Object),
                                    typeof(CslaDatePicker),
                                    new PropertyMetadata(MyDataContextChanged));

        private static void MyDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CslaDatePicker myControl = (CslaDatePicker)sender;
            myControl.SetStatusControl();
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
                    BindingExpression expression = GetBindingExpression(CslaDatePicker.SelectedDateProperty);
                    if (expression != null &&
                        expression.ParentBinding != null &&
                        expression.ParentBinding.Path != null)
                    {
                        Binding binding =
                            new Binding(expression.ParentBinding.Path.Path) { Source = expression.ParentBinding.Source };
#if !SILVERLIGHT
                        if (binding.Source == null)
                        {
                            binding.Source = DataContext;
                        }
#endif
                        status.SetBinding(PropertyStatus.PropertyProperty, binding);
                        status.TargetControl = this;
                    }
                }
            }
        }
    }
}
