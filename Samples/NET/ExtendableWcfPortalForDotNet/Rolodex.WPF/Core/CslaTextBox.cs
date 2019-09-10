using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Csla.Xaml;

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
        typeof(object),
        typeof(CslaTextBox),
        new PropertyMetadata(MyDataContextChanged));

    private static void MyDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      CslaTextBox myControl = (CslaTextBox) sender;
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
          BindingExpression expression = GetBindingExpression(TextProperty);
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