Public Class Form1

  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Dim dp = New Csla.DataPortal(Of ThreadCheck)
    AddHandler dp.CreateCompleted, AddressOf dp_CreateCompleted
    dp.BeginCreate(New Csla.SingleCriteria(Of ThreadCheck, String)("hi there"))
    'dp.BeginCreate()

  End Sub

  Private Sub dp_CreateCompleted(ByVal sender As Object, ByVal e As Csla.DataPortalResult(Of ThreadCheck))

    If e.Error IsNot Nothing Then
      Me.TextBox1.AppendText(e.Error.ToString)
      Me.TextBox1.AppendText(vbCrLf)

    Else
      e.Object.Thread = System.Threading.Thread.CurrentThread.ManagedThreadId

      Me.TextBox1.AppendText(e.Object.Thread.ToString)
      Me.TextBox1.AppendText(vbCrLf)

      Me.TextBox1.AppendText(e.Object.CreateThread.ToString)
      Me.TextBox1.AppendText(vbCrLf)

      Me.TextBox1.AppendText(e.Object.Data)
      Me.TextBox1.AppendText(vbCrLf)
    End If

  End Sub

End Class
