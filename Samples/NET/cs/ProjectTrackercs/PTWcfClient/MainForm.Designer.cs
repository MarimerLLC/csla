namespace PTWcfClient
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
      this.label1 = new System.Windows.Forms.Label();
      this.ProjectListButton = new System.Windows.Forms.Button();
      this.RoleListButton = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.NewProjectButton = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(37, 41);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(91, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Get list of projects";
      // 
      // ProjectListButton
      // 
      this.ProjectListButton.Location = new System.Drawing.Point(228, 36);
      this.ProjectListButton.Name = "ProjectListButton";
      this.ProjectListButton.Size = new System.Drawing.Size(75, 23);
      this.ProjectListButton.TabIndex = 1;
      this.ProjectListButton.Text = "Go";
      this.ProjectListButton.UseVisualStyleBackColor = true;
      this.ProjectListButton.Click += new System.EventHandler(this.ProjectListButton_Click);
      // 
      // RoleListButton
      // 
      this.RoleListButton.Location = new System.Drawing.Point(228, 65);
      this.RoleListButton.Name = "RoleListButton";
      this.RoleListButton.Size = new System.Drawing.Size(75, 23);
      this.RoleListButton.TabIndex = 3;
      this.RoleListButton.Text = "Go";
      this.RoleListButton.UseVisualStyleBackColor = true;
      this.RoleListButton.Click += new System.EventHandler(this.RoleListButton_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(37, 70);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(76, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Get list of roles";
      // 
      // NewProjectButton
      // 
      this.NewProjectButton.Location = new System.Drawing.Point(228, 94);
      this.NewProjectButton.Name = "NewProjectButton";
      this.NewProjectButton.Size = new System.Drawing.Size(75, 23);
      this.NewProjectButton.TabIndex = 5;
      this.NewProjectButton.Text = "Go";
      this.NewProjectButton.UseVisualStyleBackColor = true;
      this.NewProjectButton.Click += new System.EventHandler(this.NewProjectButton_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(37, 99);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(84, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Add new project";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(715, 305);
      this.Controls.Add(this.NewProjectButton);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.RoleListButton);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.ProjectListButton);
      this.Controls.Add(this.label1);
      this.Name = "MainForm";
      this.Text = "MainForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button ProjectListButton;
    private System.Windows.Forms.Button RoleListButton;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button NewProjectButton;
    private System.Windows.Forms.Label label3;
  }
}