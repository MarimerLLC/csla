using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProjectTracker.Library.Admin;

namespace PTWin
{
  public partial class RolesEdit : WinPart
  {

    private Roles _roles;

    public RolesEdit()
    {
      InitializeComponent();
    }

    private void RolesEdit_Load(object sender, EventArgs e)
    {
      try
      {
        _roles = Roles.GetRoles();
      }
      catch (Csla.DataPortalException ex)
      {
        MessageBox.Show(ex.BusinessException.ToString(), 
          "Data load error", MessageBoxButtons.OK, 
          MessageBoxIcon.Exclamation);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(),
          "Data load error", MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation);
      }
      this.rolesBindingSource.DataSource = _roles;
    }

    protected internal override object GetIdValue()
    {
      return "Edit Roles";
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      this.rolesBindingSource.RaiseListChangedEvents = false;
      Roles temp = _roles.Clone();
      try
      {
        _roles = temp.Save();
        // rebind the UI
        this.rolesBindingSource.DataSource = null;
        this.rolesBindingSource.DataSource = _roles;
      }
      catch (Csla.DataPortalException ex)
      {
        MessageBox.Show(ex.BusinessException.ToString(),
          "Error saving", MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Error saving",
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      finally
      {
        this.rolesBindingSource.RaiseListChangedEvents = true;
      }
      this.Close();
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void RolesEdit_CurrentPrincipalChanged(
      object sender, EventArgs e)
    {
      if (!Roles.CanEditObject())
        this.Close();
    }
  }
}
