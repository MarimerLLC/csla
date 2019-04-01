namespace PTWin
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.ProjectsStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
      this.NewProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.DeleteProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ResourcesToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.NewResourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditResourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.DeleteResourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.AdminToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.EditRolesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.LoginToolStripLabel = new System.Windows.Forms.ToolStripLabel();
      this.LoginToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.DocumentsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.Panel1 = new System.Windows.Forms.Panel();
      this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
      this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStrip1.SuspendLayout();
      this.StatusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip1
      // 
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.ProjectsStripDropDownButton1,
      this.ResourcesToolStripDropDownButton,
      this.AdminToolStripDropDownButton,
      this.LoginToolStripLabel,
      this.LoginToolStripButton,
      this.DocumentsToolStripDropDownButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(827, 25);
      this.toolStrip1.TabIndex = 1;
      // 
      // ProjectsStripDropDownButton1
      // 
      this.ProjectsStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.NewProjectToolStripMenuItem,
      this.EditProjectToolStripMenuItem,
      this.DeleteProjectToolStripMenuItem});
      this.ProjectsStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("ProjectsStripDropDownButton1.Image")));
      this.ProjectsStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.ProjectsStripDropDownButton1.Name = "ProjectsStripDropDownButton1";
      this.ProjectsStripDropDownButton1.Size = new System.Drawing.Size(78, 22);
      this.ProjectsStripDropDownButton1.Text = "Projects";
      // 
      // NewProjectToolStripMenuItem
      // 
      this.NewProjectToolStripMenuItem.Name = "NewProjectToolStripMenuItem";
      this.NewProjectToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
      this.NewProjectToolStripMenuItem.Text = "New project";
      this.NewProjectToolStripMenuItem.Click += new System.EventHandler(this.NewProjectToolStripMenuItem_Click);
      // 
      // EditProjectToolStripMenuItem
      // 
      this.EditProjectToolStripMenuItem.Name = "EditProjectToolStripMenuItem";
      this.EditProjectToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
      this.EditProjectToolStripMenuItem.Text = "Edit project";
      this.EditProjectToolStripMenuItem.Click += new System.EventHandler(this.EditProjectToolStripMenuItem_Click);
      // 
      // DeleteProjectToolStripMenuItem
      // 
      this.DeleteProjectToolStripMenuItem.Name = "DeleteProjectToolStripMenuItem";
      this.DeleteProjectToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
      this.DeleteProjectToolStripMenuItem.Text = "Delete project";
      this.DeleteProjectToolStripMenuItem.Click += new System.EventHandler(this.DeleteProjectToolStripMenuItem_Click);
      // 
      // ResourcesToolStripDropDownButton
      // 
      this.ResourcesToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.NewResourceToolStripMenuItem,
      this.EditResourceToolStripMenuItem,
      this.DeleteResourceToolStripMenuItem});
      this.ResourcesToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("ResourcesToolStripDropDownButton.Image")));
      this.ResourcesToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.ResourcesToolStripDropDownButton.Name = "ResourcesToolStripDropDownButton";
      this.ResourcesToolStripDropDownButton.Size = new System.Drawing.Size(89, 22);
      this.ResourcesToolStripDropDownButton.Text = "Resources";
      // 
      // NewResourceToolStripMenuItem
      // 
      this.NewResourceToolStripMenuItem.Name = "NewResourceToolStripMenuItem";
      this.NewResourceToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this.NewResourceToolStripMenuItem.Text = "New resource";
      this.NewResourceToolStripMenuItem.Click += new System.EventHandler(this.NewResourceToolStripMenuItem_Click);
      // 
      // EditResourceToolStripMenuItem
      // 
      this.EditResourceToolStripMenuItem.Name = "EditResourceToolStripMenuItem";
      this.EditResourceToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this.EditResourceToolStripMenuItem.Text = "Edit resource";
      this.EditResourceToolStripMenuItem.Click += new System.EventHandler(this.EditResourceToolStripMenuItem_Click);
      // 
      // DeleteResourceToolStripMenuItem
      // 
      this.DeleteResourceToolStripMenuItem.Name = "DeleteResourceToolStripMenuItem";
      this.DeleteResourceToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this.DeleteResourceToolStripMenuItem.Text = "Delete resource";
      this.DeleteResourceToolStripMenuItem.Click += new System.EventHandler(this.DeleteResourceToolStripMenuItem_Click);
      // 
      // AdminToolStripDropDownButton
      // 
      this.AdminToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.EditRolesToolStripMenuItem});
      this.AdminToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("AdminToolStripDropDownButton.Image")));
      this.AdminToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.AdminToolStripDropDownButton.Name = "AdminToolStripDropDownButton";
      this.AdminToolStripDropDownButton.Size = new System.Drawing.Size(72, 22);
      this.AdminToolStripDropDownButton.Text = "Admin";
      // 
      // EditRolesToolStripMenuItem
      // 
      this.EditRolesToolStripMenuItem.Name = "EditRolesToolStripMenuItem";
      this.EditRolesToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
      this.EditRolesToolStripMenuItem.Text = "Edit roles";
      this.EditRolesToolStripMenuItem.Click += new System.EventHandler(this.EditRolesToolStripMenuItem_Click);
      // 
      // LoginToolStripLabel
      // 
      this.LoginToolStripLabel.Name = "LoginToolStripLabel";
      this.LoginToolStripLabel.Size = new System.Drawing.Size(80, 22);
      this.LoginToolStripLabel.Text = "Not logged in";
      // 
      // LoginToolStripButton
      // 
      this.LoginToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.LoginToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("LoginToolStripButton.Image")));
      this.LoginToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.LoginToolStripButton.Name = "LoginToolStripButton";
      this.LoginToolStripButton.Size = new System.Drawing.Size(41, 22);
      this.LoginToolStripButton.Text = "Login";
      this.LoginToolStripButton.Click += new System.EventHandler(this.LoginToolStripButton_Click);
      // 
      // DocumentsToolStripDropDownButton
      // 
      this.DocumentsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.DocumentsToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("DocumentsToolStripDropDownButton.Image")));
      this.DocumentsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.DocumentsToolStripDropDownButton.Name = "DocumentsToolStripDropDownButton";
      this.DocumentsToolStripDropDownButton.Size = new System.Drawing.Size(81, 22);
      this.DocumentsToolStripDropDownButton.Text = "Documents";
      this.DocumentsToolStripDropDownButton.DropDownOpening += new System.EventHandler(this.DocumentsToolStripDropDownButton_DropDownOpening);
      // 
      // Panel1
      // 
      this.Panel1.BackColor = System.Drawing.SystemColors.ControlDark;
      this.Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Panel1.Location = new System.Drawing.Point(0, 25);
      this.Panel1.Name = "Panel1";
      this.Panel1.Size = new System.Drawing.Size(827, 393);
      this.Panel1.TabIndex = 2;
      // 
      // StatusStrip1
      // 
      this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.StatusLabel});
      this.StatusStrip1.Location = new System.Drawing.Point(0, 396);
      this.StatusStrip1.Name = "StatusStrip1";
      this.StatusStrip1.Size = new System.Drawing.Size(827, 22);
      this.StatusStrip1.TabIndex = 3;
      this.StatusStrip1.Text = "statusStrip1";
      // 
      // StatusLabel
      // 
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.Size = new System.Drawing.Size(0, 17);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(827, 418);
      this.Controls.Add(this.StatusStrip1);
      this.Controls.Add(this.Panel1);
      this.Controls.Add(this.toolStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "Project Tracker";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.StatusStrip1.ResumeLayout(false);
      this.StatusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.ToolStrip toolStrip1;
    internal System.Windows.Forms.ToolStripDropDownButton ProjectsStripDropDownButton1;
    internal System.Windows.Forms.ToolStripMenuItem NewProjectToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem EditProjectToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem DeleteProjectToolStripMenuItem;
    internal System.Windows.Forms.ToolStripLabel LoginToolStripLabel;
    internal System.Windows.Forms.ToolStripButton LoginToolStripButton;
    internal System.Windows.Forms.ToolStripDropDownButton ResourcesToolStripDropDownButton;
    internal System.Windows.Forms.ToolStripMenuItem NewResourceToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem EditResourceToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem DeleteResourceToolStripMenuItem;
    internal System.Windows.Forms.ToolStripDropDownButton AdminToolStripDropDownButton;
    internal System.Windows.Forms.ToolStripMenuItem EditRolesToolStripMenuItem;
    internal System.Windows.Forms.ToolStripDropDownButton DocumentsToolStripDropDownButton;
    internal System.Windows.Forms.Panel Panel1;
    internal System.Windows.Forms.StatusStrip StatusStrip1;
    internal System.Windows.Forms.ToolStripStatusLabel StatusLabel;
  }
}