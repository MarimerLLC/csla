using System;
using System.Windows.Forms;
using BusinessLibrary;
using Csla;

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
      this.cslaActionExtender1.ResetActionBehaviors(BusinessLibrary.Order.NewOrder());
    }

    private void button1_Click(object sender, EventArgs e)
    {
      try
      {
        // Delete method will always throw a non-serializable exception
        Order.Delete(101);
      }
      catch (DataPortalException ex)
      {
        MessageBox.Show(ex.BusinessException.ToString(), "Server error:" + ex.BusinessException.Message);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Client error:" + ex.Message);
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      try
      {
        // Delete method will always throw a non-serializable exception
        Order.Delete(202);
      }
      catch (DataPortalException ex)
      {
        MessageBox.Show(ex.BusinessException.ToString(), "Server error:" + ex.BusinessException.Message);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Client error:" + ex.Message);
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      try
      {
        var order = Order.GetOrder(1);
      }
      catch (DataPortalException ex)
      {
        MessageBox.Show(ex.ToString(), "Server error:" + ex.Message);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Client error:" + ex.Message);
      }
    }

  }
}
