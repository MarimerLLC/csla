using System;
using Wisej.Web;

namespace PTWisej
{
  public class StatusBusy : IDisposable
  {
    private string _oldStatus;
    private Cursor _oldCursor;

    public StatusBusy(string statusText)
    {
      _oldStatus = MainPage.Instance.StatusLabel.Text;
      MainPage.Instance.StatusLabel.Text = statusText;
      _oldCursor = MainPage.Instance.Cursor;
      MainPage.Instance.Cursor = Cursors.WaitCursor;
    }

    // IDisposable
    private bool _disposedValue = false; // To detect redundant calls

    protected void Dispose(bool disposing)
    {
      if (!_disposedValue)
        if (disposing)
        {
          MainPage.Instance.StatusLabel.Text = _oldStatus;
          MainPage.Instance.Cursor = _oldCursor;
        }
      _disposedValue = true;
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
