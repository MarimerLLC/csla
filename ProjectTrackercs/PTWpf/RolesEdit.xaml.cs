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
      dp.DataChanged += new EventHandler(base.DataChanged);
    }

    protected override void ApplyAuthorization()
    {
      if (Csla.Security.AuthorizationRules.CanEditObject(typeof(Roles)))
      {
        this.RolesListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
      }
      else
      {
        this.RolesListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
        ((Csla.Wpf.CslaDataProvider)this.FindResource("RoleList")).Cancel();
      }
    }
  }
}
