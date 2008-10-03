using System;

namespace Csla.Silverlight
{
  /// <summary>
  /// Event arguments class that is part of Navigator event
  /// that is fired after a bookmakr has been processed
  /// in response to browser's back or forward button processing.
  /// </summary>
  public class NavigationBookmarkProcessedEventArgs : EventArgs
  {
    int _bookmarkId;
    bool _success;

    /// <summary>
    /// New instance of event arguments
    /// </summary>
    /// <param name="bookmarkId">
    /// Id of the bookmakr that was processed
    /// </param>
    /// <param name="success">
    /// True if bookamrk was processed successfully.
    /// </param>
    public NavigationBookmarkProcessedEventArgs(int bookmarkId, bool success)
    {
      _bookmarkId = bookmarkId;
      _success = success;
    }

    /// <summary>
    /// Bookrmark Id
    /// </summary>
    public int BookmarkId { get { return _bookmarkId; } }

    /// <summary>
    /// Indicates if bookmakr was processed successfully
    /// and control is displayed.
    /// </summary>
    public bool Success { get { return _success; } }


  }
}
