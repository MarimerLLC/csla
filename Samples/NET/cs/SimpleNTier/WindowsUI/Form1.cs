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
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //var obj = BusinessLibrary.Order.NewOrder();
      var obj =  BusinessLibrary.Order.GetOrder(441);
      this.cslaActionExtender1.ResetActionBehaviors(obj);
    }
  }
}
