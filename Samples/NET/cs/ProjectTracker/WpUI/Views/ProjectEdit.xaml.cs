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

    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      var viewmodel = this.DataContext as ViewModels.ProjectEdit;
      if (viewmodel != null)
        viewmodel.Close();
    }
  }
}