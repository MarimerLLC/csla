Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace Rolodex
  Partial Public Class CompaniesList
    Inherits UserControl
    Public Event CompanySelected As EventHandler(Of CompanySelectedEventArgs)
    Public Event NewCompanyRequested As EventHandler

    Public Event ShowRanksRequested As EventHandler

    Public Sub New()
      InitializeComponent()
    End Sub

    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If CompanySelectedEvent IsNot Nothing Then
        CompanySelectedEvent.Invoke(Me, New CompanySelectedEventArgs(CInt((CType(sender, Button)).Tag)))
      End If
    End Sub

    Private Sub EditRanks_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If ShowRanksRequestedEvent IsNot Nothing Then
        ShowRanksRequestedEvent.Invoke(Me, EventArgs.Empty)
      End If
    End Sub

    Private Sub NewCompany_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If NewCompanyRequestedEvent IsNot Nothing Then
        NewCompanyRequestedEvent.Invoke(Me, EventArgs.Empty)
      End If
    End Sub

    Private Sub CslaDataProvider_DataChanged(ByVal sender As Object, ByVal e As EventArgs)
      If (CType(sender, Csla.Silverlight.CslaDataProvider)).Error IsNot Nothing Then
        System.Windows.Browser.HtmlPage.Window.Alert((CType(sender, Csla.Silverlight.CslaDataProvider)).Error.Message)
      End If
    End Sub
  End Class
End Namespace
