using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#if SILVERLIGHT
using Csla.Silverlight;
#else
using Csla.Wpf;
#endif

namespace Rolodex.Silverlight.Core
{
    public class CslaTextBox : TextBox
    {
        public CslaTextBox()
        {
            DefaultStyleKey = typeof(CslaTextBox);
            SetBinding(MyDataContextProperty, new Binding());
        }

        private static readonly DependencyProperty MyDataContextProperty =
        DependencyProperty.Register("MyDataContext",
                                    typeof(Object),
                                    typeof(CslaTextBox),
                                    new PropertyMetadata(MyDataContextChanged));

        private static void MyDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CslaTextBox myControl = (CslaTextBox)sender;
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
                    BindingExpression expression = GetBindingExpression(TextBox.TextProperty);
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
