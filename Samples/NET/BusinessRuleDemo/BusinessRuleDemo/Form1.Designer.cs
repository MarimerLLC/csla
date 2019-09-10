namespace BusinessRuleDemo
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
      System.Windows.Forms.Label nameLabel;
      System.Windows.Forms.Label num1Label;
      System.Windows.Forms.Label num2Label;
      System.Windows.Forms.Label sumLabel;
      System.Windows.Forms.Label additionalInfoForUSLabel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      System.Windows.Forms.Label stateNameLabel;
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.rootBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.num1TextBox = new System.Windows.Forms.TextBox();
      this.num2TextBox = new System.Windows.Forms.TextBox();
      this.sumTextBox = new System.Windows.Forms.TextBox();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.countryNVLBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.comboBox2 = new System.Windows.Forms.ComboBox();
      this.statesNVLBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.readWriteAuthorization1 = new BusinessRuleDemo.Controls.ReadWriteAuthorization(this.components);
      this.additionalInfoForUSTextBox = new System.Windows.Forms.TextBox();
      this.stateNameTextBox = new System.Windows.Forms.TextBox();
      nameLabel = new System.Windows.Forms.Label();
      num1Label = new System.Windows.Forms.Label();
      num2Label = new System.Windows.Forms.Label();
      sumLabel = new System.Windows.Forms.Label();
      additionalInfoForUSLabel = new System.Windows.Forms.Label();
      stateNameLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.rootBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.countryNVLBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.statesNVLBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(nameLabel, false);
      nameLabel.AutoSize = true;
      nameLabel.Location = new System.Drawing.Point(22, 36);
      nameLabel.Name = "nameLabel";
      nameLabel.Size = new System.Drawing.Size(38, 13);
      nameLabel.TabIndex = 3;
      nameLabel.Text = "Name:";
      // 
      // num1Label
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(num1Label, false);
      num1Label.AutoSize = true;
      num1Label.Location = new System.Drawing.Point(22, 89);
      num1Label.Name = "num1Label";
      num1Label.Size = new System.Drawing.Size(38, 13);
      num1Label.TabIndex = 5;
      num1Label.Text = "Num1:";
      // 
      // num2Label
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(num2Label, false);
      num2Label.AutoSize = true;
      num2Label.Location = new System.Drawing.Point(22, 115);
      num2Label.Name = "num2Label";
      num2Label.Size = new System.Drawing.Size(38, 13);
      num2Label.TabIndex = 7;
      num2Label.Text = "Num2:";
      // 
      // sumLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(sumLabel, false);
      sumLabel.AutoSize = true;
      sumLabel.Location = new System.Drawing.Point(22, 141);
      sumLabel.Name = "sumLabel";
      sumLabel.Size = new System.Drawing.Size(31, 13);
      sumLabel.TabIndex = 9;
      sumLabel.Text = "Sum:";
      // 
      // additionalInfoForUSLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(additionalInfoForUSLabel, false);
      additionalInfoForUSLabel.AutoSize = true;
      additionalInfoForUSLabel.Location = new System.Drawing.Point(24, 262);
      additionalInfoForUSLabel.Name = "additionalInfoForUSLabel";
      additionalInfoForUSLabel.Size = new System.Drawing.Size(113, 13);
      additionalInfoForUSLabel.TabIndex = 15;
      additionalInfoForUSLabel.Text = "Additional Info For US:";
      // 
      // nameTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.nameTextBox, false);
      this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Name", true));
      this.nameTextBox.Location = new System.Drawing.Point(72, 33);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(104, 20);
      this.nameTextBox.TabIndex = 4;
      // 
      // rootBindingSource
      // 
      this.rootBindingSource.DataSource = typeof(BusinessRuleDemo.Root);
      this.rootBindingSource.CurrentItemChanged += new System.EventHandler(this.rootBindingSource_CurrentItemChanged);
      // 
      // num1TextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.num1TextBox, false);
      this.num1TextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Num1", true));
      this.num1TextBox.Location = new System.Drawing.Point(72, 86);
      this.num1TextBox.Name = "num1TextBox";
      this.num1TextBox.Size = new System.Drawing.Size(104, 20);
      this.num1TextBox.TabIndex = 6;
      // 
      // num2TextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.num2TextBox, false);
      this.num2TextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Num2", true));
      this.num2TextBox.Location = new System.Drawing.Point(72, 112);
      this.num2TextBox.Name = "num2TextBox";
      this.num2TextBox.Size = new System.Drawing.Size(104, 20);
      this.num2TextBox.TabIndex = 8;
      // 
      // sumTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.sumTextBox, false);
      this.sumTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "Sum", true));
      this.sumTextBox.Location = new System.Drawing.Point(72, 138);
      this.sumTextBox.Name = "sumTextBox";
      this.sumTextBox.Size = new System.Drawing.Size(104, 20);
      this.sumTextBox.TabIndex = 10;
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      this.errorProvider1.DataSource = this.rootBindingSource;
      // 
      // textBox1
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.textBox1, false);
      this.textBox1.Location = new System.Drawing.Point(413, 33);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(262, 246);
      this.textBox1.TabIndex = 11;
      this.textBox1.Text = resources.GetString("textBox1.Text");
      // 
      // label1
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.label1, false);
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(25, 185);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 12;
      this.label1.Text = "Country:";
      // 
      // label2
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.label2, false);
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(25, 214);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 13;
      this.label2.Text = "State:";
      // 
      // comboBox1
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.comboBox1, false);
      this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.rootBindingSource, "Country", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.comboBox1.DataSource = this.countryNVLBindingSource;
      this.comboBox1.DisplayMember = "Value";
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(72, 185);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(265, 21);
      this.comboBox1.TabIndex = 14;
      this.comboBox1.ValueMember = "Key";
      // 
      // countryNVLBindingSource
      // 
      this.countryNVLBindingSource.DataSource = typeof(BusinessRuleDemo.CountryNVL);
      // 
      // comboBox2
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.comboBox2, true);
      this.comboBox2.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.rootBindingSource, "State", true));
      this.comboBox2.DataSource = this.statesNVLBindingSource;
      this.comboBox2.DisplayMember = "Value";
      this.comboBox2.FormattingEnabled = true;
      this.comboBox2.Location = new System.Drawing.Point(72, 214);
      this.comboBox2.Name = "comboBox2";
      this.comboBox2.Size = new System.Drawing.Size(265, 21);
      this.comboBox2.TabIndex = 15;
      this.comboBox2.ValueMember = "Key";
      // 
      // statesNVLBindingSource
      // 
      this.statesNVLBindingSource.DataSource = typeof(BusinessRuleDemo.StatesNVL);
      // 
      // additionalInfoForUSTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.additionalInfoForUSTextBox, false);
      this.additionalInfoForUSTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "AdditionalInfoForUS", true));
      this.additionalInfoForUSTextBox.Location = new System.Drawing.Point(153, 259);
      this.additionalInfoForUSTextBox.Name = "additionalInfoForUSTextBox";
      this.additionalInfoForUSTextBox.Size = new System.Drawing.Size(184, 20);
      this.additionalInfoForUSTextBox.TabIndex = 16;
      // 
      // stateNameLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(stateNameLabel, false);
      stateNameLabel.AutoSize = true;
      stateNameLabel.Location = new System.Drawing.Point(199, 146);
      stateNameLabel.Name = "stateNameLabel";
      stateNameLabel.Size = new System.Drawing.Size(66, 13);
      stateNameLabel.TabIndex = 16;
      stateNameLabel.Text = "State Name:";
      // 
      // stateNameTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.stateNameTextBox, false);
      this.stateNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.rootBindingSource, "StateName", true));
      this.stateNameTextBox.Location = new System.Drawing.Point(271, 143);
      this.stateNameTextBox.Name = "stateNameTextBox";
      this.stateNameTextBox.Size = new System.Drawing.Size(100, 20);
      this.stateNameTextBox.TabIndex = 17;
      // 
      // Form1
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this, false);
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(732, 304);
      this.Controls.Add(stateNameLabel);
      this.Controls.Add(this.stateNameTextBox);
      this.Controls.Add(additionalInfoForUSLabel);
      this.Controls.Add(this.additionalInfoForUSTextBox);
      this.Controls.Add(this.comboBox2);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(nameLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(num1Label);
      this.Controls.Add(this.num1TextBox);
      this.Controls.Add(num2Label);
      this.Controls.Add(this.num2TextBox);
      this.Controls.Add(sumLabel);
      this.Controls.Add(this.sumTextBox);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.rootBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.countryNVLBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.statesNVLBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource rootBindingSource;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.TextBox num1TextBox;
    private System.Windows.Forms.TextBox num2TextBox;
    private System.Windows.Forms.TextBox sumTextBox;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.ComboBox comboBox2;
    private System.Windows.Forms.BindingSource countryNVLBindingSource;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.BindingSource statesNVLBindingSource;
    private Controls.ReadWriteAuthorization readWriteAuthorization1;
    private System.Windows.Forms.TextBox additionalInfoForUSTextBox;
    private System.Windows.Forms.TextBox stateNameTextBox;
  }
}

