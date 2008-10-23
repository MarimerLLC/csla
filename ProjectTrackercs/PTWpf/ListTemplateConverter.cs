using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTWpf
{
  public class ListTemplateConverter : System.Windows.Data.IValueConverter
  {
    public System.Windows.DataTemplate TrueTemplate { get; set; }

    public System.Windows.DataTemplate FalseTemplate { get; set; }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, 
      object parameter, System.Globalization.CultureInfo culture)
    {
      if ((bool)value)
        return TrueTemplate;
      else
        return FalseTemplate;
    }

    public object ConvertBack(object value, Type targetType, 
      object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    #endregion
  }
}
