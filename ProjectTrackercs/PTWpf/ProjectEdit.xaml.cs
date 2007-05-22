using System;
using System.Linq;
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
using ProjectTracker.Library;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for ProjectEdit.xaml
  /// </summary>

  public partial class ProjectEdit : System.Windows.Controls.Page, IRefresh
  {
    public ProjectEdit()
    {
      InitializeComponent();
    }

    public ProjectEdit(Project project)
      : this()
    {
      SetTitle(project);

      project.BeginEdit();

      this.DataContext = project;

      ApplyAuthorization();
    }

    void SetTitle(Project project)
    {
      if (project.IsNew)
        this.Title = string.Format("Project: {0}", "<new>");
      else
        this.Title = string.Format("Project: {0}", project.Name);
    }

    void SaveObject(object sender, EventArgs e)
    {
      Project project = (Project)this.DataContext;
      try
      {
        Project tmp = project.Clone();
        tmp.ApplyEdit();
        project = tmp.Save();
        project.BeginEdit();
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
        this.DataContext = null;
        this.DataContext = project;
        SetTitle(project);
      }
    }

    void CancelChanges(object sender, EventArgs e)
    {
      Project project = (Project)this.DataContext;
      project.CancelEdit();
      project.BeginEdit();
    }

    void ShowResource(object sender, EventArgs e)
    {
      ProjectTracker.Library.ProjectResource item =
        (ProjectTracker.Library.ProjectResource)ResourceListBox.SelectedItem;

      if (item != null)
      {
        ResourceEdit frm = new ResourceEdit(Resource.GetResource(item.ResourceId));
        MainPage.ShowPage(frm);
      }
    }

    void TestClick(object sender, EventArgs e)
    {
      this.NameTextBox.Text = string.Empty;
    }

    void ApplyAuthorization()
    {
      this.AuthPanel.Refresh();
      if (Project.CanEditObject())
        this.ResourceListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
      else
        this.ResourceListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
    }

    #region IRefresh Members

    /// <summary>
    /// Called by MainPage when the currently
    /// logged in user changes.
    /// </summary>
    public void Refresh()
    {
      ApplyAuthorization();
      Project project = (Project)this.DataContext;
      project.CancelEdit();
      if (Project.CanEditObject())
        project.BeginEdit();
    }

    #endregion
  }
}