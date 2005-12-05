using System;

namespace PTWin
{
  public class StatusBusy : IDisposable
  {

    public StatusBusy(string statusText)
    {
      MainForm.Instance.StatusChanged(statusText, true);
    }

    private bool _disposedValue = false; // To detect redundant calls

    // IDisposable
    protected void Dispose(bool disposing)
    {
      if (!_disposedValue)
        if (disposing)
          MainForm.Instance.StatusChanged();
      _disposedValue = true;
    }

    #region IDisposable Support

    // This code added to correctly implement the disposable pattern
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

  }
}
