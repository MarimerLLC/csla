using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace WpUI.Views
{
  public partial class ProjectEdit : PhoneApplicationPage
  {
    public ProjectEdit()
    {
      InitializeComponent();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      var viewmodel = (ViewModels.ProjectEdit)this.DataContext;

      // copy lostfocus-based view values to model
      var project = viewmodel.Model.Project;
      project.Name = NameTextBox.Text;
      if (!string.IsNullOrWhiteSpace(StartedTextBox.Text))
        project.Started = DateTime.Parse(StartedTextBox.Text);
      if (!string.IsNullOrWhiteSpace(EndedTextBox.Text))
        project.Ended = DateTime.Parse(EndedTextBox.Text);
      project.Description = DescriptionTextBox.Text;

      viewmodel.Save();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      var viewmodel = (ViewModels.ProjectEdit)this.DataContext;
      viewmodel.Close();
    }
  }
}