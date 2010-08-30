namespace WindowsApplication2
{
  partial class Form1
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
      System.Windows.Forms.Label idLabel;
      System.Windows.Forms.Label nameLabel;
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.childrenBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.childrenDataGridView = new System.Windows.Forms.DataGridView();
      this.grandchildrenBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.grandchildrenDataGridView = new System.Windows.Forms.DataGridView();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rootBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      idLabel = new System.Windows.Forms.Label();
      nameLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.childrenBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.childrenDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.grandchildrenBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.grandchildrenDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.rootBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // idLabel
      // 
      idLabel.AutoSize = true;
      idLabel.Location = new System.Drawing.Point(12, 21);
      idLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      idLabel.Name = "idLabel";
      idLabel.Size = new System.Drawing.Size(36, 26);
      idLabel.TabIndex = 1;
      idLabel.Text = "Id:";
      // 
      // nameLabel
      // 
      nameLabel.AutoSize = true;
      nameLabel.Location = new System.Drawing.Point(12, 71);
      nameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      nameLabel.Name = "nameLabel";
      nameLabel.Size = new System.Drawing.Size(77, 26);
      nameLabel.TabIndex = 3;
      nameLabel.Text = "Name:";
      // 
      // idTextBox
      // 
      this.idTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Id", true));
      this.idTextBox.Location = new System.Drawing.Point(100, 15);
      this.idTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.Size = new System.Drawing.Size(196, 32);
      this.idTextBox.TabIndex = 2;
      // 
      // nameTextBox
      // 
      this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Name", true));
      this.nameTextBox.Location = new System.Drawing.Point(100, 65);
      this.nameTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(196, 32);
      this.nameTextBox.TabIndex = 4;
      // 
      // childrenBindingSource
      // 
      this.childrenBindingSource.DataMember = "Children";
      this.childrenBindingSource.DataSource = this.rootBindingSource;
      // 
      // childrenDataGridView
      // 
      this.childrenDataGridView.AutoGenerateColumns = false;
      this.childrenDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.childrenDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
      this.childrenDataGridView.ColumnHeadersHeight = 32;
      this.childrenDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
      this.childrenDataGridView.DataSource = this.childrenBindingSource;
      this.childrenDataGridView.Location = new System.Drawing.Point(18, 138);
      this.childrenDataGridView.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.childrenDataGridView.Name = "childrenDataGridView";
      this.childrenDataGridView.RowTemplate.Height = 32;
      this.childrenDataGridView.Size = new System.Drawing.Size(409, 254);
      this.childrenDataGridView.TabIndex = 5;
      // 
      // grandchildrenBindingSource
      // 
      this.grandchildrenBindingSource.DataMember = "Grandchildren";
      this.grandchildrenBindingSource.DataSource = this.childrenBindingSource;
      // 
      // grandchildrenDataGridView
      // 
      this.grandchildrenDataGridView.AutoGenerateColumns = false;
      this.grandchildrenDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.grandchildrenDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
      this.grandchildrenDataGridView.ColumnHeadersHeight = 32;
      this.grandchildrenDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
      this.grandchildrenDataGridView.DataSource = this.grandchildrenBindingSource;
      this.grandchildrenDataGridView.Location = new System.Drawing.Point(17, 404);
      this.grandchildrenDataGridView.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.grandchildrenDataGridView.Name = "grandchildrenDataGridView";
      this.grandchildrenDataGridView.RowTemplate.Height = 32;
      this.grandchildrenDataGridView.Size = new System.Drawing.Size(410, 254);
      this.grandchildrenDataGridView.TabIndex = 6;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(636, 15);
      this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(150, 44);
      this.button1.TabIndex = 7;
      this.button1.Text = "Apply";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.ApplyButton_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(636, 65);
      this.button2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(150, 44);
      this.button2.TabIndex = 8;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.CancelButton_Click);
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.DataPropertyName = "Id";
      this.dataGridViewTextBoxColumn3.HeaderText = "Id";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.Width = 55;
      // 
      // dataGridViewTextBoxColumn4
      // 
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn4.HeaderText = "Name";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.Width = 96;
      // 
      // rootBindingSource
      // 
      this.rootBindingSource.DataSource = typeof(WindowsApplication2.Root);
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Id";
      this.dataGridViewTextBoxColumn1.HeaderText = "Id";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Width = 55;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Name";
      this.dataGridViewTextBoxColumn2.HeaderText = "Name";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.Width = 96;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1424, 864);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.grandchildrenDataGridView);
      this.Controls.Add(this.childrenDataGridView);
      this.Controls.Add(idLabel);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(nameLabel);
      this.Controls.Add(this.nameTextBox);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.childrenBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.childrenDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.grandchildrenBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.grandchildrenDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.rootBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource rootBindingSource;
    private System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.BindingSource childrenBindingSource;
    private System.Windows.Forms.DataGridView childrenDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.BindingSource grandchildrenBindingSource;
    private System.Windows.Forms.DataGridView grandchildrenDataGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
  }
}

