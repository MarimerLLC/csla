namespace WindowsUI
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.orderBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.lineItemsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.cslaActionExtender1 = new Csla.Windows.CslaActionExtender(this.components);
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.bindingSourceRefresh1 = new Csla.Windows.BindingSourceRefresh(this.components);
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.button3 = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).BeginInit();
      this.SuspendLayout();
      // 
      // orderBindingSource
      // 
      this.orderBindingSource.DataSource = typeof(BusinessLibrary.Order);
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.orderBindingSource, true);
      // 
      // lineItemsBindingSource
      // 
      this.lineItemsBindingSource.DataMember = "LineItems";
      this.lineItemsBindingSource.DataSource = this.orderBindingSource;
      this.bindingSourceRefresh1.SetReadValuesOnChange(this.lineItemsBindingSource, false);
      // 
      // cslaActionExtender1
      // 
      this.cslaActionExtender1.DataSource = this.orderBindingSource;
      // 
      // button1
      // 
      this.cslaActionExtender1.SetActionType(this.button1, Csla.Windows.CslaFormAction.Cancel);
      this.button1.Location = new System.Drawing.Point(13, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(219, 23);
      this.button1.TabIndex = 8;
      this.button1.Text = "Test non-serializable exception serverside";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.cslaActionExtender1.SetActionType(this.button2, Csla.Windows.CslaFormAction.Cancel);
      this.button2.Location = new System.Drawing.Point(264, 12);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(219, 23);
      this.button2.TabIndex = 10;
      this.button2.Text = "Test non-existing exception on client";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      this.errorProvider1.DataSource = this.orderBindingSource;
      // 
      // bindingSourceRefresh1
      // 
      this.bindingSourceRefresh1.Host = this;
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(13, 60);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(751, 292);
      this.textBox1.TabIndex = 9;
      this.textBox1.Text = resources.GetString("textBox1.Text");
      // 
      // button3
      // 
      this.cslaActionExtender1.SetActionType(this.button3, Csla.Windows.CslaFormAction.Cancel);
      this.button3.Location = new System.Drawing.Point(515, 12);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(219, 23);
      this.button3.TabIndex = 11;
      this.button3.Text = "Test NotImplementedException";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(776, 364);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.button1);
      this.Name = "Form1";
      this.Text = "Custom Exception handling";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lineItemsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSourceRefresh1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource orderBindingSource;
    private System.Windows.Forms.BindingSource lineItemsBindingSource;
    private Csla.Windows.CslaActionExtender cslaActionExtender1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private Csla.Windows.BindingSourceRefresh bindingSourceRefresh1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button3;
  }
}

