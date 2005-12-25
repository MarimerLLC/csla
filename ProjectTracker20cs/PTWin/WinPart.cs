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
    //public event EventHandler<StatusChangedEventArgs> StatusChanged;

    public virtual string Title
    {
      get { return GetIdValue().ToString(); }
    }

    protected internal virtual object GetIdValue()
    {
      return null;
    }

    public override bool Equals(object obj)
    {
      object id = GetIdValue();
      if (this.GetType().Equals(obj.GetType()) && id != null)
        return ((WinPart)obj).GetIdValue().Equals(id);
      else
        return false;
    }

    public override int GetHashCode()
    {
      object id = GetIdValue();
      if (id != null)
        return GetIdValue().GetHashCode();
      else
        return base.GetHashCode();
    }

    protected void Close()
    {
      if (CloseWinPart != null)
        CloseWinPart(this, EventArgs.Empty);
    }

    //#region Status 

    //protected void OnStatusChanged()
    //{
    //  OnStatusChanged(string.Empty, false);
    //}

    //protected void OnStatusChanged(string statusText)
    //{
    //  OnStatusChanged(statusText, !string.IsNullOrEmpty(statusText));
    //}

    //protected virtual void OnStatusChanged(string statusText, bool busy)
    //{
    //  if (StatusChanged != null)
    //    StatusChanged(this, new StatusChangedEventArgs(statusText, busy));
    //}

    //#endregion

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
