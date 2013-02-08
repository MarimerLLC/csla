namespace MultipleBindingSources
{
  partial class SingleRootDualBindingSorcesFrom
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components;

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
      System.Windows.Forms.Label address1Label;
      System.Windows.Forms.Label idLabel;
      System.Windows.Forms.Label nameLabel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleRootDualBindingSorcesFrom));
      this.rootBindingNavigator = new System.Windows.Forms.BindingNavigator();
      this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
      this.rootBindingSource = new System.Windows.Forms.BindingSource();
      this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
      this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
      this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
      this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
      this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
      this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
      this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
      this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
      this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.rootBindingNavigatorSaveItem = new System.Windows.Forms.ToolStripButton();
      this.address1TextBox = new System.Windows.Forms.TextBox();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.rootBindingSource2 = new System.Windows.Forms.BindingSource();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh();
      address1Label = new System.Windows.Forms.Label();
      idLabel = new System.Windows.Forms.Label();
      nameLabel = new System.Windows.Forms.Label();
      this.rootBindingNavigator.SuspendLayout();
      this.SuspendLayout();
      // 
      // address1Label
      // 
      address1Label.AutoSize = true;
      address1Label.Location = new System.Drawing.Point(27, 49);
      address1Label.Name = "address1Label";
      address1Label.Size = new System.Drawing.Size(54, 13);
      address1Label.TabIndex = 1;
      address1Label.Text = "Address1:";
      // 
      // idLabel
      // 
      idLabel.AutoSize = true;
      idLabel.Location = new System.Drawing.Point(27, 75);
      idLabel.Name = "idLabel";
      idLabel.Size = new System.Drawing.Size(19, 13);
      idLabel.TabIndex = 3;
      idLabel.Text = "Id:";
      // 
      // nameLabel
      // 
      nameLabel.AutoSize = true;
      nameLabel.Location = new System.Drawing.Point(27, 101);
      nameLabel.Name = "nameLabel";
      nameLabel.Size = new System.Drawing.Size(38, 13);
      nameLabel.TabIndex = 7;
      nameLabel.Text = "Name:";
      // 
      // rootBindingNavigator
      // 
      this.rootBindingNavigator.AddNewItem = this.bindingNavigatorAddNewItem;
      this.rootBindingNavigator.BindingSource = this.rootBindingSource;
      this.rootBindingNavigator.CountItem = this.bindingNavigatorCountItem;
      this.rootBindingNavigator.DeleteItem = this.bindingNavigatorDeleteItem;
      this.rootBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.rootBindingNavigatorSaveItem});
      this.rootBindingNavigator.Location = new System.Drawing.Point(0, 0);
      this.rootBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
      this.rootBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
      this.rootBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
      this.rootBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
      this.rootBindingNavigator.Name = "rootBindingNavigator";
      this.rootBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
      this.rootBindingNavigator.Size = new System.Drawing.Size(341, 25);
      this.rootBindingNavigator.TabIndex = 0;
      this.rootBindingNavigator.Text = "bindingNavigator1";
      // 
      // bindingNavigatorAddNewItem
      // 
      this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
      this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
      this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorAddNewItem.Text = "Add new";
      // 
      // rootBindingSource
      // 
      this.rootBindingSource.DataSource = typeof(MultipleBindingSources.Root);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.rootBindingSource, true);
      // 
      // bindingNavigatorCountItem
      // 
      this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
      this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
      this.bindingNavigatorCountItem.Text = "of {0}";
      this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
      // 
      // bindingNavigatorDeleteItem
      // 
      this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
      this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
      this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorDeleteItem.Text = "Delete";
      // 
      // bindingNavigatorMoveFirstItem
      // 
      this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
      this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
      this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorMoveFirstItem.Text = "Move first";
      // 
      // bindingNavigatorMovePreviousItem
      // 
      this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
      this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
      this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorMovePreviousItem.Text = "Move previous";
      // 
      // bindingNavigatorSeparator
      // 
      this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
      this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
      // 
      // bindingNavigatorPositionItem
      // 
      this.bindingNavigatorPositionItem.AccessibleName = "Position";
      this.bindingNavigatorPositionItem.AutoSize = false;
      this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
      this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
      this.bindingNavigatorPositionItem.Text = "0";
      this.bindingNavigatorPositionItem.ToolTipText = "Current position";
      // 
      // bindingNavigatorSeparator1
      // 
      this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
      this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // bindingNavigatorMoveNextItem
      // 
      this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
      this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
      this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorMoveNextItem.Text = "Move next";
      // 
      // bindingNavigatorMoveLastItem
      // 
      this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
      this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
      this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
      this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
      this.bindingNavigatorMoveLastItem.Text = "Move last";
      // 
      // bindingNavigatorSeparator2
      // 
      this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
      this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // rootBindingNavigatorSaveItem
      // 
      this.rootBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.rootBindingNavigatorSaveItem.Enabled = false;
      this.rootBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("rootBindingNavigatorSaveItem.Image")));
      this.rootBindingNavigatorSaveItem.Name = "rootBindingNavigatorSaveItem";
      this.rootBindingNavigatorSaveItem.Size = new System.Drawing.Size(23, 22);
      this.rootBindingNavigatorSaveItem.Text = "Save Data";
      // 
      // address1TextBox
      // 
      this.address1TextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Address1", true));
      this.address1TextBox.Location = new System.Drawing.Point(87, 46);
      this.address1TextBox.Name = "address1TextBox";
      this.address1TextBox.Size = new System.Drawing.Size(100, 20);
      this.address1TextBox.TabIndex = 2;
      // 
      // idTextBox
      // 
      this.idTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Id", true));
      this.idTextBox.Location = new System.Drawing.Point(87, 72);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.Size = new System.Drawing.Size(100, 20);
      this.idTextBox.TabIndex = 4;
      // 
      // nameTextBox
      // 
      this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Name", true));
      this.nameTextBox.Location = new System.Drawing.Point(87, 98);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(100, 20);
      this.nameTextBox.TabIndex = 8;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(30, 168);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 9;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // rootBindingSource2
      // 
      this.rootBindingSource2.DataSource = typeof(MultipleBindingSources.Root);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.rootBindingSource2, true);
      // 
      // textBox1
      // 
      this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource2, "Name", true));
      this.textBox1.Location = new System.Drawing.Point(209, 98);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(100, 20);
      this.textBox1.TabIndex = 12;
      // 
      // textBox2
      // 
      this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource2, "Address1", true));
      this.textBox2.Location = new System.Drawing.Point(209, 46);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(100, 20);
      this.textBox2.TabIndex = 10;
      // 
      // textBox3
      // 
      this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource2, "Id", true));
      this.textBox3.Location = new System.Drawing.Point(209, 72);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new System.Drawing.Size(100, 20);
      this.textBox3.TabIndex = 11;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(218, 168);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 13;
      this.button2.Text = "debugInfo";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // SingleRootDualBindingSorcesFrom
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(341, 266);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.textBox2);
      this.Controls.Add(this.textBox3);
      this.Controls.Add(this.button1);
      this.Controls.Add(nameLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(address1Label);
      this.Controls.Add(this.address1TextBox);
      this.Controls.Add(idLabel);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.rootBindingNavigator);
      this.Name = "SingleRootDualBindingSorcesFrom";
      this.Text = "Form1";
      this.rootBindingNavigator.ResumeLayout(false);
      this.rootBindingNavigator.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource rootBindingSource;
    private System.Windows.Forms.BindingNavigator rootBindingNavigator;
    private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
    private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
    private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
    private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
    private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
    private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
    private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
    private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
    private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
    private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
    private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
    private System.Windows.Forms.ToolStripButton rootBindingNavigatorSaveItem;
    private System.Windows.Forms.TextBox address1TextBox;
    private System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.BindingSource rootBindingSource2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Button button2;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
  }
}

