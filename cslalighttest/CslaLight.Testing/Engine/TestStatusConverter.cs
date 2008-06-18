using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace cslalighttest.Engine
{
  public class TestStatusConverter : IValueConverter
  {

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      MethodTesterStatus status = (MethodTesterStatus)value;
      switch (status)
      {
        case MethodTesterStatus.Indeterminate:
          return new SolidColorBrush(Colors.Yellow);
        case MethodTesterStatus.Success:
          return new SolidColorBrush(Colors.Green);
        case MethodTesterStatus.Fail:
          return new SolidColorBrush(Colors.Red);
        case MethodTesterStatus.Evaluating:
        default:
          return new SolidColorBrush(Colors.LightGray);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
