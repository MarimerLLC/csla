using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace BackgroundWorkerDemo
{
  public partial class BackgroundWorkerForm : Form
  {
    delegate void AddItemToList(string item, bool allowInvoke);

    private void AddToList(string item, bool allowInvoke = false)
    {
      if (allowInvoke && this.listBox1.InvokeRequired)
      {
        AddItemToList d = AddToList;
        this.Invoke(d, new object[] { item, false });
      }
      else
      {
        listBox1.Items.Add(item);
      }
    }

    public BackgroundWorkerForm()
    {
      InitializeComponent();



      listBox1.Items.Clear();
    }

    #region  Clear list

    private void button1_Click_1(object sender, EventArgs e)
    {
      listBox1.Items.Clear();
    }

    #endregion

    #region Do work and completed

    private void Test_DoWork(object sender, DoWorkEventArgs e)
    {
      var param = e.Argument as string;

      var principal = Csla.ApplicationContext.User;
      AddToList(string.Format("Csla User PrincipalType: {0}", principal), true);
      AddToList(string.Format("UserName: {0}", principal.Identity.Name), true);
      AddToList(string.Format("CurrentCulture: {0}", Thread.CurrentThread.CurrentCulture.DisplayName), true);
      AddToList(string.Format("CurrentUICulture: {0}", Thread.CurrentThread.CurrentUICulture.DisplayName), true);

      var clientContext = Csla.ApplicationContext.ClientContext;
      var test = clientContext["TEST"];
      AddToList(string.Format("ClientContext: {0}", test), true);

      var globalContext = Csla.ApplicationContext.GlobalContext;
      var global1 = globalContext["GLOBAL1"];
      AddToList(string.Format("GlobalContext: {0}", global1), true);

      Csla.ApplicationContext.GlobalContext["NEW"] = "NewGlobalContextValue";

      if (!string.IsNullOrEmpty(param) && param == "THROW")
        throw new ArgumentException("Error");
    }

    private void Test_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        AddToList(string.Format("Worker failed, {0}", e.Error.Message));
      }
      else
      {
        AddToList("Worker completed");
        var bw = sender as Csla.Threading.BackgroundWorker;
        if (bw != null)
        {
          AddToList(string.Format("New value in GlobalContext: {0}", (string)bw.GlobalContext["NEW"]));
        }
      }
      AddToList("----- END ------");
    }

    #endregion

    #region Csla.Threading.BackgroundWorker

    private void CslaButton_Click(object sender, EventArgs e)
    {
      cslaBackgroundWorker1.RunWorkerAsync();
    }

    #endregion

    #region System.ComponentModel.BackgroundWorker

    private void BWButton_Click(object sender, EventArgs e)
    {
      backgroundWorker1.RunWorkerAsync();
    }

    #endregion

    #region Worker reporting progress changed

    private void button2_Click(object sender, EventArgs e)
    {
      AddToList(string.Format("Starting worker reporting progress changed"));

      backgroundWorker2.RunWorkerAsync(null);
    }

    private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
    {
      for (int i = 0; i < 11; i++)
      {
        backgroundWorker2.ReportProgress(i * 10);
        Thread.Sleep(100);
      }
    }

    private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      AddToList(string.Format("Progress update {0} percent", e.ProgressPercentage));
    }

    private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      AddToList(string.Format("Worker completed"));
    }



    #endregion


  }
}
