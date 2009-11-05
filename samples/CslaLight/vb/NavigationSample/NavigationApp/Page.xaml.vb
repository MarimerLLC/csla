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
Imports System.Windows.Browser
Imports Csla.Silverlight

Namespace NavigationApp
  Partial Public Class Page
    Inherits UserControl
    Public Sub New()
      InitializeComponent()
      AddHandler Loaded, AddressOf Page_Loaded
    End Sub

    Private Sub Page_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Navigator.Current.ContentPlaceholder = Me.PlaceHolder
      AddHandler Navigator.Current.BeforeNavigation, AddressOf HandleNavigation

    End Sub

    Private Sub HandleNavigation(ByVal sender As Object, ByVal e As Csla.Silverlight.NavigationEventArgs)
      If e.ControlTypeName = GetType(ControlTwo).AssemblyQualifiedName AndAlso e.IsInitiatedByBrowserButton = False Then
        e.Parameters = "Parameter=" + (New Random()).[Next](1, 100).ToString()
      End If
      If e.ControlTypeName.ToUpper().Contains("ControlThree".ToUpper()) Then
        e.Cancel = True
      End If
      If e.ControlTypeName.ToUpper().Contains("ControlFour".ToUpper()) Then
        e.Cancel = True
        Dim bookmark As New BoomarkInformation("NavigationApp.ControlOne, NavigationApp, Version=..., Culture=neutral, PublicKeyToken=null", "", "COntrol One - Redirected")
        e.RedirectToOnCancel = bookmark
      End If

    End Sub
  End Class
End Namespace
