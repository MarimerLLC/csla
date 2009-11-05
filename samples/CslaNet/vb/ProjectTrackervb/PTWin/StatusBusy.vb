Public Class StatusBusy

  Implements IDisposable

  Private _oldStatus As String
  Private _oldCursor As Cursor

  Public Sub New(ByVal statusText As String)

    _oldStatus = MainForm.StatusLabel.Text
    MainForm.StatusLabel.Text = statusText
    _oldCursor = MainForm.Cursor
    MainForm.Cursor = Cursors.WaitCursor

  End Sub

  ' IDisposable
  Private disposedValue As Boolean = False ' To detect redundant calls

  Protected Overridable Sub Dispose(ByVal disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
        MainForm.StatusLabel.Text = _oldStatus
        MainForm.Cursor = _oldCursor
      End If
    End If
    Me.disposedValue = True
  End Sub

  Public Sub Dispose() Implements IDisposable.Dispose
    ' Do not change this code.  
    ' Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    Dispose(True)
    GC.SuppressFinalize(Me)
  End Sub

End Class
