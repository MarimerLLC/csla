using System;
using System.Windows.Data;
using System.Windows;

namespace Rolodex.Silverlight.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool useFalseAsVisible = false;
            if (parameter != null && parameter.ToString().ToUpper() == "FALSE")
                useFalseAsVisible = true;
            if (!useFalseAsVisible)
            {
                if ((bool)value == true)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
            {
                if ((bool)value == true)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
