Imports System.Windows.Data

Namespace Converters

  <ValueConversion(GetType(Date), GetType(String))> _
  Public Class DateConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert

      If IsDate(value) Then
        Dim dt As Date = CDate(value)
        If dt.Equals(DateTime.MinValue) Then
          Return ""

        Else
          Return CDate(value).ToShortDateString
        End If

      Else
        Return ""
      End If

    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack

      If IsDate(value) Then
        Return CDate(value)

      Else
        Return DateTime.MinValue
      End If

    End Function

  End Class

End Namespace
