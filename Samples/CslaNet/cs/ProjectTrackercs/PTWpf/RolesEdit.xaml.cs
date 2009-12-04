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
    }

    protected override void ApplyAuthorization()
    {
      var dp = (Csla.Wpf.CslaDataProvider)this.FindResource("RoleList");
      dp.Rebind();
      if (!Csla.Security.AuthorizationRules.CanEditObject(dp.ObjectType))
        dp.Cancel();
    }
  }
}
