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

namespace NavigationApp
{
  public class NavigationBookmarkProcessedEventArgs : EventArgs
  {
    int _bookmarkId;
    bool _success;
    public NavigationBookmarkProcessedEventArgs(int bookmarkId, bool success)
    {
      _bookmarkId = bookmarkId;
      _success = success;
    }
    public int BookmarkId { get { return _bookmarkId; } }
    public bool Success { get { return _success; } }


  }
}
