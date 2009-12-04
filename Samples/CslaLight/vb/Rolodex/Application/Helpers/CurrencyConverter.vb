Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Windows.Data
Imports System.Globalization

Namespace Rolodex
  Public Class CurrencyConverter
	  Implements IValueConverter
	Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
	  If value IsNot Nothing AndAlso value.ToString() <> String.Empty Then
		Dim price As Decimal = CDec(value)
		Return price.ToString("C")
	  Else
		Return Nothing
	  End If
	End Function

	Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
	  Dim price As String = value.ToString()

	  Dim result As Decimal
	  If Decimal.TryParse(price, NumberStyles.Any, Nothing, result) Then
		Return result
	  End If
	  Return 0
	End Function
  End Class
End Namespace
