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
  public partial class ProjectEdit : EditForm
  {
    private Guid _projectId;

    public ProjectEdit()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(ProjectEdit_Loaded);
      Csla.Wpf.CslaDataProvider dp = this.FindResource("Project") as Csla.Wpf.CslaDataProvider;
      dp.DataChanged += new EventHandler(DataChanged);
    }

    public ProjectEdit(Guid id)
      : this()
    {
      _projectId = id;
    }

    void ProjectEdit_Loaded(object sender, RoutedEventArgs e)
    {
      Csla.Wpf.CslaDataProvider dp = this.FindResource("Project") as Csla.Wpf.CslaDataProvider;
      using (dp.DeferRefresh())
      {
        dp.FactoryParameters.Clear();
        if (_projectId.Equals(Guid.Empty))
        {
          dp.FactoryMethod = "NewProject";
        }
        else
        {
          dp.FactoryMethod = "GetProject";
          dp.FactoryParameters.Add(_projectId);
        }
      }
      if (dp.Data != null)
        SetTitle((Project)dp.Data);
      else
        MainForm.ShowControl(null);
    }

    void SetTitle(Project project)
    {
      if (project.IsNew)
        this.Title = string.Format("Project: {0}", "<new>");
      else
        this.Title = string.Format("Project: {0}", project.Name);
    }

    void ShowResource(object sender, EventArgs e)
    {
      ProjectTracker.Library.ProjectResource item =
        (ProjectTracker.Library.ProjectResource)ResourceListBox.SelectedItem;

      if (item != null)
      {
        ResourceEdit frm = new ResourceEdit(item.ResourceId);
        MainForm.ShowControl(frm);
      }
    }

    protected override void ApplyAuthorization()
    {
      this.AuthPanel.Refresh();
      if (Project.CanEditObject())
      {
        this.ResourceListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
        this.AssignButton.IsEnabled = true;
      }
      else
      {
        this.ResourceListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
        ((Csla.Wpf.CslaDataProvider)this.FindResource("Project")).Cancel();
        this.AssignButton.IsEnabled = false;
      }
    }

    void Assign(object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      if ((bool)dlg.ShowDialog())
      {
        int id = dlg.ResourceId;
        Project project = (Project)((Csla.Wpf.CslaDataProvider)this.FindResource("Project")).Data;
        try
        {
          project.Resources.Assign(id);
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
      int id = (int)btn.Tag;
      Project project = (Project)((Csla.Wpf.CslaDataProvider)this.FindResource("Project")).Data;
      project.Resources.Remove(id);
    }
  }
}