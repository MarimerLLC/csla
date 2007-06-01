Namespace Web.Design

  <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
  Partial Class CslaDataSourceConfiguration
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
        If disposing AndAlso components IsNot Nothing Then
          components.Dispose()
        End If
      Finally
        MyBase.Dispose(disposing)
      End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CslaDataSourceConfiguration))
      Me.Label1 = New System.Windows.Forms.Label
      Me.TypeComboBox = New System.Windows.Forms.ComboBox
      Me.OkButton = New System.Windows.Forms.Button
      Me.CancelBtn = New System.Windows.Forms.Button
      Me.SuspendLayout()
      '
      'Label1
      '
      resources.ApplyResources(Me.Label1, "Label1")
      Me.Label1.Name = "Label1"
      '
      'TypeComboBox
      '
      resources.ApplyResources(Me.TypeComboBox, "TypeComboBox")
      Me.TypeComboBox.FormattingEnabled = True
      Me.TypeComboBox.Name = "TypeComboBox"
      Me.TypeComboBox.Sorted = True
      '
      'OkButton
      '
      resources.ApplyResources(Me.OkButton, "OkButton")
      Me.OkButton.Name = "OkButton"
      Me.OkButton.UseVisualStyleBackColor = True
      '
      'CancelBtn
      '
      resources.ApplyResources(Me.CancelBtn, "CancelBtn")
      Me.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel
      Me.CancelBtn.Name = "CancelBtn"
      Me.CancelBtn.UseVisualStyleBackColor = True
      '
      'CslaDataSourceConfiguration
      '
      Me.AcceptButton = Me.OkButton
      resources.ApplyResources(Me, "$this")
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.CancelButton = Me.CancelBtn
      Me.ControlBox = False
      Me.Controls.Add(Me.CancelBtn)
      Me.Controls.Add(Me.OkButton)
      Me.Controls.Add(Me.TypeComboBox)
      Me.Controls.Add(Me.Label1)
      Me.MaximizeBox = False
      Me.MinimizeBox = False
      Me.Name = "CslaDataSourceConfiguration"
      Me.ShowInTaskbar = False
      Me.ResumeLayout(False)
      Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TypeComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents OkButton As System.Windows.Forms.Button
    Friend WithEvents CancelBtn As System.Windows.Forms.Button
  End Class

End Namespace
