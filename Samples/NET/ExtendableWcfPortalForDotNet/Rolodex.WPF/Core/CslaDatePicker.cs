using System.Windows;
using System.Windows.Data;
using Csla.Xaml;
using Microsoft.Windows.Controls;

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
        typeof(object),
        typeof(CslaDatePicker),
        new PropertyMetadata(MyDataContextChanged));

    private static void MyDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var myControl = (CslaDatePicker) sender;
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
        var status = GetTemplateChild("PART_PropertyStatus") as PropertyStatus;
        if (status != null)
        {
          BindingExpression expression = GetBindingExpression(SelectedDateProperty);
          if (expression != null &&
              expression.ParentBinding != null &&
              expression.ParentBinding.Path != null)
          {
            var binding = new Binding(expression.ParentBinding.Path.Path) {Source = expression.ParentBinding.Source};

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