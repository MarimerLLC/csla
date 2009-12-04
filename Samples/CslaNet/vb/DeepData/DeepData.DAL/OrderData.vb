Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient

Public MustInherit Class OrderData
  Implements IDisposable

  Public MustOverride Function GetOrders() As Object

  Protected Sub New()

  End Sub

#Region " IDisposable Support "

  Private disposedValue As Boolean = False    ' To detect redundant calls

  ' IDisposable
  Protected Overridable Sub Dispose(ByVal disposing As Boolean)
    If Not Me.disposedValue Then
      If disposing Then
      End If
    End If
    Me.disposedValue = True
  End Sub

  ' This code added by Visual Basic to correctly implement the disposable pattern.
  Public Sub Dispose() Implements IDisposable.Dispose
    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    Dispose(True)
    GC.SuppressFinalize(Me)
  End Sub
#End Region

End Class
