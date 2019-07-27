namespace Csla.Test.Windows
{
    partial class PersonForm
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
      System.Windows.Forms.Label authLevelLabel;
      System.Windows.Forms.Label firstNameLabel;
      System.Windows.Forms.Label lastNameLabel;
      System.Windows.Forms.Label middleNameLabel;
      System.Windows.Forms.Label placeOfBirthLabel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonForm));
      this.editablePersonBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.authLevelTextBox = new System.Windows.Forms.TextBox();
      this.firstNameTextBox = new System.Windows.Forms.TextBox();
      this.lastNameTextBox = new System.Windows.Forms.TextBox();
      this.middleNameTextBox = new System.Windows.Forms.TextBox();
      this.placeOfBirthTextBox = new System.Windows.Forms.TextBox();
      this.readWriteAuthorization1 = new Csla.Windows.ReadWriteAuthorization(this.components);
      this.SaveButton = new System.Windows.Forms.Button();
      this.CloseButton = new System.Windows.Forms.Button();
      this.CancelButton = new System.Windows.Forms.Button();
      this.ValidateButton = new System.Windows.Forms.Button();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.cancelToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.validateToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.cslaActionExtender1 = new Csla.Windows.CslaActionExtender(this.components);
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      this.cslaActionExtenderToolStrip1 = new Csla.Windows.CslaActionExtenderToolStrip(this.components);
      authLevelLabel = new System.Windows.Forms.Label();
      firstNameLabel = new System.Windows.Forms.Label();
      lastNameLabel = new System.Windows.Forms.Label();
      middleNameLabel = new System.Windows.Forms.Label();
      placeOfBirthLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.editablePersonBindingSource)).BeginInit();
      this.toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // authLevelLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(authLevelLabel, false);
      authLevelLabel.AutoSize = true;
      authLevelLabel.Location = new System.Drawing.Point(36, 27);
      authLevelLabel.Name = "authLevelLabel";
      authLevelLabel.Size = new System.Drawing.Size(61, 13);
      authLevelLabel.TabIndex = 1;
      authLevelLabel.Text = "Auth Level:";
      // 
      // firstNameLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(firstNameLabel, false);
      firstNameLabel.AutoSize = true;
      firstNameLabel.Location = new System.Drawing.Point(36, 53);
      firstNameLabel.Name = "firstNameLabel";
      firstNameLabel.Size = new System.Drawing.Size(60, 13);
      firstNameLabel.TabIndex = 3;
      firstNameLabel.Text = "First Name:";
      // 
      // lastNameLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(lastNameLabel, false);
      lastNameLabel.AutoSize = true;
      lastNameLabel.Location = new System.Drawing.Point(36, 79);
      lastNameLabel.Name = "lastNameLabel";
      lastNameLabel.Size = new System.Drawing.Size(61, 13);
      lastNameLabel.TabIndex = 5;
      lastNameLabel.Text = "Last Name:";
      // 
      // middleNameLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(middleNameLabel, false);
      middleNameLabel.AutoSize = true;
      middleNameLabel.Location = new System.Drawing.Point(36, 105);
      middleNameLabel.Name = "middleNameLabel";
      middleNameLabel.Size = new System.Drawing.Size(72, 13);
      middleNameLabel.TabIndex = 7;
      middleNameLabel.Text = "Middle Name:";
      // 
      // placeOfBirthLabel
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(placeOfBirthLabel, false);
      placeOfBirthLabel.AutoSize = true;
      placeOfBirthLabel.Location = new System.Drawing.Point(36, 131);
      placeOfBirthLabel.Name = "placeOfBirthLabel";
      placeOfBirthLabel.Size = new System.Drawing.Size(75, 13);
      placeOfBirthLabel.TabIndex = 9;
      placeOfBirthLabel.Text = "Place Of Birth:";
      // 
      // editablePersonBindingSource
      // 
      this.editablePersonBindingSource.DataSource = typeof(Csla.Test.Windows.EditablePerson);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.editablePersonBindingSource, true);
      this.editablePersonBindingSource.CurrentItemChanged += new System.EventHandler(this.editablePersonBindingSource_CurrentItemChanged);
      // 
      // authLevelTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.authLevelTextBox, false);
      this.authLevelTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "AuthLevel", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.authLevelTextBox.Location = new System.Drawing.Point(117, 24);
      this.authLevelTextBox.Name = "authLevelTextBox";
      this.authLevelTextBox.Size = new System.Drawing.Size(100, 20);
      this.authLevelTextBox.TabIndex = 2;
      // 
      // firstNameTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.firstNameTextBox, false);
      this.firstNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "FirstName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.firstNameTextBox.Location = new System.Drawing.Point(117, 50);
      this.firstNameTextBox.Name = "firstNameTextBox";
      this.firstNameTextBox.Size = new System.Drawing.Size(100, 20);
      this.firstNameTextBox.TabIndex = 4;
      // 
      // lastNameTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.lastNameTextBox, false);
      this.lastNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "LastName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.lastNameTextBox.Location = new System.Drawing.Point(117, 76);
      this.lastNameTextBox.Name = "lastNameTextBox";
      this.lastNameTextBox.Size = new System.Drawing.Size(100, 20);
      this.lastNameTextBox.TabIndex = 6;
      // 
      // middleNameTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.middleNameTextBox, false);
      this.middleNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "MiddleName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.middleNameTextBox.Location = new System.Drawing.Point(117, 102);
      this.middleNameTextBox.Name = "middleNameTextBox";
      this.middleNameTextBox.Size = new System.Drawing.Size(100, 20);
      this.middleNameTextBox.TabIndex = 8;
      // 
      // placeOfBirthTextBox
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.placeOfBirthTextBox, false);
      this.placeOfBirthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "PlaceOfBirth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.placeOfBirthTextBox.Location = new System.Drawing.Point(117, 128);
      this.placeOfBirthTextBox.Name = "placeOfBirthTextBox";
      this.placeOfBirthTextBox.Size = new System.Drawing.Size(100, 20);
      this.placeOfBirthTextBox.TabIndex = 10;
      // 
      // SaveButton
      // 
      this.cslaActionExtender1.SetActionType(this.SaveButton, Csla.Windows.CslaFormAction.Save);
      this.readWriteAuthorization1.SetApplyAuthorization(this.SaveButton, false);
      this.cslaActionExtender1.SetDisableWhenUseless(this.SaveButton, true);
      this.SaveButton.Location = new System.Drawing.Point(12, 182);
      this.SaveButton.Name = "SaveButton";
      this.SaveButton.Size = new System.Drawing.Size(75, 23);
      this.SaveButton.TabIndex = 11;
      this.SaveButton.Text = "Save";
      this.SaveButton.UseVisualStyleBackColor = true;
      // 
      // CloseButton
      // 
      this.cslaActionExtender1.SetActionType(this.CloseButton, Csla.Windows.CslaFormAction.Close);
      this.readWriteAuthorization1.SetApplyAuthorization(this.CloseButton, false);
      this.CloseButton.Location = new System.Drawing.Point(12, 211);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 23);
      this.CloseButton.TabIndex = 12;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      // 
      // CancelButton
      // 
      this.cslaActionExtender1.SetActionType(this.CancelButton, Csla.Windows.CslaFormAction.Cancel);
      this.readWriteAuthorization1.SetApplyAuthorization(this.CancelButton, false);
      this.cslaActionExtender1.SetDisableWhenUseless(this.CancelButton, true);
      this.CancelButton.Location = new System.Drawing.Point(93, 182);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 23);
      this.CancelButton.TabIndex = 13;
      this.CancelButton.Text = "Cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      // 
      // ValidateButton
      // 
      this.cslaActionExtender1.SetActionType(this.ValidateButton, Csla.Windows.CslaFormAction.Validate);
      this.readWriteAuthorization1.SetApplyAuthorization(this.ValidateButton, false);
      this.ValidateButton.Location = new System.Drawing.Point(93, 211);
      this.ValidateButton.Name = "ValidateButton";
      this.ValidateButton.Size = new System.Drawing.Size(75, 23);
      this.ValidateButton.TabIndex = 14;
      this.ValidateButton.Text = "Validate";
      this.ValidateButton.UseVisualStyleBackColor = true;
      // 
      // toolStrip1
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this.toolStrip1, false);
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripButton,
            this.cancelToolStripButton,
            this.closeToolStripButton,
            this.validateToolStripButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(292, 25);
      this.toolStrip1.TabIndex = 15;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // saveToolStripButton
      // 
      this.cslaActionExtenderToolStrip1.SetActionType(this.saveToolStripButton, Csla.Windows.CslaFormAction.Save);
      this.cslaActionExtenderToolStrip1.SetDisableWhenUseless(this.saveToolStripButton, true);
      this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
      this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.saveToolStripButton.Name = "saveToolStripButton";
      this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.saveToolStripButton.Text = "&Save";
      // 
      // cancelToolStripButton
      // 
      this.cslaActionExtenderToolStrip1.SetActionType(this.cancelToolStripButton, Csla.Windows.CslaFormAction.Cancel);
      this.cslaActionExtenderToolStrip1.SetDisableWhenUseless(this.cancelToolStripButton, true);
      this.cancelToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.cancelToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelToolStripButton.Image")));
      this.cancelToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.cancelToolStripButton.Name = "cancelToolStripButton";
      this.cancelToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.cancelToolStripButton.Text = "&Print";
      // 
      // closeToolStripButton
      // 
      this.cslaActionExtenderToolStrip1.SetActionType(this.closeToolStripButton, Csla.Windows.CslaFormAction.Close);
      this.closeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.closeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripButton.Image")));
      this.closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.closeToolStripButton.Name = "closeToolStripButton";
      this.closeToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.closeToolStripButton.Text = "&New";
      // 
      // validateToolStripButton
      // 
      this.validateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.validateToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("validateToolStripButton.Image")));
      this.validateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.validateToolStripButton.Name = "validateToolStripButton";
      this.validateToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.validateToolStripButton.Text = "&Open";
      // 
      // cslaActionExtender1
      // 
      this.cslaActionExtender1.DataSource = this.editablePersonBindingSource;
      this.cslaActionExtender1.ObjectIsValidMessage = "Object is valid";
      // 
      // cslaActionExtenderToolStrip1
      // 
      this.cslaActionExtenderToolStrip1.DataSource = this.editablePersonBindingSource;
      this.cslaActionExtenderToolStrip1.ObjectIsValidMessage = "Object is valid";
      // 
      // PersonForm
      // 
      this.readWriteAuthorization1.SetApplyAuthorization(this, false);
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 273);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.ValidateButton);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.CloseButton);
      this.Controls.Add(this.SaveButton);
      this.Controls.Add(authLevelLabel);
      this.Controls.Add(this.authLevelTextBox);
      this.Controls.Add(firstNameLabel);
      this.Controls.Add(this.firstNameTextBox);
      this.Controls.Add(lastNameLabel);
      this.Controls.Add(this.lastNameTextBox);
      this.Controls.Add(middleNameLabel);
      this.Controls.Add(this.middleNameTextBox);
      this.Controls.Add(placeOfBirthLabel);
      this.Controls.Add(this.placeOfBirthTextBox);
      this.Name = "PersonForm";
      this.Text = "PersonForm";
      ((System.ComponentModel.ISupportInitialize)(this.editablePersonBindingSource)).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource editablePersonBindingSource;
        public System.Windows.Forms.TextBox authLevelTextBox;
        public System.Windows.Forms.TextBox firstNameTextBox;
        public System.Windows.Forms.TextBox lastNameTextBox;
        public System.Windows.Forms.TextBox middleNameTextBox;
        public System.Windows.Forms.TextBox placeOfBirthTextBox;
        public Csla.Windows.ReadWriteAuthorization readWriteAuthorization1;
        private Csla.Windows.CslaActionExtender cslaActionExtender1;
        public System.Windows.Forms.Button SaveButton;
        public System.Windows.Forms.Button CloseButton;
#pragma warning disable CS0108 
        public System.Windows.Forms.Button CancelButton;
#pragma warning restore CS0108 
        public System.Windows.Forms.Button ValidateButton;
        public Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
        private Csla.Windows.CslaActionExtenderToolStrip cslaActionExtenderToolStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton closeToolStripButton;
        public System.Windows.Forms.ToolStripButton validateToolStripButton;
        public System.Windows.Forms.ToolStripButton saveToolStripButton;
        public System.Windows.Forms.ToolStripButton cancelToolStripButton;
    }
}