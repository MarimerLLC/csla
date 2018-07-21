using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsUI
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
      try
      {
        //var obj = await BusinessLibrary.Order.NewOrderAsync();
        var obj = await BusinessLibrary.Order.GetOrderAsync(441);
        cslaActionExtender1.ResetActionBehaviors(obj);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}
