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

  public partial class ResourceEdit : System.Windows.Controls.Page
  {
    public ResourceEdit()
    {
      InitializeComponent();
    }

    public ResourceEdit(Resource resource)
      : this()
    {
    }
  }
}