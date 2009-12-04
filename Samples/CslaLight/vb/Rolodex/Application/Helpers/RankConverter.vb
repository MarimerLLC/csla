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
Imports Rolodex.Business.BusinessClasses

Namespace Rolodex
  Public Class RankConverter
    Implements IValueConverter
    Private rankList As Ranks
    Public Sub New()
      Ranks.GetRanks(AddressOf EndGetRanks)
    End Sub

    Private Sub EndGetRanks(ByVal sender As Object, ByVal e As Csla.DataPortalResult(Of Ranks))
      rankList = e.Object
      OnGotData()
    End Sub


    Public Event GotData As EventHandler

    Protected Sub OnGotData()
      If GotDataEvent IsNot Nothing Then
        GotDataEvent.Invoke(Me, EventArgs.Empty)
      End If
    End Sub

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
      If value IsNot Nothing AndAlso TypeOf value Is Integer AndAlso rankList IsNot Nothing Then
        Return rankList.GetRankName(CInt(Fix(value)))
      Else
        Return String.Empty
      End If

    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
      If value IsNot Nothing AndAlso TypeOf value Is String AndAlso rankList IsNot Nothing Then
        Dim returnValue = rankList.GetItemByValue(CStr(value))
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
