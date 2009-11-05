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
Imports Rolodex.Business.BusinessClasses

Namespace Rolodex
  Public Class RanksConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert

      Dim provider As Csla.Silverlight.CslaDataProvider = TryCast(parameter, Csla.Silverlight.CslaDataProvider)

      If value IsNot Nothing AndAlso provider IsNot Nothing AndAlso provider.Data IsNot Nothing Then
        Dim rankList As Ranks = TryCast(provider.Data, Ranks)
        Return rankList.GetItemByKey(CInt(value))
      Else
        Return Nothing
      End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
      Dim provider As Csla.Silverlight.CslaDataProvider = TryCast(parameter, Csla.Silverlight.CslaDataProvider)

      If value IsNot Nothing AndAlso provider IsNot Nothing AndAlso provider.Data IsNot Nothing Then
        Dim returnValue = DirectCast(value, Ranks.NameValuePair)
        If returnValue IsNot Nothing Then
          Return returnValue.Key
        Else
          Return 0
        End If
      Else
        Return 0
      End If
    End Function

  End Class
End Namespace
