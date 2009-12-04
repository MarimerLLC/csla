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
            this.editablePersonBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.authLevelTextBox = new System.Windows.Forms.TextBox();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.middleNameTextBox = new System.Windows.Forms.TextBox();
            this.placeOfBirthTextBox = new System.Windows.Forms.TextBox();
            this.readWriteAuthorization1 = new Csla.Windows.ReadWriteAuthorization(this.components);
            authLevelLabel = new System.Windows.Forms.Label();
            firstNameLabel = new System.Windows.Forms.Label();
            lastNameLabel = new System.Windows.Forms.Label();
            middleNameLabel = new System.Windows.Forms.Label();
            placeOfBirthLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.editablePersonBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // editablePersonBindingSource
            // 
            this.editablePersonBindingSource.DataSource = typeof(Csla.Test.Windows.EditablePerson);
            this.editablePersonBindingSource.CurrentItemChanged += new System.EventHandler(this.editablePersonBindingSource_CurrentItemChanged);
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
            // authLevelTextBox
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this.authLevelTextBox, false);
            this.authLevelTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "AuthLevel", true));
            this.authLevelTextBox.Location = new System.Drawing.Point(117, 24);
            this.authLevelTextBox.Name = "authLevelTextBox";
            this.authLevelTextBox.Size = new System.Drawing.Size(100, 20);
            this.authLevelTextBox.TabIndex = 2;
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
            // firstNameTextBox
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this.firstNameTextBox, false);
            this.firstNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "FirstName", true));
            this.firstNameTextBox.Location = new System.Drawing.Point(117, 50);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.firstNameTextBox.TabIndex = 4;
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
            // lastNameTextBox
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this.lastNameTextBox, false);
            this.lastNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "LastName", true));
            this.lastNameTextBox.Location = new System.Drawing.Point(117, 76);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.lastNameTextBox.TabIndex = 6;
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
            // middleNameTextBox
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this.middleNameTextBox, false);
            this.middleNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "MiddleName", true));
            this.middleNameTextBox.Location = new System.Drawing.Point(117, 102);
            this.middleNameTextBox.Name = "middleNameTextBox";
            this.middleNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.middleNameTextBox.TabIndex = 8;
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
            // placeOfBirthTextBox
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this.placeOfBirthTextBox, false);
            this.placeOfBirthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.editablePersonBindingSource, "PlaceOfBirth", true));
            this.placeOfBirthTextBox.Location = new System.Drawing.Point(117, 128);
            this.placeOfBirthTextBox.Name = "placeOfBirthTextBox";
            this.placeOfBirthTextBox.Size = new System.Drawing.Size(100, 20);
            this.placeOfBirthTextBox.TabIndex = 10;
            // 
            // PersonForm
            // 
            this.readWriteAuthorization1.SetApplyAuthorization(this, false);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
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
    }
}