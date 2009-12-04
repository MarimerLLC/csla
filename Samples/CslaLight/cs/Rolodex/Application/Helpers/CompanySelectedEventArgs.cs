using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rolodex
{
  public class CompanySelectedEventArgs : EventArgs
  {
    private CompanySelectedEventArgs() { }
    public CompanySelectedEventArgs(int companyID)
    {
      CompanyID = companyID;
    }
    public int CompanyID { get; private set; }
  }
}
