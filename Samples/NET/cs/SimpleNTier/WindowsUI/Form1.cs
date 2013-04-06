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
      //this.cslaActionExtender1.ResetActionBehaviors(BusinessLibrary.Order.NewOrder());
      this.cslaActionExtender1.ResetActionBehaviors(BusinessLibrary.Order.GetOrder(441));
    }
  }
}
