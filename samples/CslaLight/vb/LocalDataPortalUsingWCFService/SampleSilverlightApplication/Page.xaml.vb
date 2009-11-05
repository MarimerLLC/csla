Imports Csla.Silverlight
Imports System.Collections.ObjectModel
Imports WcfService.Business.Client

Partial Public Class Page
  Inherits UserControl

  Private _reloadInitalCompany As Boolean = False

  Public Sub New()
    InitializeComponent()
  End Sub

  Private Sub CslaDataProvider_PropertyChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs)
    If e.PropertyName = "IsBusy" Then
      Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
      If TypeOf provider.Data Is Csla.Core.ITrackStatus Then
        If CType(provider.Data, Csla.Core.ITrackStatus).IsDeleted Then
          _reloadInitalCompany = True
        Else
          _reloadInitalCompany = False
        End If
      End If
    End If
  End Sub

  Private Sub CslaDataProvider_DataChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    If provider.Error IsNot Nothing Then
      System.Windows.Browser.HtmlPage.Window.Alert(provider.Error.Message)
    Else
      If _reloadInitalCompany Then
        StartShowingData()
      End If
    End If
  End Sub

  Private Sub Page_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    Me.objectBusy.IsRunning = True
    SamplePrincipal.Login("admin", "admin", Function(o1, e1) StartShowingData())
  End Sub

  Private Function StartShowingData() As Boolean
    Dispatcher.BeginInvoke(AddressOf ShowData)
    Return True
  End Function

  Private Sub ShowData()
    Me.objectBusy.IsRunning = False
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    provider.FactoryMethod = "GetCompany"
    provider.FactoryParameters.Add(2)
    provider.Refresh()
  End Sub

  Private Sub CreateButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    Dim provider As CslaDataProvider = CType(Me.Resources("CompanyData"), CslaDataProvider)
    provider.FactoryParameters.Clear()
    provider.FactoryMethod = "CreateCompany"
    provider.Refresh()
  End Sub

End Class