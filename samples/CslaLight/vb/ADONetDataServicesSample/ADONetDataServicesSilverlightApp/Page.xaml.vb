Imports DataServices.Business
Imports Csla

Partial Public Class Page
  Inherits UserControl

  Private _companyData As Company
  Private _reloadOldCompany As Boolean = False

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

  Private Sub BindData(ByVal sender As Object, ByVal e As Csla.DataPortalResult(Of Company))
    If e.Error Is Nothing AndAlso e.Object IsNot Nothing Then
      _companyData = e.Object
      _companyData.BeginEdit()
    Else
      System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message)
    End If
    Me.LayoutRoot.DataContext = _companyData
    Me.screenBusy.IsRunning = False
  End Sub

  Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    If _companyData IsNot Nothing Then
      Try
        Me.screenBusy.IsRunning = True
        _reloadOldCompany = _companyData.IsDeleted
        Dim tempCompany As Company = _companyData.Clone()
        tempCompany.ApplyEdit()
        tempCompany.BeginSave(AddressOf QueryCompleted)
      Catch ex As Exception
        System.Windows.Browser.HtmlPage.Window.Alert(ex.Message)
      End Try

    End If
  End Sub

  Private Sub QueryCompleted(ByVal sender As Object, ByVal e As Csla.Core.SavedEventArgs)
    Me.screenBusy.IsRunning = False
    If e.Error Is Nothing AndAlso e.NewObject IsNot Nothing Then
      _companyData = CType(e.NewObject, Company)
      _companyData.BeginEdit()
      Me.LayoutRoot.DataContext = _companyData
      If _reloadOldCompany Then
        ShowData(Me, EventArgs.Empty)
      End If
    Else
      System.Windows.Browser.HtmlPage.Window.Alert(e.Error.Message)
    End If
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
      End If
    End If
  End Sub

  Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    If _companyData IsNot Nothing Then
      _companyData.Delete()
    End If
  End Sub

End Class