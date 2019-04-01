namespace PTWin
{
  partial class ProjectSelect
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
      this.components = new System.ComponentModel.Container();
      this.GetListButton = new System.Windows.Forms.Button();
      this.NameTextBox = new System.Windows.Forms.TextBox();
      this.NameLabel = new System.Windows.Forms.Label();
      this.ProjectListListBox = new System.Windows.Forms.ListBox();
      this.projectListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.OK_Button = new System.Windows.Forms.Button();
      this.Cancel_Button = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.projectListBindingSource)).BeginInit();
      this.TableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // GetListButton
      // 
      this.GetListButton.Location = new System.Drawing.Point(356, 9);
      this.GetListButton.Name = "GetListButton";
      this.GetListButton.Size = new System.Drawing.Size(75, 23);
      this.GetListButton.TabIndex = 2;
      this.GetListButton.Text = "Get list";
      this.GetListButton.UseVisualStyleBackColor = true;
      this.GetListButton.Click += new System.EventHandler(this.GetListButton_Click);
      // 
      // NameTextBox
      // 
      this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.NameTextBox.Location = new System.Drawing.Point(83, 9);
      this.NameTextBox.Name = "NameTextBox";
      this.NameTextBox.Size = new System.Drawing.Size(267, 20);
      this.NameTextBox.TabIndex = 1;
      // 
      // NameLabel
      // 
      this.NameLabel.AutoSize = true;
      this.NameLabel.Location = new System.Drawing.Point(12, 12);
      this.NameLabel.Name = "NameLabel";
      this.NameLabel.Size = new System.Drawing.Size(38, 13);
      this.NameLabel.TabIndex = 0;
      this.NameLabel.Text = "Name:";
      // 
      // ProjectListListBox
      // 
      this.ProjectListListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ProjectListListBox.DataSource = this.projectListBindingSource;
      this.ProjectListListBox.DisplayMember = "Name";
      this.ProjectListListBox.Location = new System.Drawing.Point(12, 41);
      this.ProjectListListBox.Name = "ProjectListListBox";
      this.ProjectListListBox.Size = new System.Drawing.Size(419, 238);
      this.ProjectListListBox.TabIndex = 3;
      this.ProjectListListBox.ValueMember = "Id";
      this.ProjectListListBox.DoubleClick += new System.EventHandler(this.ProjectListListBox_DoubleClick);
      // 
      // projectListBindingSource
      // 
      this.projectListBindingSource.DataSource = typeof(ProjectTracker.Library.ProjectInfo);
      // 
      // TableLayoutPanel1
      // 
      this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.TableLayoutPanel1.ColumnCount = 2;
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
      this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
      this.TableLayoutPanel1.Location = new System.Drawing.Point(285, 285);
      this.TableLayoutPanel1.Name = "TableLayoutPanel1";
      this.TableLayoutPanel1.RowCount = 1;
      this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
      this.TableLayoutPanel1.TabIndex = 4;
      // 
      // OK_Button
      // 
      this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OK_Button.Location = new System.Drawing.Point(3, 3);
      this.OK_Button.Name = "OK_Button";
      this.OK_Button.Size = new System.Drawing.Size(67, 23);
      this.OK_Button.TabIndex = 0;
      this.OK_Button.Text = "OK";
      this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
      this.Cancel_Button.TabIndex = 1;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // ProjectSelect
      // 
      this.AcceptButton = this.OK_Button;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.Cancel_Button;
      this.ClientSize = new System.Drawing.Size(443, 323);
      this.Controls.Add(this.GetListButton);
      this.Controls.Add(this.NameTextBox);
      this.Controls.Add(this.NameLabel);
      this.Controls.Add(this.ProjectListListBox);
      this.Controls.Add(this.TableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ProjectSelect";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select Project";
      this.Load += new System.EventHandler(this.ProjectSelect_Load);
      ((System.ComponentModel.ISupportInitialize)(this.projectListBindingSource)).EndInit();
      this.TableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button GetListButton;
    internal System.Windows.Forms.TextBox NameTextBox;
    internal System.Windows.Forms.Label NameLabel;
    internal System.Windows.Forms.ListBox ProjectListListBox;
    internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
    internal System.Windows.Forms.Button OK_Button;
    internal System.Windows.Forms.Button Cancel_Button;
    private System.Windows.Forms.BindingSource projectListBindingSource;
  }
}