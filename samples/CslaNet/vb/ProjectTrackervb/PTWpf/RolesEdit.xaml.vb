Imports ProjectTracker.Library.Admin

''' <summary>
''' Interaction logic for RolesEdit.xaml
''' </summary>
Partial Public Class RolesEdit
  Inherits EditForm

  Public Sub New()

    InitializeComponent()

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("RoleList"), Csla.Wpf.CslaDataProvider)
    AddHandler dp.DataChanged, AddressOf MyBase.DataChanged

  End Sub

  Protected Overrides Sub ApplyAuthorization()
    If Roles.CanEditObject() Then
      Me.RolesListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbTemplate"), DataTemplate)
    Else
      Me.RolesListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbroTemplate"), DataTemplate)
      CType(Me.FindResource("RoleList"), Csla.Wpf.CslaDataProvider).Cancel()
    End If
  End Sub

End Class
