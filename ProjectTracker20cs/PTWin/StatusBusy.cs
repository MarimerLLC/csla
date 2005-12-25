using System;
using System.Windows.Forms;

namespace PTWin
{
  public class StatusBusy : IDisposable
  {
    public StatusBusy(string statusText)
    {
      MainForm.Instance.StatusLabel.Text = statusText;
      MainForm.Instance.Cursor = Cursors.WaitCursor;
    }

    // IDisposable
    private bool _disposedValue = false; // To detect redundant calls

    protected void Dispose(bool disposing)
    {
      if (!_disposedValue)
        if (disposing)
        {
          MainForm.Instance.StatusLabel.Text = string.Empty;
          MainForm.Instance.Cursor = Cursors.Default;
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
