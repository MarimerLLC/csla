using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectTracker.Library;
using Csla.Security;
using Csla.Configuration;
using Csla;

namespace PTWin
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
      _main = this;
    }

    private static MainForm _main;

    internal static MainForm Instance
    {
      get { return _main; }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      if (Csla.ApplicationContext.AuthenticationType == "Windows")
      {
        AppDomain.CurrentDomain.SetPrincipalPolicy(
          System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
        ApplyAuthorizationRules();
      }
      else
      {
        DoLogin();
      }
      if (DocumentCount == 0)
        DocumentsToolStripDropDownButton.Enabled = false;

      // initialize cache of role list
      var task = RoleList.CacheListAsync();
    }

    #region Projects

    private void NewProjectToolStripMenuItem_Click(
      object sender, EventArgs e)
    {
      using (StatusBusy busy = 
        new StatusBusy("Creating project..."))
      {
        AddWinPart(new ProjectEdit(ProjectTracker.Library.ProjectEdit.NewProject()));
      }
    }

    private void EditProjectToolStripMenuItem_Click(
      object sender, EventArgs e)
    {
      using (ProjectSelect dlg = new ProjectSelect())
      {
        dlg.Text = "Edit Project";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          ShowEditProject(dlg.ProjectId);
        }
      }
    }

    public void ShowEditProject(int projectId)
    {
      // see if this project is already loaded
      foreach (Control ctl in Panel1.Controls)
      {
        if (ctl is ProjectEdit)
        {
          ProjectEdit part = (ProjectEdit)ctl;
          if (part.Project.Id.Equals(projectId))
          {
            // project already loaded so just
            // display the existing winpart
            ShowWinPart(part);
            return;
          }
        }
      }

      // the project wasn't already loaded
      // so load it and display the new winpart
      using (StatusBusy busy = new StatusBusy("Loading project..."))
      {
        try
        {
          AddWinPart(new ProjectEdit(ProjectTracker.Library.ProjectEdit.GetProject(projectId)));
        }
        catch (Csla.DataPortalException ex)
        {
          MessageBox.Show(ex.BusinessException.ToString(),
            "Error loading", MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString(),
            "Error loading", MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        }
      }
    }

    private async void DeleteProjectToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      dlg.Text = "Delete Project";
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        // get the project id
        var projectId = dlg.ProjectId;

        if (MessageBox.Show("Are you sure?", "Delete project",
          MessageBoxButtons.YesNo, MessageBoxIcon.Question,
          MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        {
          using (StatusBusy busy = new StatusBusy("Deleting project..."))
          {
            try
            {
              await ProjectTracker.Library.ProjectEdit.DeleteProjectAsync(projectId);
            }
            catch (Csla.DataPortalException ex)
            {
              MessageBox.Show(ex.BusinessException.ToString(),
                "Error deleting", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
              MessageBox.Show(ex.ToString(),
                "Error deleting", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
          }
        }
      }
    }

    #endregion

    #region Resources

    private void NewResourceToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Creating resource..."))
      {
        AddWinPart(new ResourceEdit(ProjectTracker.Library.ResourceEdit.NewResourceEdit()));
      }
    }

    private void EditResourceToolStripMenuItem_Click(
      object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      dlg.Text = "Edit Resource";
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        // get the resource id
        ShowEditResource(dlg.ResourceId);
      }
    }

    public void ShowEditResource(int resourceId)
    {
      // see if this resource is already loaded
      foreach (Control ctl in Panel1.Controls)
      {
        if (ctl is ResourceEdit)
        {
          ResourceEdit part = (ResourceEdit)ctl;
          if (part.Resource.Id.Equals(resourceId))
          {
            // resource already loaded so just
            // display the existing winpart
            ShowWinPart(part);
            return;
          }
        }
      }

      // the resource wasn't already loaded
      // so load it and display the new winpart
      using (StatusBusy busy = new StatusBusy("Loading resource..."))
      {
        try
        {
          AddWinPart(new ResourceEdit(ProjectTracker.Library.ResourceEdit.GetResourceEdit(resourceId)));
        }
        catch (Csla.DataPortalException ex)
        {
          MessageBox.Show(ex.BusinessException.ToString(),
            "Error loading", MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString(),
            "Error loading", MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        }
      }
    }

    private void DeleteResourceToolStripMenuItem_Click(
      object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      dlg.Text = "Delete Resource";
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        // get the resource id
        int resourceId = dlg.ResourceId;

        if (MessageBox.Show("Are you sure?", "Delete resource",
          MessageBoxButtons.YesNo, MessageBoxIcon.Question,
          MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        {
          using (StatusBusy busy = 
            new StatusBusy("Deleting resource..."))
          {
            try
            {
              ProjectTracker.Library.ResourceEdit.DeleteResourceEdit(resourceId);
            }
            catch (Csla.DataPortalException ex)
            {
              MessageBox.Show(ex.BusinessException.ToString(), 
                "Error deleting", MessageBoxButtons.OK, 
                MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
              MessageBox.Show(ex.ToString(), 
                "Error deleting", MessageBoxButtons.OK, 
                MessageBoxIcon.Exclamation);
            }
          }
        }
      }
    }

    #endregion

    #region Roles

    private void EditRolesToolStripMenuItem_Click(
      object sender, EventArgs e)
    {
      // see if this form is already loaded
      foreach (Control ctl in Panel1.Controls)
      {
        if (ctl is RolesEdit)
        {
          ShowWinPart((WinPart)ctl);
          return;
        }
      }

      // it wasn't already loaded, so show it.
      AddWinPart(new RolesEdit());
    }

    #endregion

    #region ApplyAuthorizationRules

    private void ApplyAuthorizationRules()
    {
      // Project menu
      this.NewProjectToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ProjectEdit));
      this.EditProjectToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ProjectTracker.Library.ProjectEdit));
      if (Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ProjectEdit)))
        this.EditProjectToolStripMenuItem.Text = 
          "Edit project";
      else
        this.EditProjectToolStripMenuItem.Text = 
          "View project";
      this.DeleteProjectToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectTracker.Library.ProjectEdit));

      // Resource menu
      this.NewResourceToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(ProjectTracker.Library.ResourceEdit));
      this.EditResourceToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(ProjectTracker.Library.ResourceEdit));
      if (Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ResourceEdit)))
        this.EditResourceToolStripMenuItem.Text = 
          "Edit resource";
      else
        this.EditResourceToolStripMenuItem.Text = 
          "View resource";
      this.DeleteResourceToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(ProjectTracker.Library.ResourceEdit));

      // Admin menu
      this.EditRolesToolStripMenuItem.Enabled =
        Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.Admin.RoleEditBindingList));
    }

    #endregion

    #region Login/Logout

    private void LoginToolStripButton_Click(
      object sender, EventArgs e)
    {
      DoLogin();
    }

    private void DoLogin()
    {
      ProjectTracker.Library.Security.PTPrincipal.Logout();

      if (this.LoginToolStripButton.Text == "Login")
      {
        LoginForm loginForm = new LoginForm();
        loginForm.ShowDialog(this);
      }

      System.Security.Principal.IPrincipal user =
        Csla.ApplicationContext.User;

      if (user.Identity.IsAuthenticated)
      {
        this.LoginToolStripLabel.Text = "Logged in as " +
          user.Identity.Name;
        this.LoginToolStripButton.Text = "Logout";
      }
      else
      {
        this.LoginToolStripLabel.Text = "Not logged in";
        this.LoginToolStripButton.Text = "Login";
      }

      // reset menus, etc.
      ApplyAuthorizationRules();

      // notify all documents
      List<object> tmpList = new List<object>();
      foreach (var ctl in Panel1.Controls)
        tmpList.Add(ctl);
      foreach (var ctl in tmpList)
        if (ctl is WinPart)
          ((WinPart)ctl).OnCurrentPrincipalChanged(this, EventArgs.Empty);
    }

    #endregion

    #region WinPart handling

    /// <summary>
    /// Add a new WinPart control to the
    /// list of available documents and
    /// make it the active WinPart.
    /// </summary>
    /// <param name="part">The WinPart control to add and display.</param>
    private void AddWinPart(WinPart part)
    {
      part.CloseWinPart += new EventHandler(CloseWinPart);
      part.BackColor = toolStrip1.BackColor;
      Panel1.Controls.Add(part);
      this.DocumentsToolStripDropDownButton.Enabled = true;
      ShowWinPart(part);
    }

    /// <summary>
    /// Make the specified WinPart the 
    /// active, displayed control.
    /// </summary>
    /// <param name="part">The WinPart control to display.</param>
    private void ShowWinPart(WinPart part)
    {
      part.Dock = DockStyle.Fill;
      part.Visible = true;
      part.BringToFront();
      this.Text = "Project Tracker - " + part.ToString();
      PopulateDocuments();
    }

    /// <summary>
    /// Populate the Documents dropdown list.
    /// </summary>
    private void DocumentsToolStripDropDownButton_DropDownOpening(
      object sender, EventArgs e)
    {
      PopulateDocuments();
    }

    /// <summary>
    /// Populate the Documents dropdown list.
    /// </summary>
    private void PopulateDocuments()
    {
      ToolStripItemCollection items = 
        DocumentsToolStripDropDownButton.DropDownItems;
      foreach (ToolStripItem item in items)
        item.Click -= new EventHandler(DocumentClick);
      items.Clear();

      foreach (Control ctl in Panel1.Controls)
        if (ctl is WinPart)
        {
          ToolStripItem item = new ToolStripMenuItem();
          item.Text = ((WinPart)ctl).ToString();
          item.Tag = ctl;
          item.Click += new EventHandler(DocumentClick);
          items.Add(item);
        }
    }

    /// <summary>
    /// Make selected WinPart the active control.
    /// </summary>
    private void DocumentClick(object sender, EventArgs e)
    {
      WinPart ctl = (WinPart)((ToolStripItem)sender).Tag;
      ShowWinPart(ctl);
    }

    /// <summary>
    /// Gets a count of the number of loaded
    /// documents.
    /// </summary>
    public int DocumentCount
    {
      get
      {
        int count = 0;
        foreach (Control ctl in Panel1.Controls)
          if (ctl is WinPart)
            count++;
        return count;
      }
    }

    /// <summary>
    /// Handles event from WinPart when that
    /// WinPart is closing.
    /// </summary>
    private void CloseWinPart(object sender, EventArgs e)
    {
      WinPart part = (WinPart)sender;
      part.CloseWinPart -= new EventHandler(CloseWinPart);
      part.Visible = false;
      Panel1.Controls.Remove(part);
      part.Dispose();
      PopulateDocuments();

      if (DocumentCount == 0)
      {
        this.DocumentsToolStripDropDownButton.Enabled = false;
        this.Text = "Project Tracker";
      }
      else
      {
        // Find the first WinPart control and set
        // the main form's Text property accordingly.
        // This works because the first WinPart 
        // is the active one.
        foreach (Control ctl in Panel1.Controls)
        {
          if (ctl is WinPart)
          {
            this.Text = "Project Tracker - " + ((WinPart)ctl).ToString();
            break;
          }
        }
      }
    }

    #endregion
  }
}