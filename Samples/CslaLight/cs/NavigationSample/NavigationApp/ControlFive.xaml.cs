using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Silverlight;
using System.ComponentModel;

namespace NavigationApp
{
  public partial class ControlFive : UserControl, ISupportNavigation
  {
    public ControlFive()
    {
      InitializeComponent();
    }

    private string title = "Control Five";
    public const string Bookmark = "ControlThree";

    #region ISupportNavigation Members

    public bool CreateBookmarkAfterLoadCompleted
    {
      get { return true; }
    }

    public event EventHandler LoadCompleted;

    public void SetParameters(string parameters)
    {
      this.ParametersBlock.Text = parameters;
      BackgroundWorker worder = new BackgroundWorker();
      worder.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worder_RunWorkerCompleted);
      worder.DoWork += new DoWorkEventHandler(worder_DoWork);
      worder.RunWorkerAsync();
    }

    void worder_DoWork(object sender, DoWorkEventArgs e)
    {
      //simulate long running process
      for (int i = 0; i < 100000000; i++)
      {
        //just waste some time
      }
    }

    void worder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      title = "Asynch Control";
      if (LoadCompleted != null)
        LoadCompleted(this, EventArgs.Empty);
    }

    public string Title
    {
      get { return title; }
    }

    #endregion
  }
}
