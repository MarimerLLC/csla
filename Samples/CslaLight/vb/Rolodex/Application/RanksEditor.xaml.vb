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
  Partial Public Class RanksEditor
    Inherits UserControl
    Public Sub New()
      InitializeComponent()
    End Sub

    Public Event CloseRequested As EventHandler

    Private Sub CslaDataProvider_PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs)
      If e.PropertyName = "Error" AndAlso (CType(sender, Csla.Silverlight.CslaDataProvider)).Error IsNot Nothing Then
        System.Windows.Browser.HtmlPage.Window.Alert((CType(sender, Csla.Silverlight.CslaDataProvider)).Error.Message)
      End If
    End Sub

    Private Sub AddRank_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

    End Sub

    Private Sub CloseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If CloseRequestedEvent IsNot Nothing Then
        CloseRequestedEvent.Invoke(Me, EventArgs.Empty)
      End If
    End Sub
  End Class
End Namespace
