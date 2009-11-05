namespace PTWcfClient
{
  partial class Form3
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
      System.Windows.Forms.Label descriptionLabel;
      System.Windows.Forms.Label endedLabel;
      System.Windows.Forms.Label idLabel;
      System.Windows.Forms.Label nameLabel;
      System.Windows.Forms.Label startedLabel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
      this.projectDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.projectDataBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
      this.projectDataBindingNavigatorSaveItem = new System.Windows.Forms.ToolStripButton();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.endedTextBox = new System.Windows.Forms.TextBox();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.startedTextBox = new System.Windows.Forms.TextBox();
      descriptionLabel = new System.Windows.Forms.Label();
      endedLabel = new System.Windows.Forms.Label();
      idLabel = new System.Windows.Forms.Label();
      nameLabel = new System.Windows.Forms.Label();
      startedLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingNavigator)).BeginInit();
      this.projectDataBindingNavigator.SuspendLayout();
      this.SuspendLayout();
      // 
      // descriptionLabel
      // 
      descriptionLabel.AutoSize = true;
      descriptionLabel.Location = new System.Drawing.Point(12, 195);
      descriptionLabel.Name = "descriptionLabel";
      descriptionLabel.Size = new System.Drawing.Size(127, 26);
      descriptionLabel.TabIndex = 9;
      descriptionLabel.Text = "Description:";
      // 
      // endedLabel
      // 
      endedLabel.AutoSize = true;
      endedLabel.Location = new System.Drawing.Point(12, 157);
      endedLabel.Name = "endedLabel";
      endedLabel.Size = new System.Drawing.Size(81, 26);
      endedLabel.TabIndex = 7;
      endedLabel.Text = "Ended:";
      // 
      // idLabel
      // 
      idLabel.AutoSize = true;
      idLabel.Location = new System.Drawing.Point(12, 43);
      idLabel.Name = "idLabel";
      idLabel.Size = new System.Drawing.Size(36, 26);
      idLabel.TabIndex = 1;
      idLabel.Text = "Id:";
      // 
      // nameLabel
      // 
      nameLabel.AutoSize = true;
      nameLabel.Location = new System.Drawing.Point(12, 81);
      nameLabel.Name = "nameLabel";
      nameLabel.Size = new System.Drawing.Size(77, 26);
      nameLabel.TabIndex = 3;
      nameLabel.Text = "Name:";
      // 
      // startedLabel
      // 
      startedLabel.AutoSize = true;
      startedLabel.Location = new System.Drawing.Point(12, 119);
      startedLabel.Name = "startedLabel";
      startedLabel.Size = new System.Drawing.Size(88, 26);
      startedLabel.TabIndex = 5;
      startedLabel.Text = "Started:";
      // 
      // projectDataBindingSource
      // 
      this.projectDataBindingSource.DataSource = typeof(PTWcfClient.PTWcfService.ProjectData);
      // 
      // projectDataBindingNavigator
      // 
      this.projectDataBindingNavigator.AddNewItem = null;
      this.projectDataBindingNavigator.BindingSource = this.projectDataBindingSource;
      this.projectDataBindingNavigator.CountItem = null;
      this.projectDataBindingNavigator.DeleteItem = null;
      this.projectDataBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectDataBindingNavigatorSaveItem});
      this.projectDataBindingNavigator.Location = new System.Drawing.Point(0, 0);
      this.projectDataBindingNavigator.MoveFirstItem = null;
      this.projectDataBindingNavigator.MoveLastItem = null;
      this.projectDataBindingNavigator.MoveNextItem = null;
      this.projectDataBindingNavigator.MovePreviousItem = null;
      this.projectDataBindingNavigator.Name = "projectDataBindingNavigator";
      this.projectDataBindingNavigator.PositionItem = null;
      this.projectDataBindingNavigator.Size = new System.Drawing.Size(584, 25);
      this.projectDataBindingNavigator.TabIndex = 0;
      this.projectDataBindingNavigator.Text = "bindingNavigator1";
      // 
      // projectDataBindingNavigatorSaveItem
      // 
      this.projectDataBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.projectDataBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("projectDataBindingNavigatorSaveItem.Image")));
      this.projectDataBindingNavigatorSaveItem.Name = "projectDataBindingNavigatorSaveItem";
      this.projectDataBindingNavigatorSaveItem.Size = new System.Drawing.Size(23, 22);
      this.projectDataBindingNavigatorSaveItem.Text = "Save Data";
      this.projectDataBindingNavigatorSaveItem.Click += new System.EventHandler(this.projectDataBindingNavigatorSaveItem_Click);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectDataBindingSource, "Description", true));
      this.descriptionTextBox.Location = new System.Drawing.Point(145, 192);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.Size = new System.Drawing.Size(427, 308);
      this.descriptionTextBox.TabIndex = 10;
      // 
      // endedTextBox
      // 
      this.endedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectDataBindingSource, "Ended", true));
      this.endedTextBox.Location = new System.Drawing.Point(145, 154);
      this.endedTextBox.Name = "endedTextBox";
      this.endedTextBox.Size = new System.Drawing.Size(427, 32);
      this.endedTextBox.TabIndex = 8;
      // 
      // idTextBox
      // 
      this.idTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectDataBindingSource, "Id", true));
      this.idTextBox.Location = new System.Drawing.Point(145, 40);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.Size = new System.Drawing.Size(427, 32);
      this.idTextBox.TabIndex = 2;
      // 
      // nameTextBox
      // 
      this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectDataBindingSource, "Name", true));
      this.nameTextBox.Location = new System.Drawing.Point(145, 78);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(427, 32);
      this.nameTextBox.TabIndex = 4;
      // 
      // startedTextBox
      // 
      this.startedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.projectDataBindingSource, "Started", true));
      this.startedTextBox.Location = new System.Drawing.Point(145, 116);
      this.startedTextBox.Name = "startedTextBox";
      this.startedTextBox.Size = new System.Drawing.Size(427, 32);
      this.startedTextBox.TabIndex = 6;
      // 
      // Form3
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 512);
      this.Controls.Add(descriptionLabel);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(endedLabel);
      this.Controls.Add(this.endedTextBox);
      this.Controls.Add(idLabel);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(nameLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(startedLabel);
      this.Controls.Add(this.startedTextBox);
      this.Controls.Add(this.projectDataBindingNavigator);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "Form3";
      this.Text = "Form3";
      this.Load += new System.EventHandler(this.Form3_Load);
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.projectDataBindingNavigator)).EndInit();
      this.projectDataBindingNavigator.ResumeLayout(false);
      this.projectDataBindingNavigator.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource projectDataBindingSource;
    private System.Windows.Forms.BindingNavigator projectDataBindingNavigator;
    private System.Windows.Forms.ToolStripButton projectDataBindingNavigatorSaveItem;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.TextBox endedTextBox;
    private System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.TextBox startedTextBox;
  }
}