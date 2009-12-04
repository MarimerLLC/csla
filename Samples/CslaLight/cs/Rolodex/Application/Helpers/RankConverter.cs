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
using System.Globalization;
using Rolodex.Business.BusinessClasses;

namespace Rolodex
{
  public class RankConverter : IValueConverter
  {
    Ranks rankList;
    public RankConverter()
    {
      Ranks.GetRanks((o, e) =>
        {
          rankList = e.Object;
          OnGotData();
        });
    }

    public event EventHandler GotData;

    protected void OnGotData()
    {
      if (GotData != null)
        GotData.Invoke(this, EventArgs.Empty);
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null && value is int && rankList != null)
      {
        return rankList.GetRankName((int)value);
      }
      else
        return string.Empty;;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null && value is string && rankList != null)
      {
        var returnValue = rankList.GetItemByValue((string)value);
        if (returnValue != null)
          return returnValue.Key;
        else
          return 0;
      }
      else
        return 0;
    }
  }
}
