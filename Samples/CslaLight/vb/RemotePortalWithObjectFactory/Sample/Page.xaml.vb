Imports Csla.Silverlight
Imports System.Collections.ObjectModel
Imports Sample.Business

Partial Public Class Page
  Inherits UserControl

  Public Sub New()
    InitializeComponent()
  End Sub

  Private Sub CslaDataProvider_PropertyChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs)

  End Sub

  Private Sub CslaDataProvider_DataChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    If provider.Error IsNot Nothing Then
      System.Windows.Browser.HtmlPage.Window.Alert(provider.Error.Message)
    End If
  End Sub

  Private Sub Page_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    Me.objectBusy.IsRunning = True
    SamplePrincipal.Login("admin", "admin", Function(o1, e1) ShowData())
  End Sub

  Private Function ShowData() As Boolean
    Me.objectBusy.IsRunning = False
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    provider.FactoryParameters.Add(2)
    provider.Refresh()
    Return True
  End Function

  Private Sub CreateButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    provider.FactoryParameters.Clear()
    provider.FactoryMethod = "CreateCompany"
    provider.Refresh()
  End Sub
End Class