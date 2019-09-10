using System.Windows.Controls;
using System.Windows.Data;
using Csla.Xaml;

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
          BindingExpression expression = GetBindingExpression(SelectedValueProperty);
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