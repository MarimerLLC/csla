//-----------------------------------------------------------------------
// <copyright file="OrderMaint2.Designer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------

namespace ActionExtenderSample
{
  partial class OrderMaint2
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
      System.Windows.Forms.Label cardHolderLabel;
      System.Windows.Forms.Label cardTypeLabel;
      System.Windows.Forms.Label creditCardLabel;
      System.Windows.Forms.Label expDateLabel;
      System.Windows.Forms.Label orderIDLabel;
      System.Windows.Forms.Label orderNumberLabel;
      System.Windows.Forms.Label userNameLabel;
      System.Windows.Forms.Label textLabel;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderMaint2));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.cardHolderTextBox = new System.Windows.Forms.TextBox();
      this.orderBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.cardTypeTextBox = new System.Windows.Forms.TextBox();
      this.creditCardTextBox = new System.Windows.Forms.TextBox();
      this.expDateTextBox = new System.Windows.Forms.TextBox();
      this.orderIDLabel1 = new System.Windows.Forms.Label();
      this.orderNumberTextBox = new System.Windows.Forms.TextBox();
      this.userNameTextBox = new System.Windows.Forms.TextBox();
      this.orderDateTextBox = new System.Windows.Forms.TextBox();
      this.orderDetailListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.orderDetailListDataGridView = new System.Windows.Forms.DataGridView();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.toolSave = new System.Windows.Forms.ToolStripButton();
      this.toolSaveNew = new System.Windows.Forms.ToolStripButton();
      this.toolSaveClose = new System.Windows.Forms.ToolStripButton();
      this.toolCancel = new System.Windows.Forms.ToolStripButton();
      this.toolClose = new System.Windows.Forms.ToolStripButton();
      this.toolValidate = new System.Windows.Forms.ToolStripButton();
      this.toolForceSave = new System.Windows.Forms.ToolStripButton();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.cslaActionExtender1 = new Csla.Windows.CslaActionExtenderToolStrip(this.components);
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      cardHolderLabel = new System.Windows.Forms.Label();
      cardTypeLabel = new System.Windows.Forms.Label();
      creditCardLabel = new System.Windows.Forms.Label();
      expDateLabel = new System.Windows.Forms.Label();
      orderIDLabel = new System.Windows.Forms.Label();
      orderNumberLabel = new System.Windows.Forms.Label();
      userNameLabel = new System.Windows.Forms.Label();
      textLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.orderDetailListBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.orderDetailListDataGridView)).BeginInit();
      this.toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // cardHolderLabel
      // 
      cardHolderLabel.AutoSize = true;
      cardHolderLabel.Location = new System.Drawing.Point(36, 59);
      cardHolderLabel.Name = "cardHolderLabel";
      cardHolderLabel.Size = new System.Drawing.Size(66, 13);
      cardHolderLabel.TabIndex = 1;
      cardHolderLabel.Text = "Card Holder:";
      // 
      // cardTypeLabel
      // 
      cardTypeLabel.AutoSize = true;
      cardTypeLabel.Location = new System.Drawing.Point(36, 85);
      cardTypeLabel.Name = "cardTypeLabel";
      cardTypeLabel.Size = new System.Drawing.Size(59, 13);
      cardTypeLabel.TabIndex = 3;
      cardTypeLabel.Text = "Card Type:";
      // 
      // creditCardLabel
      // 
      creditCardLabel.AutoSize = true;
      creditCardLabel.Location = new System.Drawing.Point(36, 111);
      creditCardLabel.Name = "creditCardLabel";
      creditCardLabel.Size = new System.Drawing.Size(62, 13);
      creditCardLabel.TabIndex = 5;
      creditCardLabel.Text = "Credit Card:";
      // 
      // expDateLabel
      // 
      expDateLabel.AutoSize = true;
      expDateLabel.Location = new System.Drawing.Point(36, 137);
      expDateLabel.Name = "expDateLabel";
      expDateLabel.Size = new System.Drawing.Size(54, 13);
      expDateLabel.TabIndex = 7;
      expDateLabel.Text = "Exp Date:";
      // 
      // orderIDLabel
      // 
      orderIDLabel.AutoSize = true;
      orderIDLabel.Location = new System.Drawing.Point(36, 163);
      orderIDLabel.Name = "orderIDLabel";
      orderIDLabel.Size = new System.Drawing.Size(50, 13);
      orderIDLabel.TabIndex = 9;
      orderIDLabel.Text = "Order ID:";
      // 
      // orderNumberLabel
      // 
      orderNumberLabel.AutoSize = true;
      orderNumberLabel.Location = new System.Drawing.Point(36, 189);
      orderNumberLabel.Name = "orderNumberLabel";
      orderNumberLabel.Size = new System.Drawing.Size(76, 13);
      orderNumberLabel.TabIndex = 11;
      orderNumberLabel.Text = "Order Number:";
      // 
      // userNameLabel
      // 
      userNameLabel.AutoSize = true;
      userNameLabel.Location = new System.Drawing.Point(36, 215);
      userNameLabel.Name = "userNameLabel";
      userNameLabel.Size = new System.Drawing.Size(63, 13);
      userNameLabel.TabIndex = 13;
      userNameLabel.Text = "User Name:";
      // 
      // textLabel
      // 
      textLabel.AutoSize = true;
      textLabel.Location = new System.Drawing.Point(37, 241);
      textLabel.Name = "textLabel";
      textLabel.Size = new System.Drawing.Size(62, 13);
      textLabel.TabIndex = 15;
      textLabel.Text = "Order Date:";
      // 
      // cardHolderTextBox
      // 
      this.cardHolderTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "CardHolder", true));
      this.cardHolderTextBox.Location = new System.Drawing.Point(118, 56);
      this.cardHolderTextBox.Name = "cardHolderTextBox";
      this.cardHolderTextBox.Size = new System.Drawing.Size(292, 20);
      this.cardHolderTextBox.TabIndex = 2;
      // 
      // orderBindingSource
      // 
      this.orderBindingSource.DataSource = typeof(ActionExtenderSample.Business.Order);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.orderBindingSource, true);
      // 
      // cardTypeTextBox
      // 
      this.cardTypeTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "CardType", true));
      this.cardTypeTextBox.Location = new System.Drawing.Point(118, 82);
      this.cardTypeTextBox.Name = "cardTypeTextBox";
      this.cardTypeTextBox.Size = new System.Drawing.Size(77, 20);
      this.cardTypeTextBox.TabIndex = 4;
      // 
      // creditCardTextBox
      // 
      this.creditCardTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "CreditCard", true));
      this.creditCardTextBox.Location = new System.Drawing.Point(118, 108);
      this.creditCardTextBox.MaxLength = 20;
      this.creditCardTextBox.Name = "creditCardTextBox";
      this.creditCardTextBox.Size = new System.Drawing.Size(140, 20);
      this.creditCardTextBox.TabIndex = 6;
      // 
      // expDateTextBox
      // 
      this.expDateTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "ExpDate", true));
      this.expDateTextBox.Location = new System.Drawing.Point(118, 134);
      this.expDateTextBox.Name = "expDateTextBox";
      this.expDateTextBox.Size = new System.Drawing.Size(77, 20);
      this.expDateTextBox.TabIndex = 8;
      // 
      // orderIDLabel1
      // 
      this.orderIDLabel1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "OrderID", true));
      this.orderIDLabel1.Location = new System.Drawing.Point(118, 163);
      this.orderIDLabel1.Name = "orderIDLabel1";
      this.orderIDLabel1.Size = new System.Drawing.Size(289, 13);
      this.orderIDLabel1.TabIndex = 10;
      this.orderIDLabel1.Text = "label1";
      // 
      // orderNumberTextBox
      // 
      this.orderNumberTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "OrderNumber", true));
      this.orderNumberTextBox.Location = new System.Drawing.Point(118, 186);
      this.orderNumberTextBox.MaxLength = 20;
      this.orderNumberTextBox.Name = "orderNumberTextBox";
      this.orderNumberTextBox.Size = new System.Drawing.Size(140, 20);
      this.orderNumberTextBox.TabIndex = 12;
      // 
      // userNameTextBox
      // 
      this.userNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "UserName", true));
      this.userNameTextBox.Location = new System.Drawing.Point(118, 212);
      this.userNameTextBox.MaxLength = 50;
      this.userNameTextBox.Name = "userNameTextBox";
      this.userNameTextBox.Size = new System.Drawing.Size(292, 20);
      this.userNameTextBox.TabIndex = 14;
      // 
      // orderDateTextBox
      // 
      this.orderDateTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderBindingSource, "OrderDate", true));
      this.orderDateTextBox.Location = new System.Drawing.Point(118, 238);
      this.orderDateTextBox.Name = "orderDateTextBox";
      this.orderDateTextBox.Size = new System.Drawing.Size(77, 20);
      this.orderDateTextBox.TabIndex = 16;
      this.orderDateTextBox.Validating += new System.ComponentModel.CancelEventHandler(orderDateTextBox_Validating);
      // 
      // orderDetailListBindingSource
      // 
      this.orderDetailListBindingSource.DataMember = "OrderDetailList";
      this.orderDetailListBindingSource.DataSource = this.orderBindingSource;
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.orderDetailListBindingSource, true);
      // 
      // orderDetailListDataGridView
      // 
      this.orderDetailListDataGridView.AutoGenerateColumns = false;
      this.orderDetailListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.orderDetailListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
      this.dataGridViewTextBoxColumn1,
      this.dataGridViewTextBoxColumn3,
      this.dataGridViewTextBoxColumn4,
      this.dataGridViewTextBoxColumn5});
      this.orderDetailListDataGridView.DataSource = this.orderDetailListBindingSource;
      this.orderDetailListDataGridView.Location = new System.Drawing.Point(76, 284);
      this.orderDetailListDataGridView.Name = "orderDetailListDataGridView";
      this.orderDetailListDataGridView.Size = new System.Drawing.Size(600, 220);
      this.orderDetailListDataGridView.TabIndex = 16;
      this.orderDetailListDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(orderDetailListDataGridView_DataError);
      // 
      // toolStrip1
      // 
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
      this.toolSave,
      this.toolSaveNew,
      this.toolSaveClose,
      this.toolCancel,
      this.toolClose,
      this.toolValidate,
      this.toolForceSave});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(791, 25);
      this.toolStrip1.TabIndex = 22;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // toolSave
      // 
      this.cslaActionExtender1.SetActionType(this.toolSave, Csla.Windows.CslaFormAction.Save);
      this.cslaActionExtender1.SetDisableWhenUseless(this.toolSave, true);
      this.toolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolSave.Image = ((System.Drawing.Image)(resources.GetObject("toolSave.Image")));
      this.toolSave.ImageTransparentColor = System.Drawing.Color.Black;
      this.toolSave.Name = "toolSave";
      this.toolSave.Size = new System.Drawing.Size(23, 22);
      this.toolSave.Text = "Save";
      // 
      // toolSaveNew
      // 
      this.cslaActionExtender1.SetActionType(this.toolSaveNew, Csla.Windows.CslaFormAction.Save);
      this.cslaActionExtender1.SetDisableWhenUseless(this.toolSaveNew, true);
      this.toolSaveNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolSaveNew.Name = "toolSaveNew";
      this.cslaActionExtender1.SetPostSaveAction(this.toolSaveNew, Csla.Windows.PostSaveActionType.AndNew);
      this.toolSaveNew.Size = new System.Drawing.Size(60, 22);
      this.toolSaveNew.Text = "Save/New";
      // 
      // toolSaveClose
      // 
      this.cslaActionExtender1.SetActionType(this.toolSaveClose, Csla.Windows.CslaFormAction.Save);
      this.cslaActionExtender1.SetDisableWhenUseless(this.toolSaveClose, true);
      this.toolSaveClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolSaveClose.Name = "toolSaveClose";
      this.cslaActionExtender1.SetPostSaveAction(this.toolSaveClose, Csla.Windows.PostSaveActionType.AndClose);
      this.cslaActionExtender1.SetRebindAfterSave(this.toolSaveClose, false);
      this.toolSaveClose.Size = new System.Drawing.Size(65, 22);
      this.toolSaveClose.Text = "Save/Close";
      // 
      // toolCancel
      // 
      this.cslaActionExtender1.SetActionType(this.toolCancel, Csla.Windows.CslaFormAction.Cancel);
      this.cslaActionExtender1.SetDisableWhenUseless(this.toolCancel, true);
      this.toolCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolCancel.Image")));
      this.toolCancel.ImageTransparentColor = System.Drawing.Color.Black;
      this.toolCancel.Name = "toolCancel";
      this.toolCancel.Size = new System.Drawing.Size(23, 22);
      this.toolCancel.Text = "Cancel";
      // 
      // toolClose
      // 
      this.cslaActionExtender1.SetActionType(this.toolClose, Csla.Windows.CslaFormAction.Close);
      this.toolClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolClose.Image = ((System.Drawing.Image)(resources.GetObject("toolClose.Image")));
      this.toolClose.ImageTransparentColor = System.Drawing.Color.Black;
      this.toolClose.Name = "toolClose";
      this.toolClose.Size = new System.Drawing.Size(23, 22);
      this.toolClose.Text = "Close";
      // 
      // toolValidate
      // 
      this.cslaActionExtender1.SetActionType(this.toolValidate, Csla.Windows.CslaFormAction.Validate);
      this.toolValidate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolValidate.Name = "toolValidate";
      this.toolValidate.Size = new System.Drawing.Size(49, 22);
      this.toolValidate.Text = "Validate";
      // 
      // toolForceSave
      // 
      this.cslaActionExtender1.SetActionType(this.toolForceSave, Csla.Windows.CslaFormAction.Save);
      this.toolForceSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolForceSave.Name = "toolForceSave";
      this.toolForceSave.Size = new System.Drawing.Size(65, 22);
      this.toolForceSave.Text = "Force Save";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      this.errorProvider1.DataSource = this.orderBindingSource;
      // 
      // cslaActionExtender1
      // 
      this.cslaActionExtender1.DataSource = this.orderBindingSource;
      this.cslaActionExtender1.ObjectIsValidMessage = "Order is valid";
      this.cslaActionExtender1.SetForNew += new System.EventHandler<Csla.Windows.CslaActionEventArgs>(this.cslaActionExtender1_SetForNew);
      // 
      // bindingSourceRefresh1
      // 
      this.bindingSourceRefresh1.Host = this;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "OrderDetailID";
      this.dataGridViewTextBoxColumn1.HeaderText = "Order Detail ID";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      this.dataGridViewTextBoxColumn1.Width = 185;
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.DataPropertyName = "ProductID";
      this.dataGridViewTextBoxColumn3.HeaderText = "Product ID";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.Width = 185;
      // 
      // dataGridViewTextBoxColumn4
      // 
      this.dataGridViewTextBoxColumn4.DataPropertyName = "PurchaseUnitPrice";
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridViewTextBoxColumn4.HeaderText = "Purchase Unit Price";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.Width = 130;
      // 
      // dataGridViewTextBoxColumn5
      // 
      this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.dataGridViewTextBoxColumn5.DataPropertyName = "Quantity";
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle2;
      this.dataGridViewTextBoxColumn5.HeaderText = "Quantity";
      this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
      // 
      // OrderMaint2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(791, 571);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.orderDetailListDataGridView);
      this.Controls.Add(textLabel);
      this.Controls.Add(this.orderDateTextBox);
      this.Controls.Add(cardHolderLabel);
      this.Controls.Add(this.cardHolderTextBox);
      this.Controls.Add(cardTypeLabel);
      this.Controls.Add(this.cardTypeTextBox);
      this.Controls.Add(creditCardLabel);
      this.Controls.Add(this.creditCardTextBox);
      this.Controls.Add(expDateLabel);
      this.Controls.Add(this.expDateTextBox);
      this.Controls.Add(orderIDLabel);
      this.Controls.Add(this.orderIDLabel1);
      this.Controls.Add(orderNumberLabel);
      this.Controls.Add(this.orderNumberTextBox);
      this.Controls.Add(userNameLabel);
      this.Controls.Add(this.userNameTextBox);
      this.Name = "OrderMaint2";
      this.Text = "Order Maintenance - ToolStripButton";
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.orderDetailListBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.orderDetailListDataGridView)).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource orderBindingSource;
    private System.Windows.Forms.TextBox cardHolderTextBox;
    private System.Windows.Forms.TextBox cardTypeTextBox;
    private System.Windows.Forms.TextBox creditCardTextBox;
    private System.Windows.Forms.TextBox expDateTextBox;
    private System.Windows.Forms.Label orderIDLabel1;
    private System.Windows.Forms.TextBox orderNumberTextBox;
    private System.Windows.Forms.TextBox userNameTextBox;
    private System.Windows.Forms.TextBox orderDateTextBox;
    private System.Windows.Forms.BindingSource orderDetailListBindingSource;
    private System.Windows.Forms.DataGridView orderDetailListDataGridView;
    private Csla.Windows.CslaActionExtenderToolStrip cslaActionExtender1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton toolSave;
    private System.Windows.Forms.ToolStripButton toolSaveNew;
    private System.Windows.Forms.ToolStripButton toolSaveClose;
    private System.Windows.Forms.ToolStripButton toolCancel;
    private System.Windows.Forms.ToolStripButton toolClose;
    private System.Windows.Forms.ToolStripButton toolValidate;
    private System.Windows.Forms.ToolStripButton toolForceSave;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
  }
}