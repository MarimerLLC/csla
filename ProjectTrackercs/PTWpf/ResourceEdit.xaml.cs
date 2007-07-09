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
  /// Interaction logic for ResourceEdit.xaml
  /// </summary>
  public partial class ResourceEdit : EditForm
  {
    private int _resourceId;

    public ResourceEdit()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(ResourceEdit_Loaded);
      Csla.Wpf.CslaDataProvider dp = this.FindResource("Resource") as Csla.Wpf.CslaDataProvider;
      dp.DataChanged += new EventHandler(DataChanged);
    }

    public ResourceEdit(int resourceId)
      : this()
    {
      _resourceId = resourceId;
    }

    void ResourceEdit_Loaded(object sender, RoutedEventArgs e)
    {
      Csla.Wpf.CslaDataProvider dp = this.FindResource("Resource") as Csla.Wpf.CslaDataProvider;
      using (dp.DeferRefresh())
      {
        dp.FactoryParameters.Clear();
        if (_resourceId == 0)
        {
          dp.FactoryMethod = "NewResource";
        }
        else
        {
          dp.FactoryMethod = "GetResource";
          dp.FactoryParameters.Add(_resourceId);
        }
      }
      if (dp.Data != null)
        SetTitle((Resource)dp.Data);
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

    void ShowProject(object sender, EventArgs e)
    {
      ProjectTracker.Library.ResourceAssignment item =
        (ProjectTracker.Library.ResourceAssignment)ProjectListBox.SelectedItem;

      if (item != null)
      {
        ProjectEdit frm = new ProjectEdit(item.ProjectId);
        MainForm.ShowControl(frm);
      }
    }

    protected override void ApplyAuthorization()
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
        ((Csla.Wpf.CslaDataProvider)this.FindResource("Resource")).Cancel();
        this.AssignButton.IsEnabled = false;
      }
    }

    void Assign(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      if ((bool)dlg.ShowDialog())
      {
        Guid id = dlg.ProjectId;
        Resource resource = (Resource)((Csla.Wpf.CslaDataProvider)this.FindResource("Resource")).Data;
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
      Resource resource = (Resource)((Csla.Wpf.CslaDataProvider)this.FindResource("Resource")).Data;
      resource.Assignments.Remove(id);
    }
  }
}