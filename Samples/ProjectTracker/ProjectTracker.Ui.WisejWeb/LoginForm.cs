using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using Wisej.Web;

namespace PTWisej
{
  public partial class LoginForm : Form
  {
    public LoginForm()
    {
      InitializeComponent();
    }

    private void OK_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = 
        new StatusBusy("Verifying credentials..."))
      {
        ProjectTracker.Library.Security.PTPrincipal.Login(
          this.UsernameTextBox.Text, this.PasswordTextBox.Text);
      }
      this.Close();
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {
      this.UsernameTextBox.Focus();
    }
  }
}