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
  /// Interaction logic for ResourceEdit.xaml
  /// </summary>

  public partial class ResourceEdit : System.Windows.Controls.Page, IRefresh
  {
    public ResourceEdit()
    {
      InitializeComponent();
    }

    public ResourceEdit(Resource resource)
      : this()
    {
      SetTitle(resource);

      resource.BeginEdit();

      this.DataContext = resource;

      ApplyAuthorization();
    }

    void SetTitle(Resource resource)
    {
      if (resource.IsNew)
        this.Title = string.Format("Resource: {0}", "<new>");
      else
        this.Title = string.Format("Resource: {0}", resource.FullName);
    }

    void SaveObject(object sender, EventArgs e)
    {
      Resource resource = (Resource)this.DataContext;
      try
      {
        Resource tmp = resource.Clone();
        tmp.ApplyEdit();
        resource = tmp.Save();
        resource.BeginEdit();
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
        this.DataContext = resource;
        SetTitle(resource);
      }
    }

    void CancelChanges(object sender, EventArgs e)
    {
      Resource resource = (Resource)this.DataContext;
      resource.CancelEdit();
      resource.BeginEdit();
    }

    void ShowProject(object sender, EventArgs e)
    {
      ProjectTracker.Library.ResourceAssignment item =
        (ProjectTracker.Library.ResourceAssignment)ProjectListBox.SelectedItem;

      if (item != null)
      {
        ProjectEdit frm = new ProjectEdit(Project.GetProject(item.ProjectId));
        MainPage.ShowPage(frm);
      }
    }

    void Assign(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      if ((bool)dlg.ShowDialog())
      {
        Guid id = dlg.ProjectId;
        Resource resource = (Resource)this.DataContext;
        try
        {
          resource.Assignments.AssignTo(id);
        }
        catch (Exception ex)
        {
          MessageBox.Show(
            ex.Message, 
            "Assignment error", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
        }
      }
    }

    void Unassign(object sender, EventArgs e)
    {
      Button btn = (Button)sender;
      Guid id = (Guid)btn.Tag;
      Resource resource = (Resource)this.DataContext;
      resource.Assignments.Remove(id);
    }

    void ApplyAuthorization()
    {
      this.AuthPanel.Refresh();
      if (Resource.CanEditObject())
      {
        this.ProjectListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
        this.AssignButton.IsEnabled = true;
      }
      else
      {
        this.ProjectListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
        this.AssignButton.IsEnabled = false;
      }
    }

    #region IRefresh Members

    public void Refresh()
    {
      ApplyAuthorization();
      Resource resource = (Resource)this.DataContext;
        resource.CancelEdit();
      if (Resource.CanEditObject())
        resource.BeginEdit();
    }

    #endregion
  }
}