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

    private void Form1_Load(object sender, EventArgs e)
    {
      try
      {
        //var obj = BusinessLibrary.Order.NewOrder();
        var obj = BusinessLibrary.Order.GetOrder(441);
        this.cslaActionExtender1.ResetActionBehaviors(obj);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}
