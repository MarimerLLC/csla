#If Not NET20 Then
Imports System.Windows.Data

Namespace Wpf

  ''' <summary>
  ''' Provides the functionality of a WPF
  ''' value converter without affecting the
  ''' value as it flows to and from the UI.
  ''' </summary>
  Public Class IdentityConverter
    Implements IValueConverter

#Region "IValueConverter Members"

    ''' <summary>
    ''' Returns the unchanged value.
    ''' </summary>
    ''' <param name="value">Value to be converted.</param>
    ''' <param name="targetType">Desired value type.</param>
    ''' <param name="parameter">Conversion parameter.</param>
    ''' <param name="culture">Conversion culture.</param>
    Public Function Convert(ByVal value As Object, ByVal targetType As Type, _
      ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object _
      Implements IValueConverter.Convert

      Return value

    End Function

    ''' <summary>
    ''' Returns the unchanged value.
    ''' </summary>
    ''' <param name="value">Value to be converted.</param>
    ''' <param name="targetType">Desired value type.</param>
    ''' <param name="parameter">Conversion parameter.</param>
    ''' <param name="culture">Conversion culture.</param>
    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, _
      ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object _
      Implements IValueConverter.ConvertBack

      Return value

    End Function

#End Region

  End Class

End Namespace
#End If