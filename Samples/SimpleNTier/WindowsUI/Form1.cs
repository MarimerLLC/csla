using System;
using System.Windows.Forms;
using Csla;

namespace WindowsUI
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
      try
      {
        var portal = Program.ApplicationContext.GetRequiredService<IDataPortal<BusinessLibrary.Order>>();
        var obj = await portal.FetchAsync(441);
        cslaActionExtender1.ResetActionBehaviors(obj);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}
