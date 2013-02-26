using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WinRTUI
{
  public class StringNullConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var v = (string)value;
      if (string.IsNullOrWhiteSpace(v))
        return null;
      else
        return v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return value;
    }
  }
}
