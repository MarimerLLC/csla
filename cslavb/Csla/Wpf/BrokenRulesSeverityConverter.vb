Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Validation
Imports System.Windows.Resources
Imports System.Windows.Media.Imaging
Imports System.Windows
Imports System.Windows.Data

Namespace Wpf
  Public Class BrokenRuleSeverityConverter
    Implements IValueConverter
#Region "IValueConverter Members"

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
      Dim severity As RuleSeverity = DirectCast(value, RuleSeverity)
      Dim uri As String = String.Format("/Csla;component/Resources/{0}.png", severity)
      Dim sr As StreamResourceInfo = Application.GetResourceStream(New Uri(uri, UriKind.Relative))
      Dim bmp As New BitmapImage()
      bmp.BeginInit()
      bmp.StreamSource = sr.Stream
      bmp.EndInit()

      Return bmp
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
      Return RuleSeverity.[Error]
    End Function

#End Region
  End Class
End Namespace