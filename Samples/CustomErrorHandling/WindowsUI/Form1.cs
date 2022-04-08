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
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Order>>();
      try
      {
        var result = portal.Create();
        this.cslaActionExtender1.ResetActionBehaviors(result);
      }
      catch (DataPortalException ex)
      {
        MessageBox.Show(ex.Message, "Initial data load failure");
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Initial data load failure");
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Order>>();
      try
      {
        // Delete method will always throw a non-serializable exception
        portal.Delete(101);
      }
      catch (DataPortalException ex)
      {
        if (ex.BusinessException != null)
          MessageBox.Show(ex.BusinessException.ToString(), "Server error:" + ex.BusinessException.Message);
        else
          MessageBox.Show(ex.ToString(), "Server error:" + ex.Message);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Client error:" + ex.Message);
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Order>>();
      try
      {
        // Delete method will always throw a non-serializable exception
        portal.Delete(202);
      }
      catch (DataPortalException ex)
      {
        if (ex.BusinessException != null)
          MessageBox.Show(ex.BusinessException.ToString(), "Server error:" + ex.BusinessException.Message);
        else
          MessageBox.Show(ex.ToString(), "Server error:" + ex.Message);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Client error:" + ex.Message);
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Order>>();
      try
      {
        var order = portal.Fetch(1);
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
