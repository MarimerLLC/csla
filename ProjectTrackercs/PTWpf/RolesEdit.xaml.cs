using System;
using System.Collections.Generic;
using System.Linq;
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
  public partial class RolesEdit : System.Windows.Controls.Page, IRefresh
  {
    public RolesEdit()
    {
      InitializeComponent();

      Roles roles = Roles.GetRoles();
      if (Roles.CanEditObject())
        roles.BeginEdit();

      this.DataContext = roles;

      UpdateAuthorization();
    }

    void SaveObject(object sender, EventArgs e)
    {
      Roles roles = (Roles)this.DataContext;
      try
      {
        Roles tmp = roles.Clone();
        tmp.ApplyEdit();
        roles = tmp.Save();
        roles.BeginEdit();
      }
      catch (Csla.DataPortalException ex)
      {
        MessageBox.Show(ex.BusinessException.Message, "Data error");
      }
      catch (Csla.Validation.ValidationException ex)
      {
        MessageBox.Show(ex.Message, "Data validation error");
      }
      catch (System.Security.SecurityException ex)
      {
        MessageBox.Show(ex.Message, "Security error");
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Unexpected error");
      }
      finally
      {
        this.DataContext = roles;
      }
    }

    void CancelChanges(object sender, EventArgs e)
    {
      Roles roles = (Roles)this.DataContext;
      roles.CancelEdit();
      roles.BeginEdit();
    }

    void AddItem(object sender, EventArgs e)
    {
      Roles roles = (Roles)this.DataContext;
      roles.AddNew();
    }

    void RemoveItem(object sender, EventArgs e)
    {
      Button btn = (Button)sender;
      int id = (int)btn.Tag;
      Roles roles = (Roles)this.DataContext;
      foreach (Role role in roles)
        if (role.Id == id)
        {
          roles.Remove(role);
          break;
        }
    }

    void UpdateAuthorization()
    {
      this.AuthPanel.Refresh();
      if (Roles.CanEditObject())
      {
        this.RolesListBox.IsEnabled = true;
        this.AddItemButton.IsEnabled = true;
      }
      else
      {
        this.RolesListBox.IsEnabled = false;
        this.AddItemButton.IsEnabled = false;
      }
    }

    #region IRefresh Members

    public void Refresh()
    {
      UpdateAuthorization();
      Roles roles = (Roles)this.DataContext;
      roles.CancelEdit();
      if (Roles.CanEditObject())
        roles.BeginEdit();
    }

    #endregion
  }
}
