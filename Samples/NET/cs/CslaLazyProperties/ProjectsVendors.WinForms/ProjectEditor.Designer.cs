namespace ProjectsVendors.WinForms
{
    partial class ProjectEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label projectIdLabel;
            System.Windows.Forms.Label deliveryDateLabel;
            System.Windows.Forms.Label projectNameLabel;
            System.Windows.Forms.Label startDateLabel;
            this.ProjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.projectTabPage = new System.Windows.Forms.TabPage();
            this.projectIdTextBox = new System.Windows.Forms.TextBox();
            this.projectNameTextBox = new System.Windows.Forms.TextBox();
            this.deliveryDateTextBox = new System.Windows.Forms.TextBox();
            this.startDateTextBox = new System.Windows.Forms.TextBox();
            this.vendorsTabPage = new System.Windows.Forms.TabPage();
            this.vendorsDataGridView = new System.Windows.Forms.DataGridView();
            this.VendorsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.isLazyloadedLabel = new System.Windows.Forms.Label();
            this.isLazyloaded = new System.Windows.Forms.Label();
            this.bindingSourceRefresh = new Csla.Windows.BindingSourceRefresh(this.components);
            this.vendorIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorContactDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorPhoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vendorEmailDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isPrimaryVendorDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lastUpdatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            projectIdLabel = new System.Windows.Forms.Label();
            deliveryDateLabel = new System.Windows.Forms.Label();
            projectNameLabel = new System.Windows.Forms.Label();
            startDateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).BeginInit();
            this.tabControl.SuspendLayout();
            this.projectTabPage.SuspendLayout();
            this.vendorsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vendorsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VendorsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh)).BeginInit();
            this.SuspendLayout();
            // 
            // projectIdLabel
            // 
            projectIdLabel.AutoSize = true;
            projectIdLabel.Location = new System.Drawing.Point(27, 21);
            projectIdLabel.Name = "projectIdLabel";
            projectIdLabel.Size = new System.Drawing.Size(55, 13);
            projectIdLabel.TabIndex = 1;
            projectIdLabel.Text = "Project Id:";
            // 
            // deliveryDateLabel
            // 
            deliveryDateLabel.AutoSize = true;
            deliveryDateLabel.Location = new System.Drawing.Point(235, 69);
            deliveryDateLabel.Name = "deliveryDateLabel";
            deliveryDateLabel.Size = new System.Drawing.Size(74, 13);
            deliveryDateLabel.TabIndex = 7;
            deliveryDateLabel.Text = "Delivery Date:";
            // 
            // projectNameLabel
            // 
            projectNameLabel.AutoSize = true;
            projectNameLabel.Location = new System.Drawing.Point(235, 21);
            projectNameLabel.Name = "projectNameLabel";
            projectNameLabel.Size = new System.Drawing.Size(74, 13);
            projectNameLabel.TabIndex = 3;
            projectNameLabel.Text = "Project Name:";
            // 
            // startDateLabel
            // 
            startDateLabel.AutoSize = true;
            startDateLabel.Location = new System.Drawing.Point(27, 69);
            startDateLabel.Name = "startDateLabel";
            startDateLabel.Size = new System.Drawing.Size(58, 13);
            startDateLabel.TabIndex = 5;
            startDateLabel.Text = "Start Date:";
            // 
            // ProjectBindingSource
            // 
            this.ProjectBindingSource.DataSource = typeof(ProjectsVendors.Business.ProjectEdit);
            this.bindingSourceRefresh.SetReadValuesOnChange(this.ProjectBindingSource, true);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.projectTabPage);
            this.tabControl.Controls.Add(this.vendorsTabPage);
            this.tabControl.Location = new System.Drawing.Point(4, 15);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(906, 274);
            this.tabControl.TabIndex = 12;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // projectTabPage
            // 
            this.projectTabPage.Controls.Add(this.projectIdTextBox);
            this.projectTabPage.Controls.Add(projectIdLabel);
            this.projectTabPage.Controls.Add(this.projectNameTextBox);
            this.projectTabPage.Controls.Add(deliveryDateLabel);
            this.projectTabPage.Controls.Add(this.deliveryDateTextBox);
            this.projectTabPage.Controls.Add(projectNameLabel);
            this.projectTabPage.Controls.Add(this.startDateTextBox);
            this.projectTabPage.Controls.Add(startDateLabel);
            this.projectTabPage.Location = new System.Drawing.Point(4, 22);
            this.projectTabPage.Name = "projectTabPage";
            this.projectTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.projectTabPage.Size = new System.Drawing.Size(898, 248);
            this.projectTabPage.TabIndex = 0;
            this.projectTabPage.Text = "Project";
            this.projectTabPage.UseVisualStyleBackColor = true;
            // 
            // projectIdTextBox
            // 
            this.projectIdTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "ProjectId", true));
            this.projectIdTextBox.Location = new System.Drawing.Point(92, 18);
            this.projectIdTextBox.Name = "projectIdTextBox";
            this.projectIdTextBox.Size = new System.Drawing.Size(80, 20);
            this.projectIdTextBox.TabIndex = 2;
            // 
            // projectNameTextBox
            // 
            this.projectNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "ProjectName", true));
            this.projectNameTextBox.Location = new System.Drawing.Point(317, 18);
            this.projectNameTextBox.Name = "projectNameTextBox";
            this.projectNameTextBox.Size = new System.Drawing.Size(120, 20);
            this.projectNameTextBox.TabIndex = 4;
            // 
            // deliveryDateTextBox
            // 
            this.deliveryDateTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "DeliveryDate", true));
            this.deliveryDateTextBox.Location = new System.Drawing.Point(317, 66);
            this.deliveryDateTextBox.Name = "deliveryDateTextBox";
            this.deliveryDateTextBox.Size = new System.Drawing.Size(100, 20);
            this.deliveryDateTextBox.TabIndex = 8;
            // 
            // startDateTextBox
            // 
            this.startDateTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "StartDate", true));
            this.startDateTextBox.Location = new System.Drawing.Point(92, 66);
            this.startDateTextBox.Name = "startDateTextBox";
            this.startDateTextBox.Size = new System.Drawing.Size(100, 20);
            this.startDateTextBox.TabIndex = 6;
            // 
            // vendorsTabPage
            // 
            this.vendorsTabPage.Controls.Add(this.vendorsDataGridView);
            this.vendorsTabPage.Location = new System.Drawing.Point(4, 22);
            this.vendorsTabPage.Name = "vendorsTabPage";
            this.vendorsTabPage.Padding = new System.Windows.Forms.Padding(10);
            this.vendorsTabPage.Size = new System.Drawing.Size(898, 248);
            this.vendorsTabPage.TabIndex = 1;
            this.vendorsTabPage.Text = "Vendors";
            this.vendorsTabPage.UseVisualStyleBackColor = true;
            // 
            // vendorsDataGridView
            // 
            this.vendorsDataGridView.AutoGenerateColumns = false;
            this.vendorsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.vendorsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.vendorIdDataGridViewTextBoxColumn,
            this.vendorNameDataGridViewTextBoxColumn,
            this.vendorContactDataGridViewTextBoxColumn,
            this.vendorPhoneDataGridViewTextBoxColumn,
            this.vendorEmailDataGridViewTextBoxColumn,
            this.isPrimaryVendorDataGridViewCheckBoxColumn,
            this.lastUpdatedDataGridViewTextBoxColumn});
            this.vendorsDataGridView.DataSource = this.VendorsBindingSource;
            this.vendorsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vendorsDataGridView.Location = new System.Drawing.Point(10, 10);
            this.vendorsDataGridView.Name = "vendorsDataGridView";
            this.vendorsDataGridView.Size = new System.Drawing.Size(878, 228);
            this.vendorsDataGridView.TabIndex = 1;
            this.vendorsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.vendorsDataGridView_DataError);
            // 
            // VendorsBindingSource
            // 
            this.VendorsBindingSource.DataMember = "Vendors";
            this.VendorsBindingSource.DataSource = this.ProjectBindingSource;
            this.bindingSourceRefresh.SetReadValuesOnChange(this.VendorsBindingSource, false);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(679, 302);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(803, 302);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // isLazyloadedLabel
            // 
            this.isLazyloadedLabel.AutoSize = true;
            this.isLazyloadedLabel.Location = new System.Drawing.Point(8, 311);
            this.isLazyloadedLabel.Name = "isLazyloadedLabel";
            this.isLazyloadedLabel.Size = new System.Drawing.Size(83, 13);
            this.isLazyloadedLabel.TabIndex = 13;
            this.isLazyloadedLabel.Text = "Lazyload status:";
            // 
            // isLazyloaded
            // 
            this.isLazyloaded.AutoSize = true;
            this.isLazyloaded.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ProjectBindingSource, "IsLazyloaded", true));
            this.isLazyloaded.Location = new System.Drawing.Point(100, 311);
            this.isLazyloaded.Name = "isLazyloaded";
            this.isLazyloaded.Size = new System.Drawing.Size(0, 13);
            this.isLazyloaded.TabIndex = 14;
            // 
            // bindingSourceRefresh
            // 
            this.bindingSourceRefresh.Host = this;
            // 
            // vendorIdDataGridViewTextBoxColumn
            // 
            this.vendorIdDataGridViewTextBoxColumn.DataPropertyName = "VendorId";
            this.vendorIdDataGridViewTextBoxColumn.HeaderText = "VendorId";
            this.vendorIdDataGridViewTextBoxColumn.Name = "vendorIdDataGridViewTextBoxColumn";
            this.vendorIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // vendorNameDataGridViewTextBoxColumn
            // 
            this.vendorNameDataGridViewTextBoxColumn.DataPropertyName = "VendorName";
            this.vendorNameDataGridViewTextBoxColumn.HeaderText = "VendorName";
            this.vendorNameDataGridViewTextBoxColumn.Name = "vendorNameDataGridViewTextBoxColumn";
            // 
            // vendorContactDataGridViewTextBoxColumn
            // 
            this.vendorContactDataGridViewTextBoxColumn.DataPropertyName = "VendorContact";
            this.vendorContactDataGridViewTextBoxColumn.HeaderText = "VendorContact";
            this.vendorContactDataGridViewTextBoxColumn.Name = "vendorContactDataGridViewTextBoxColumn";
            // 
            // vendorPhoneDataGridViewTextBoxColumn
            // 
            this.vendorPhoneDataGridViewTextBoxColumn.DataPropertyName = "VendorPhone";
            this.vendorPhoneDataGridViewTextBoxColumn.HeaderText = "VendorPhone";
            this.vendorPhoneDataGridViewTextBoxColumn.Name = "vendorPhoneDataGridViewTextBoxColumn";
            // 
            // vendorEmailDataGridViewTextBoxColumn
            // 
            this.vendorEmailDataGridViewTextBoxColumn.DataPropertyName = "VendorEmail";
            this.vendorEmailDataGridViewTextBoxColumn.HeaderText = "VendorEmail";
            this.vendorEmailDataGridViewTextBoxColumn.Name = "vendorEmailDataGridViewTextBoxColumn";
            // 
            // isPrimaryVendorDataGridViewCheckBoxColumn
            // 
            this.isPrimaryVendorDataGridViewCheckBoxColumn.DataPropertyName = "IsPrimaryVendor";
            this.isPrimaryVendorDataGridViewCheckBoxColumn.HeaderText = "IsPrimaryVendor";
            this.isPrimaryVendorDataGridViewCheckBoxColumn.Name = "isPrimaryVendorDataGridViewCheckBoxColumn";
            // 
            // lastUpdatedDataGridViewTextBoxColumn
            // 
            this.lastUpdatedDataGridViewTextBoxColumn.DataPropertyName = "LastUpdated";
            this.lastUpdatedDataGridViewTextBoxColumn.HeaderText = "LastUpdated";
            this.lastUpdatedDataGridViewTextBoxColumn.Name = "lastUpdatedDataGridViewTextBoxColumn";
            this.lastUpdatedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ProjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.isLazyloaded);
            this.Controls.Add(this.isLazyloadedLabel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "ProjectEditor";
            this.Size = new System.Drawing.Size(910, 340);
            this.Load += new System.EventHandler(this.ProjectEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ProjectBindingSource)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.projectTabPage.ResumeLayout(false);
            this.projectTabPage.PerformLayout();
            this.vendorsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vendorsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VendorsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Csla.Windows.BindingSourceRefresh bindingSourceRefresh;
        private System.Windows.Forms.BindingSource ProjectBindingSource;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage projectTabPage;
        private System.Windows.Forms.TabPage vendorsTabPage;
        private System.Windows.Forms.TextBox projectIdTextBox;
        private System.Windows.Forms.TextBox projectNameTextBox;
        private System.Windows.Forms.TextBox startDateTextBox;
        private System.Windows.Forms.TextBox deliveryDateTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.DataGridView vendorsDataGridView;        
        private System.Windows.Forms.Label isLazyloadedLabel;
        private System.Windows.Forms.Label isLazyloaded;
        private System.Windows.Forms.BindingSource VendorsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorContactDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorPhoneDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vendorEmailDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isPrimaryVendorDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdatedDataGridViewTextBoxColumn;
    }
}
