using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PTWin
{
  public partial class WinPart : UserControl
  {

    public event EventHandler CloseWinPart;
    public event EventHandler<StatusChangedEventArgs> StatusChanged;

    private string _title;
    public virtual string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    protected void Close()
    {
      if (CloseWinPart != null)
        CloseWinPart(this, EventArgs.Empty);
    }

    protected virtual void OnStatusChanged()
    {
      OnStatusChanged(string.Empty, false);
    }

    protected virtual void OnStatusChanged(string statusText)
    {
      if (StatusChanged != null)
        StatusChanged(this, new StatusChangedEventArgs(statusText));
    }

    protected virtual void OnStatusChanged(string statusText, bool busy)
    {
      if (StatusChanged != null)
        StatusChanged(this, new StatusChangedEventArgs(statusText, busy));
    }

    #region CurrentPrincipalChanged

    protected event EventHandler CurrentPrincipalChanged;

    internal void PrincipalChanged(object sender, EventArgs e)
    {
      OnCurrentPrincipalChanged(sender, e);
    }

    protected virtual void OnCurrentPrincipalChanged(object sender, EventArgs e)
    {
      if (CurrentPrincipalChanged != null)
        CurrentPrincipalChanged(sender, e);
    }

    #endregion

  }
}
