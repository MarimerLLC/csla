namespace ProjectsVendors.WinForms
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
            this.selectProject = new System.Windows.Forms.Button();
            this.workspace = new System.Windows.Forms.Panel();
            this.newProject = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectProject
            // 
            this.selectProject.Location = new System.Drawing.Point(10, 10);
            this.selectProject.Name = "selectProject";
            this.selectProject.Size = new System.Drawing.Size(116, 23);
            this.selectProject.TabIndex = 2;
            this.selectProject.Text = "Select Project";
            this.selectProject.UseVisualStyleBackColor = true;
            this.selectProject.Click += new System.EventHandler(this.selectProject_Click);
            // 
            // workspace
            // 
            this.workspace.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.workspace.Location = new System.Drawing.Point(0, 44);
            this.workspace.Name = "workspace";
            this.workspace.Size = new System.Drawing.Size(914, 350);
            this.workspace.TabIndex = 3;
            // 
            // newProject
            // 
            this.newProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newProject.Location = new System.Drawing.Point(145, 10);
            this.newProject.Name = "newProject";
            this.newProject.Size = new System.Drawing.Size(116, 23);
            this.newProject.TabIndex = 4;
            this.newProject.Text = "New Project";
            this.newProject.UseVisualStyleBackColor = true;
            this.newProject.Click += new System.EventHandler(this.newProject_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.selectProject;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 394);
            this.Controls.Add(this.newProject);
            this.Controls.Add(this.workspace);
            this.Controls.Add(this.selectProject);
            this.MaximumSize = new System.Drawing.Size(930, 433);
            this.MinimumSize = new System.Drawing.Size(930, 433);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button selectProject;
        private System.Windows.Forms.Panel workspace;
        private System.Windows.Forms.Button newProject;
    }
}
