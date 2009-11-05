using System;
using System.Windows.Forms;

namespace PTWin
{
  public class StatusBusy : IDisposable
  {
    private string _oldStatus;
    private Cursor _oldCursor;

    public StatusBusy(string statusText)
    {
      _oldStatus = MainForm.Instance.StatusLabel.Text;
      MainForm.Instance.StatusLabel.Text = statusText;
      _oldCursor = MainForm.Instance.Cursor;
      MainForm.Instance.Cursor = Cursors.WaitCursor;
    }

    // IDisposable
    private bool _disposedValue = false; // To detect redundant calls

    protected void Dispose(bool disposing)
    {
      if (!_disposedValue)
        if (disposing)
        {
          MainForm.Instance.StatusLabel.Text = _oldStatus;
          MainForm.Instance.Cursor = _oldCursor;
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
