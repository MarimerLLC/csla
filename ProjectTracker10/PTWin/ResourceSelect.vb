Public Class ResourceSelect
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
  Friend WithEvents DataView1 As PTWin.DataListView
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents btnOK As System.Windows.Forms.Button
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.DataView1 = New PTWin.DataListView
    Me.btnCancel = New System.Windows.Forms.Button
    Me.btnOK = New System.Windows.Forms.Button
    Me.SuspendLayout()
    '
    'DataView1
    '
    Me.DataView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.DataView1.AutoDiscover = False
    Me.DataView1.DataSource = Nothing
    Me.DataView1.DisplayMember = ""
    Me.DataView1.FullRowSelect = True
    Me.DataView1.Location = New System.Drawing.Point(-8, -2)
    Me.DataView1.MultiSelect = False
    Me.DataView1.Name = "DataView1"
    Me.DataView1.Size = New System.Drawing.Size(368, 232)
    Me.DataView1.TabIndex = 5
    Me.DataView1.View = System.Windows.Forms.View.Details
    '
    'btnCancel
    '
    Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.btnCancel.Location = New System.Drawing.Point(375, 38)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.TabIndex = 4
    Me.btnCancel.Text = "Cancel"
    '
    'btnOK
    '
    Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnOK.Location = New System.Drawing.Point(375, 6)
    Me.btnOK.Name = "btnOK"
    Me.btnOK.TabIndex = 3
    Me.btnOK.Text = "OK"
    '
    'ResourceSelect
    '
    Me.AcceptButton = Me.btnOK
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.btnCancel
    Me.ClientSize = New System.Drawing.Size(456, 229)
    Me.Controls.Add(Me.DataView1)
    Me.Controls.Add(Me.btnCancel)
    Me.Controls.Add(Me.btnOK)
    Me.Name = "ResourceSelect"
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "ResourceSelect"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mResult As String

  Private Sub ResourceSelect_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    Dim list As ResourceList = ResourceList.GetResourceList
    With DataView1
      .AutoDiscover = False
      .Columns.Add("ID", 0)
      .Columns.Add("Name", DataView1.Width)
      .DataSource = list
      .Focus()
    End With

  End Sub

  Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
      Handles btnOK.Click, DataView1.DoubleClick

    If DataView1.SelectedItems.Count > 0 Then
      mResult = DataView1.SelectedItems(0).Text

    Else
      mResult = ""
    End If
    Hide()

  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    mResult = ""
    Hide()

  End Sub

  Public ReadOnly Property Result() As String
    Get
      Return mResult
    End Get
  End Property

End Class
