Public Class ProjectSelect
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
  Friend WithEvents btnOK As System.Windows.Forms.Button
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents dvDisplay As PTWin.DataListView
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.btnOK = New System.Windows.Forms.Button
    Me.btnCancel = New System.Windows.Forms.Button
    Me.dvDisplay = New PTWin.DataListView
    Me.SuspendLayout()
    '
    'btnOK
    '
    Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnOK.Location = New System.Drawing.Point(368, 8)
    Me.btnOK.Name = "btnOK"
    Me.btnOK.TabIndex = 0
    Me.btnOK.Text = "OK"
    '
    'btnCancel
    '
    Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.btnCancel.Location = New System.Drawing.Point(368, 40)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.TabIndex = 1
    Me.btnCancel.Text = "Cancel"
    '
    'dvDisplay
    '
    Me.dvDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.dvDisplay.AutoDiscover = False
    Me.dvDisplay.DataSource = Nothing
    Me.dvDisplay.DisplayMember = ""
    Me.dvDisplay.FullRowSelect = True
    Me.dvDisplay.Location = New System.Drawing.Point(0, 0)
    Me.dvDisplay.MultiSelect = False
    Me.dvDisplay.Name = "dvDisplay"
    Me.dvDisplay.Size = New System.Drawing.Size(352, 232)
    Me.dvDisplay.TabIndex = 2
    Me.dvDisplay.View = System.Windows.Forms.View.Details
    '
    'ProjectSelect
    '
    Me.AcceptButton = Me.btnOK
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.btnCancel
    Me.ClientSize = New System.Drawing.Size(456, 229)
    Me.Controls.Add(Me.dvDisplay)
    Me.Controls.Add(Me.btnCancel)
    Me.Controls.Add(Me.btnOK)
    Me.Name = "ProjectSelect"
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "ProjectSelect"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mResult As String

  Private Sub ProjectSelect_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    Dim list As ProjectTracker.Library.ProjectList = ProjectTracker.Library.ProjectList.GetProjectList
    With dvDisplay
      .AutoDiscover = False
      .Columns.Add("ID", "ID", 0)
      .Columns.Add("Project name", "Name", dvDisplay.Width)
      .DataSource = list
      .Focus()
    End With

  End Sub

  Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
      Handles btnOK.Click, dvDisplay.DoubleClick

    If dvDisplay.SelectedItems.Count > 0 Then
      mResult = dvDisplay.SelectedItems(0).Text

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
