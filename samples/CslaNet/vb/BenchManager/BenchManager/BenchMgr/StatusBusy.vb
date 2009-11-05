Public Class StatusBusy

  Implements IDisposable

  Private mOldStatus As String
  Private mOldCursor As Cursor

  Public Sub New(ByVal statusText As String)

    mOldStatus = MainForm.StatusText
    MainForm.StatusText = statusText
    mOldCursor = MainForm.CurrentCursor
    MainForm.CurrentCursor = Cursors.Wait

  End Sub

  ' IDisposable
  Private disposedValue As Boolean = False ' To detect redundant calls

  Protected Overridable Sub Dispose(ByVal disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
        MainForm.StatusText = mOldStatus
        MainForm.CurrentCursor = mOldCursor
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
