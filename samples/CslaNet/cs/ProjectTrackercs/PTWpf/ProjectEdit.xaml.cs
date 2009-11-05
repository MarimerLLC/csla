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
using ProjectTracker.Library;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for ProjectEdit.xaml
  /// </summary>
  public partial class ProjectEdit : EditForm
  {
    private Guid _projectId;
    private Csla.Wpf.CslaDataProvider _dp;

    public ProjectEdit()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(ProjectEdit_Loaded);
      _dp = this.FindResource("Project") as Csla.Wpf.CslaDataProvider;
      _dp.DataChanged += new EventHandler(DataChanged);
    }

    public ProjectEdit(Guid id)
      : this()
    {
      _projectId = id;
    }

    void ProjectEdit_Loaded(object sender, RoutedEventArgs e)
    {
      using (_dp.DeferRefresh())
      {
        _dp.FactoryParameters.Clear();
        if (_projectId.Equals(Guid.Empty))
        {
          _dp.FactoryMethod = "NewProject";
        }
        else
        {
          _dp.FactoryMethod = "GetProject";
          _dp.FactoryParameters.Add(_projectId);
        }
      }
      if (_dp.Data != null)
        SetTitle((Project)_dp.Data);
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

    protected override void ApplyAuthorization()
    {
      if (!Csla.Security.AuthorizationRules.CanEditObject(typeof(Project)))
        _dp.Cancel();
      _dp.Rebind();
    }

    private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Parameter != null)
      {
        ResourceEdit frm = new ResourceEdit(Convert.ToInt32(e.Parameter));
        MainForm.ShowControl(frm);
      }
    }

    private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    { e.CanExecute = true; }

    void Assign(object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      if ((bool)dlg.ShowDialog())
      {
        int id = dlg.ResourceId;
        Project project = (Project)_dp.Data;
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
  }
}