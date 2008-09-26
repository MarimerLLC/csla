using System;

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
