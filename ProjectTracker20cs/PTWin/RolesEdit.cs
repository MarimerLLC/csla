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
      _roles = Roles.GetRoles();
      this.RolesBindingSource.DataSource = _roles;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      this.RolesBindingSource.RaiseListChangedEvents = false;
      Roles old = _roles.Clone();
      try
      {
        _roles = _roles.Save();
      }
      catch (Exception ex)
      {
        _roles = old;
        MessageBox.Show(ex.ToString(), "Error saving",
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      this.RolesBindingSource.DataSource = _roles;
      this.RolesBindingSource.RaiseListChangedEvents = true;
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
