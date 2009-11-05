Imports Csla.Silverlight
Imports System.Collections.ObjectModel
Imports Sample.Business
Imports System.ComponentModel

Partial Public Class Page
  Inherits UserControl
  Private _companyData As Company
  Private _isCompanyDeleted As Boolean = False

  Public Sub New()
    InitializeComponent()
  End Sub

  Private Sub Page_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    Me.screenBusy.IsRunning = True
    SamplePrincipal.Login("admin", "admin", AddressOf ShowData)
  End Sub

  Private Sub ShowData(ByVal sender As Object, ByVal e As EventArgs)
    Me.screenBusy.IsRunning = True
    Company.GetCompany(2, AddressOf BindData)
  End Sub

  Private Sub HandlePropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
    If _companyData.IsDirty Then
      Me.CancelButton.IsEnabled = True
    End If
    If _companyData.IsValid Then
      Me.SaveButton.IsEnabled = True
    Else
      Me.SaveButton.IsEnabled = False
    End If
  End Sub

  Private Sub AttachHandlers(ByVal companyInfo As Company)
    If _companyData IsNot Nothing Then
      RemoveHandler _companyData.PropertyChanged, AddressOf HandlePropertyChanged
    End If
    _companyData = companyInfo
    If companyInfo IsNot Nothing Then
      _companyData.BeginEdit()
      AddHandler _companyData.PropertyChanged, AddressOf HandlePropertyChanged
      SetInitialButtonStates()
    Else
      DisableButtons()
    End If
  End Sub

  Private Sub SetInitialButtonStates()
    Me.SaveButton.IsEnabled = False
    Me.CancelButton.IsEnabled = False
    Me.CreateButton.IsEnabled = True
    Me.DeleteButton.IsEnabled = True
  End Sub

  Private Sub DisableButtons()
    Me.SaveButton.IsEnabled = False
    Me.CancelButton.IsEnabled = False
    Me.CreateButton.IsEnabled = False
    Me.DeleteButton.IsEnabled = False
  End Sub

  Private Sub SetButtonsForDelete()
    Me.SaveButton.IsEnabled = True
    Me.CancelButton.IsEnabled = True
    Me.CreateButton.IsEnabled = False
    Me.DeleteButton.IsEnabled = False
  End Sub

  Private Sub BindData(ByVal sender As Object, ByVal e As Csla.DataPortalResult(Of Company))
    If e.Error Is Nothing AndAlso e.Object IsNot Nothing Then
      SetInitialButtonStates()
      AttachHandlers(e.Object)
    Else
      System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message)
      DisableButtons()
    End If
    Me.LayoutRoot.DataContext = _companyData
    Me.screenBusy.IsRunning = False
  End Sub

  Private Sub CreateButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    Me.screenBusy.IsRunning = True
    Company.CreateCompany(AddressOf BindData)
  End Sub

  Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    If _companyData IsNot Nothing Then
      If _companyData.IsNew Then
        ShowData(Me, EventArgs.Empty)
      Else
        _companyData.CancelEdit()
        _companyData.BeginEdit()
        SetInitialButtonStates()
      End If
    End If
  End Sub

  Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    If _companyData IsNot Nothing Then
      Me.screenBusy.IsRunning = True
      _isCompanyDeleted = _companyData.IsDeleted
      Dim tempCompany As Company = _companyData.Clone()
      tempCompany.ApplyEdit()
      tempCompany.BeginSave(AddressOf QueryCompleted)
    End If
  End Sub

  Private Sub QueryCompleted(ByVal sender As Object, ByVal e As Csla.Core.SavedEventArgs)
    Me.screenBusy.IsRunning = False
    If e.Error Is Nothing AndAlso e.NewObject IsNot Nothing Then
      AttachHandlers(CType(e.NewObject, Company))
      Me.LayoutRoot.DataContext = _companyData
      If _isCompanyDeleted Then
        ShowData(Me, EventArgs.Empty)
      End If
    Else
      System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message)
    End If
  End Sub

  Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    If _companyData IsNot Nothing Then
      _companyData.Delete()
      SetButtonsForDelete()
    End If
  End Sub

End Class