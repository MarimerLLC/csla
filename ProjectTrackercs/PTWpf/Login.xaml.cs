using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PTWpf
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public partial class Login : System.Windows.Window
  {
    public Login()
    {
      InitializeComponent();

      this.UsernameTextBox.Focus();
    }

    private bool _result;

    public bool Result
    {
      get { return _result; }
      set { _result = value; }
    }

    void LoginButton(object sender, EventArgs e)
    {
      _result = true;
      this.Close();
    }

    void CancelButton(object sender, EventArgs e)
    {
      _result = false;
      this.Close();
    }
  }
}