Public Class VisibilityConverter
  Implements System.Windows.Data.IValueConverter

  Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert

    If value.ToString = "True" Then
      Return System.Windows.Visibility.Visible

    Else
      Return System.Windows.Visibility.Collapsed
    End If

  End Function

  Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
    Return value
  End Function
End Class
