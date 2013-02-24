Imports System.Windows.Browser.HtmlPage
Imports Csla.Xaml
Imports Csla.DataPortal

Partial Public Class Page
  Inherits UserControl

  Public Sub New()
    InitializeComponent()
  End Sub

  Private WithEvents _provider As CslaDataProvider

  Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    _provider = CType(Resources("MyData"), CslaDataProvider)
    With _provider
      .FactoryParameters.Clear()
      .FactoryParameters.Add(ProxyModes.LocalOnly)
      .Refresh()
    End With

  End Sub

  Private Sub CslaDataProvider_DataChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _provider.DataChanged

    If _provider.Error IsNot Nothing Then
      Window.Alert(_provider.Error.Message)
    End If

  End Sub

  'Private Sub _provider_PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles _provider.PropertyChanged

  '  If _provider.IsBusy Then
  '    Me.Overlay.Visibility = Windows.Visibility.Visible
  '  Else
  '    Me.Overlay.Visibility = Windows.Visibility.Collapsed
  '  End If

  'End Sub
End Class