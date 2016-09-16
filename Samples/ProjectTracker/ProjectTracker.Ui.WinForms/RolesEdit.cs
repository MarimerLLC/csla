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

    private RoleEditList _roles;

    public RolesEdit()
    {
      InitializeComponent();
    }

    private void RolesEdit_Load(object sender, EventArgs e)
    {
      try
      {
        _roles = RoleEditList.GetRoles();
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
      if (_roles != null)
        this.rolesBindingSource.DataSource = _roles;
    }

    protected internal override object GetIdValue()
    {
      return "Edit Roles";
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      // stop the flow of events
      this.rolesBindingSource.RaiseListChangedEvents = false;
      
      // commit edits in memory and unbind
      UnbindBindingSource(this.rolesBindingSource, true, true);

      try
      {
        _roles = _roles.Save();
        this.Close();
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
        this.rolesBindingSource.DataSource = _roles;

        this.rolesBindingSource.RaiseListChangedEvents = true;
        this.rolesBindingSource.ResetBindings(false);
      }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void RolesEdit_CurrentPrincipalChanged(
      object sender, EventArgs e)
    {
      if (!Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(RoleEditList)))
        this.Close();
    }
  }
}
