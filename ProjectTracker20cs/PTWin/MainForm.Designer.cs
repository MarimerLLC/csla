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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.Label descriptionLabel;
      System.Windows.Forms.Label endedLabel;
      System.Windows.Forms.Label idLabel;
      System.Windows.Forms.Label nameLabel;
      System.Windows.Forms.Label startedLabel;
      this.projectListDataGridView = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.projectListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.projectBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.endedTextBox = new System.Windows.Forms.TextBox();
      this.idLabel1 = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.startedTextBox = new System.Windows.Forms.TextBox();
      descriptionLabel = new System.Windows.Forms.Label();
      endedLabel = new System.Windows.Forms.Label();
      idLabel = new System.Windows.Forms.Label();
      nameLabel = new System.Windows.Forms.Label();
      startedLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // projectListDataGridView
      // 
      this.projectListDataGridView.AllowUserToAddRows = false;
      this.projectListDataGridView.AllowUserToDeleteRows = false;
      dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
      this.projectListDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
      this.projectListDataGridView.AutoGenerateColumns = false;
      this.projectListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.projectListDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      this.projectListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
      this.projectListDataGridView.DataSource = this.projectListBindingSource;
      this.projectListDataGridView.Location = new System.Drawing.Point(12, 12);
      this.projectListDataGridView.MultiSelect = false;
      this.projectListDataGridView.Name = "projectListDataGridView";
      this.projectListDataGridView.ReadOnly = true;
      this.projectListDataGridView.RowHeadersVisible = false;
      this.projectListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.projectListDataGridView.Size = new System.Drawing.Size(369, 414);
      this.projectListDataGridView.TabIndex = 1;
      this.projectListDataGridView.SelectionChanged += new System.EventHandler(this.projectListDataGridView_SelectionChanged);
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Id";
      this.dataGridViewTextBoxColumn1.HeaderText = "Id";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 41;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn2.HeaderText = "Name";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      this.dataGridViewTextBoxColumn2.Width = 60;
      // 
      // projectListBindingSource
      // 
      this.projectListBindingSource.DataSource = typeof(ProjectTracker.Library.ProjectList);
      // 
      // projectBindingSource
      // 
      this.projectBindingSource.DataSource = typeof(ProjectTracker.Library.Project);
      // 
      // descriptionLabel
      // 
      descriptionLabel.AutoSize = true;
      descriptionLabel.Location = new System.Drawing.Point(417, 178);
      descriptionLabel.Name = "descriptionLabel";
      descriptionLabel.Size = new System.Drawing.Size(63, 13);
      descriptionLabel.TabIndex = 1;
      descriptionLabel.Text = "Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectBindingSource, "Description", true));
      this.descriptionTextBox.Location = new System.Drawing.Point(486, 175);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.Size = new System.Drawing.Size(295, 163);
      this.descriptionTextBox.TabIndex = 2;
      // 
      // endedLabel
      // 
      endedLabel.AutoSize = true;
      endedLabel.Location = new System.Drawing.Point(417, 152);
      endedLabel.Name = "endedLabel";
      endedLabel.Size = new System.Drawing.Size(41, 13);
      endedLabel.TabIndex = 3;
      endedLabel.Text = "Ended:";
      // 
      // endedTextBox
      // 
      this.endedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectBindingSource, "Ended", true));
      this.endedTextBox.Location = new System.Drawing.Point(486, 149);
      this.endedTextBox.Name = "endedTextBox";
      this.endedTextBox.Size = new System.Drawing.Size(100, 20);
      this.endedTextBox.TabIndex = 4;
      // 
      // idLabel
      // 
      idLabel.AutoSize = true;
      idLabel.Location = new System.Drawing.Point(417, 71);
      idLabel.Name = "idLabel";
      idLabel.Size = new System.Drawing.Size(19, 13);
      idLabel.TabIndex = 5;
      idLabel.Text = "Id:";
      // 
      // idLabel1
      // 
      this.idLabel1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectBindingSource, "Id", true));
      this.idLabel1.Location = new System.Drawing.Point(483, 71);
      this.idLabel1.Name = "idLabel1";
      this.idLabel1.Size = new System.Drawing.Size(100, 23);
      this.idLabel1.TabIndex = 6;
      // 
      // nameLabel
      // 
      nameLabel.AutoSize = true;
      nameLabel.Location = new System.Drawing.Point(417, 100);
      nameLabel.Name = "nameLabel";
      nameLabel.Size = new System.Drawing.Size(38, 13);
      nameLabel.TabIndex = 7;
      nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectBindingSource, "Name", true));
      this.nameTextBox.Location = new System.Drawing.Point(486, 97);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(100, 20);
      this.nameTextBox.TabIndex = 8;
      // 
      // startedLabel
      // 
      startedLabel.AutoSize = true;
      startedLabel.Location = new System.Drawing.Point(417, 126);
      startedLabel.Name = "startedLabel";
      startedLabel.Size = new System.Drawing.Size(44, 13);
      startedLabel.TabIndex = 9;
      startedLabel.Text = "Started:";
      // 
      // startedTextBox
      // 
      this.startedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectBindingSource, "Started", true));
      this.startedTextBox.Location = new System.Drawing.Point(486, 123);
      this.startedTextBox.Name = "startedTextBox";
      this.startedTextBox.Size = new System.Drawing.Size(100, 20);
      this.startedTextBox.TabIndex = 10;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(804, 446);
      this.Controls.Add(descriptionLabel);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(endedLabel);
      this.Controls.Add(this.endedTextBox);
      this.Controls.Add(idLabel);
      this.Controls.Add(this.idLabel1);
      this.Controls.Add(nameLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(startedLabel);
      this.Controls.Add(this.startedTextBox);
      this.Controls.Add(this.projectListDataGridView);
      this.Name = "MainForm";
      this.Text = "MainForm";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource projectListBindingSource;
    private System.Windows.Forms.DataGridView projectListDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.BindingSource projectBindingSource;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.TextBox endedTextBox;
    private System.Windows.Forms.Label idLabel1;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.TextBox startedTextBox;

  }
}