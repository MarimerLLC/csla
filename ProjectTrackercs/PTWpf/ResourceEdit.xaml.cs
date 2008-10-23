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
using Csla.Security;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for ResourceEdit.xaml
  /// </summary>
  public partial class ResourceEdit : EditForm
  {
    private int _resourceId;
    private Csla.Wpf.CslaDataProvider _dp;

    public ResourceEdit()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(ResourceEdit_Loaded);
      _dp = this.FindResource("Resource") as Csla.Wpf.CslaDataProvider;
      //_dp.DataChanged += new EventHandler(DataChanged);
    }

    public ResourceEdit(int resourceId)
      : this()
    {
      _resourceId = resourceId;
    }

    void ResourceEdit_Loaded(object sender, RoutedEventArgs e)
    {
      using (_dp.DeferRefresh())
      {
        _dp.FactoryParameters.Clear();
        if (_resourceId == 0)
        {
          _dp.FactoryMethod = "NewResource";
        }
        else
        {
          _dp.FactoryMethod = "GetResource";
          _dp.FactoryParameters.Add(_resourceId);
        }
      }
      if (_dp.Data != null)
        SetTitle((Resource)_dp.Data);
      else
        MainForm.ShowControl(null);
    }

    void SetTitle(Resource resource)
    {
      if (resource.IsNew)
        this.Title = string.Format("Resource: {0}", "<new>");
      else
        this.Title = string.Format("Resource: {0}", resource.FullName);
    }

    private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Parameter != null)
      {
        ProjectEdit frm = new ProjectEdit(new Guid(e.Parameter.ToString()));
        MainForm.ShowControl(frm);
      }
    }

    private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    { e.CanExecute = true; }

    protected override void ApplyAuthorization()
    {
      _dp.Rebind();
      //this.AuthPanel.Refresh();
      if (AuthorizationRules.CanEditObject(typeof(Resource)))
      {
        this.ProjectListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbTemplate"];
      }
      else
      {
        this.ProjectListBox.ItemTemplate = (DataTemplate)this.MainGrid.Resources["lbroTemplate"];
        _dp.Cancel();
      }
    }

    void Assign(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      if ((bool)dlg.ShowDialog())
      {
        Guid id = dlg.ProjectId;
        Resource resource = (Resource)_dp.Data;
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
  }
}