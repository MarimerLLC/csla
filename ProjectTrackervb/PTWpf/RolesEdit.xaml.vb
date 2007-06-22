Imports ProjectTracker.Library.Admin

''' <summary>
''' Interaction logic for RolesEdit.xaml
''' </summary>
Partial Public Class RolesEdit
  Inherits EditForm

  Public Sub New()

    InitializeComponent()

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("RoleList"), Csla.Wpf.CslaDataProvider)
    AddHandler dp.DataChanged, AddressOf dp_DataChanged

  End Sub

  Private Sub dp_DataChanged(ByVal sender As Object, ByVal e As EventArgs)
    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(sender, Csla.Wpf.CslaDataProvider)
    If Not dp.Error Is Nothing Then
      MessageBox.Show(dp.Error.ToString(), "Data error", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    End If
  End Sub

  Private Sub RemoveItem(ByVal sender As Object, ByVal e As EventArgs)
    Dim btn As Button = CType(sender, Button)
    Dim id As Integer = CInt(Fix(btn.Tag))
    Dim roles As Roles = CType((CType(Me.FindResource("RoleList"), Csla.Wpf.CslaDataProvider)).Data, Roles)
    For Each role As Role In roles
      If role.Id = id Then
        roles.Remove(role)
        Exit For
      End If
    Next role
  End Sub

  Protected Overrides Sub ApplyAuthorization()
    Me.AuthPanel.Refresh()
    If Roles.CanEditObject() Then
      Me.RolesListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbTemplate"), DataTemplate)
      Me.AddItemButton.IsEnabled = True
    Else
      Me.RolesListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbroTemplate"), DataTemplate)
      Me.AddItemButton.IsEnabled = False
      CType(Me.FindResource("RoleList"), Csla.Wpf.CslaDataProvider).Cancel()
    End If
  End Sub

End Class
