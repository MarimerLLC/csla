Imports System.Threading

Public Class ProjectEdit
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
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtName As System.Windows.Forms.TextBox
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents txtStarted As System.Windows.Forms.TextBox
  Friend WithEvents Label3 As System.Windows.Forms.Label
  Friend WithEvents txtEnded As System.Windows.Forms.TextBox
  Friend WithEvents Label4 As System.Windows.Forms.Label
  Friend WithEvents btnSave As System.Windows.Forms.Button
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents Label5 As System.Windows.Forms.Label
  Friend WithEvents txtID As System.Windows.Forms.TextBox
  Friend WithEvents txtDescription As System.Windows.Forms.TextBox
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents btnAddResource As System.Windows.Forms.Button
  Friend WithEvents btnRemoveResource As System.Windows.Forms.Button
  Friend WithEvents dvResources As PTWin.DataListView
  Friend WithEvents lstRules As System.Windows.Forms.ListBox
  Friend WithEvents chkIsDirty As System.Windows.Forms.CheckBox
  Friend WithEvents mnuRoles As System.Windows.Forms.ContextMenu
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.Label1 = New System.Windows.Forms.Label
    Me.txtName = New System.Windows.Forms.TextBox
    Me.Label2 = New System.Windows.Forms.Label
    Me.txtStarted = New System.Windows.Forms.TextBox
    Me.Label3 = New System.Windows.Forms.Label
    Me.txtEnded = New System.Windows.Forms.TextBox
    Me.Label4 = New System.Windows.Forms.Label
    Me.btnSave = New System.Windows.Forms.Button
    Me.btnCancel = New System.Windows.Forms.Button
    Me.Label5 = New System.Windows.Forms.Label
    Me.txtID = New System.Windows.Forms.TextBox
    Me.txtDescription = New System.Windows.Forms.TextBox
    Me.GroupBox1 = New System.Windows.Forms.GroupBox
    Me.dvResources = New PTWin.DataListView
    Me.mnuRoles = New System.Windows.Forms.ContextMenu
    Me.btnRemoveResource = New System.Windows.Forms.Button
    Me.btnAddResource = New System.Windows.Forms.Button
    Me.lstRules = New System.Windows.Forms.ListBox
    Me.chkIsDirty = New System.Windows.Forms.CheckBox
    Me.GroupBox1.SuspendLayout()
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.Location = New System.Drawing.Point(8, 40)
    Me.Label1.Name = "Label1"
    Me.Label1.TabIndex = 2
    Me.Label1.Text = "Name"
    '
    'txtName
    '
    Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtName.Location = New System.Drawing.Point(88, 40)
    Me.txtName.Name = "txtName"
    Me.txtName.Size = New System.Drawing.Size(544, 20)
    Me.txtName.TabIndex = 3
    Me.txtName.Text = ""
    '
    'Label2
    '
    Me.Label2.Location = New System.Drawing.Point(8, 72)
    Me.Label2.Name = "Label2"
    Me.Label2.TabIndex = 4
    Me.Label2.Text = "Started"
    '
    'txtStarted
    '
    Me.txtStarted.Location = New System.Drawing.Point(88, 72)
    Me.txtStarted.Name = "txtStarted"
    Me.txtStarted.TabIndex = 5
    Me.txtStarted.Text = ""
    '
    'Label3
    '
    Me.Label3.Location = New System.Drawing.Point(8, 104)
    Me.Label3.Name = "Label3"
    Me.Label3.TabIndex = 6
    Me.Label3.Text = "Ended"
    '
    'txtEnded
    '
    Me.txtEnded.Location = New System.Drawing.Point(88, 104)
    Me.txtEnded.Name = "txtEnded"
    Me.txtEnded.TabIndex = 7
    Me.txtEnded.Text = ""
    '
    'Label4
    '
    Me.Label4.Location = New System.Drawing.Point(8, 136)
    Me.Label4.Name = "Label4"
    Me.Label4.TabIndex = 8
    Me.Label4.Text = "Description"
    '
    'btnSave
    '
    Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnSave.Location = New System.Drawing.Point(664, 8)
    Me.btnSave.Name = "btnSave"
    Me.btnSave.TabIndex = 11
    Me.btnSave.Text = "&Save"
    '
    'btnCancel
    '
    Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.btnCancel.Location = New System.Drawing.Point(664, 40)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.TabIndex = 12
    Me.btnCancel.Text = "&Cancel"
    '
    'Label5
    '
    Me.Label5.Location = New System.Drawing.Point(8, 8)
    Me.Label5.Name = "Label5"
    Me.Label5.TabIndex = 0
    Me.Label5.Text = "ID"
    '
    'txtID
    '
    Me.txtID.Location = New System.Drawing.Point(88, 8)
    Me.txtID.Name = "txtID"
    Me.txtID.ReadOnly = True
    Me.txtID.Size = New System.Drawing.Size(240, 20)
    Me.txtID.TabIndex = 1
    Me.txtID.TabStop = False
    Me.txtID.Text = ""
    '
    'txtDescription
    '
    Me.txtDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.txtDescription.Location = New System.Drawing.Point(88, 136)
    Me.txtDescription.Multiline = True
    Me.txtDescription.Name = "txtDescription"
    Me.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
    Me.txtDescription.Size = New System.Drawing.Size(416, 96)
    Me.txtDescription.TabIndex = 9
    Me.txtDescription.Text = ""
    '
    'GroupBox1
    '
    Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.GroupBox1.Controls.Add(Me.dvResources)
    Me.GroupBox1.Controls.Add(Me.btnRemoveResource)
    Me.GroupBox1.Controls.Add(Me.btnAddResource)
    Me.GroupBox1.Location = New System.Drawing.Point(8, 240)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(736, 176)
    Me.GroupBox1.TabIndex = 10
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Resources"
    '
    'dvResources
    '
    Me.dvResources.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.dvResources.AutoDiscover = False
    Me.dvResources.ContextMenu = Me.mnuRoles
    Me.dvResources.DataSource = Nothing
    Me.dvResources.DisplayMember = ""
    Me.dvResources.FullRowSelect = True
    Me.dvResources.Location = New System.Drawing.Point(8, 24)
    Me.dvResources.MultiSelect = False
    Me.dvResources.Name = "dvResources"
    Me.dvResources.Size = New System.Drawing.Size(632, 144)
    Me.dvResources.TabIndex = 3
    Me.dvResources.View = System.Windows.Forms.View.Details
    '
    'btnRemoveResource
    '
    Me.btnRemoveResource.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnRemoveResource.Location = New System.Drawing.Point(648, 56)
    Me.btnRemoveResource.Name = "btnRemoveResource"
    Me.btnRemoveResource.TabIndex = 2
    Me.btnRemoveResource.Text = "&Remove"
    '
    'btnAddResource
    '
    Me.btnAddResource.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.btnAddResource.Location = New System.Drawing.Point(648, 24)
    Me.btnAddResource.Name = "btnAddResource"
    Me.btnAddResource.TabIndex = 1
    Me.btnAddResource.Text = "&Add"
    '
    'lstRules
    '
    Me.lstRules.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
    Me.lstRules.Location = New System.Drawing.Point(544, 104)
    Me.lstRules.Name = "lstRules"
    Me.lstRules.Size = New System.Drawing.Size(192, 108)
    Me.lstRules.TabIndex = 14
    '
    'chkIsDirty
    '
    Me.chkIsDirty.Enabled = False
    Me.chkIsDirty.Location = New System.Drawing.Point(488, 8)
    Me.chkIsDirty.Name = "chkIsDirty"
    Me.chkIsDirty.Size = New System.Drawing.Size(0, 0)
    Me.chkIsDirty.TabIndex = 15
    Me.chkIsDirty.Text = "IsDirty"
    '
    'ProjectEdit
    '
    Me.AcceptButton = Me.btnSave
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.btnCancel
    Me.ClientSize = New System.Drawing.Size(752, 421)
    Me.Controls.Add(Me.chkIsDirty)
    Me.Controls.Add(Me.lstRules)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(Me.txtID)
    Me.Controls.Add(Me.txtDescription)
    Me.Controls.Add(Me.txtEnded)
    Me.Controls.Add(Me.txtStarted)
    Me.Controls.Add(Me.txtName)
    Me.Controls.Add(Me.Label5)
    Me.Controls.Add(Me.btnCancel)
    Me.Controls.Add(Me.btnSave)
    Me.Controls.Add(Me.Label4)
    Me.Controls.Add(Me.Label3)
    Me.Controls.Add(Me.Label2)
    Me.Controls.Add(Me.Label1)
    Me.Name = "ProjectEdit"
    Me.Text = "ProjectEdit"
    Me.GroupBox1.ResumeLayout(False)
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mProject As Project

  Public Property Project() As Project
    Get
      Return mProject
    End Get
    Set(ByVal Value As Project)
      mProject = Value
    End Set
  End Property

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    mProject.CancelEdit()
    Hide()

  End Sub

  Private Sub btnSave_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnSave.Click

    Try
      Cursor.Current = Cursors.WaitCursor
      mProject.ApplyEdit()
      mProject = DirectCast(mProject.Save, Project)
      DataBind()
      Cursor.Current = Cursors.Default

    Catch ex As Exception
      Cursor.Current = Cursors.Default
      MsgBox(ex.ToString)
    End Try

    Hide()

  End Sub

  Private Sub ProjectEdit_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    Me.Text = "Project " & mProject.Name

    Dim Role As String
    For Each Role In Assignment.Roles
      mnuRoles.MenuItems.Add(Assignment.Roles.Item(Role))
      AddHandler mnuRoles.MenuItems(mnuRoles.MenuItems.Count - 1).Click, _
        AddressOf mnuRoles_Click
    Next

    If Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
      ' only project managers can save a project
      mProject.BeginEdit()
      btnAddResource.Enabled = True
      btnRemoveResource.Enabled = True

    Else
      btnAddResource.Enabled = False
      btnRemoveResource.Enabled = False
    End If

    DataBind()

  End Sub

  Private Sub DataBind()

    If Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
      ' only project managers can save a project
      BindField(btnSave, "Enabled", mProject, "IsValid")

    Else
      btnSave.Enabled = False
    End If
    BindField(chkIsDirty, "Checked", mProject, "IsDirty")
    BindField(txtID, "Text", mProject, "ID")
    BindField(txtName, "Text", mProject, "Name")
    BindField(txtStarted, "Text", mProject, "Started")
    BindField(txtEnded, "Text", mProject, "Ended")
    BindField(txtDescription, "Text", mProject, "Description")
    lstRules.DataSource = mProject.GetBrokenRulesCollection
    lstRules.DisplayMember = "Description"
    With dvResources
      .SuspendLayout()
      .Clear()
      .AutoDiscover = False
      .Columns.Add("ID", "ResourceID", 0)
      .Columns.Add("Last name", "LastName", 100)
      .Columns.Add("First name", "FirstName", 100)
      .Columns.Add("Assigned", "Assigned", 100)
      .Columns.Add("Role", "Role", 150)
      .DataSource = mProject.Resources
      .ResumeLayout()
    End With

  End Sub

  Private Sub ProjectEdit_Closing(ByVal sender As Object, _
      ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

    mProject.CancelEdit()

  End Sub

  Private Sub btnAddResource_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnAddResource.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Assign resource"
    dlg.ShowDialog(Me)
    Dim ID As String = dlg.Result

    If Len(ID) > 0 Then
      dvResources.SuspendLayout()
      dvResources.DataSource = Nothing
      Try
        mProject.Resources.Assign(ID)

      Catch ex As Exception
        MsgBox(ex.Message)

      Finally
        dvResources.DataSource = mProject.Resources
        dvResources.ResumeLayout()
      End Try
    End If

  End Sub

  Private Sub btnRemoveResource_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnRemoveResource.Click

    Dim ID As String = dvResources.SelectedItems(0).Text

    If MsgBox("Remove resource " & ID & " from project?", _
        MsgBoxStyle.YesNo, "Remove resource") = MsgBoxResult.Yes Then
      dvResources.SuspendLayout()
      dvResources.DataSource = Nothing
      mProject.Resources.Remove(ID)
      dvResources.DataSource = mProject.Resources
      dvResources.ResumeLayout()
    End If

  End Sub

  Private Sub dvResources_DoubleClick(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles dvResources.DoubleClick

    Dim ID As String = dvResources.SelectedItems(0).Text
    Cursor.Current = Cursors.WaitCursor
    Dim frm As New ResourceEdit
    frm.MdiParent = Me.MdiParent
    frm.Resource = Resource.GetResource(ID)
    Cursor.Current = Cursors.Default
    frm.Show()

  End Sub

  Private Sub mnuRoles_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs)

    Dim Item As MenuItem = CType(sender, MenuItem)
    If dvResources.SelectedItems.Count > 0 Then
      Dim ID As String = dvResources.SelectedItems(0).Text

      dvResources.SuspendLayout()
      dvResources.DataSource = Nothing
      mProject.Resources.Item(ID).Role = Item.Text
      dvResources.DataSource = mProject.Resources
      dvResources.ResumeLayout()
    End If

  End Sub

End Class
