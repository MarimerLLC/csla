using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectTracker.Library.Admin;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for RolesEdit.xaml
  /// </summary>
  public partial class RolesEdit : EditForm
  {
    public RolesEdit()
    {
      InitializeComponent();
      Csla.Wpf.CslaDataProvider dp = this.FindResource("RoleList") as Csla.Wpf.CslaDataProvider;
      dp.DataChanged += new EventHandler(dp_DataChanged);
    }

    void dp_DataChanged(object sender, EventArgs e)
    {
      Csla.Wpf.CslaDataProvider dp = sender as Csla.Wpf.CslaDataProvider;
      if (dp.Error != null)
        MessageBox.Show(dp.Error.ToString(), "Data error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    }

    void RemoveItem(object sender, EventArgs e)
    {
      Button btn = (Button)sender;
      int id = (int)btn.Tag;
      Roles roles = (Roles)((Csla.Wpf.CslaDataProvider)this.FindResource("RoleList")).Data;
      foreach (Role role in roles)
        if (role.Id == id)
        {
          roles.Remove(role);
          break;
        }
    }

    protected override void ApplyAuthorization()
    {
      this.AuthPanel.Refresh();
      if (Roles.CanEditObject())
      {
        this.RolesListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
        this.AddItemButton.IsEnabled = true;
      }
      else
      {
        this.RolesListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
        this.AddItemButton.IsEnabled = false;
        ((Csla.Wpf.CslaDataProvider)this.FindResource("RoleList")).Cancel();
      }
    }
  }
}
