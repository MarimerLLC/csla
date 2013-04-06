namespace CslaItemTemplateWizards
{
  partial class ListItemNameDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListItemNameDialog));
      this.OKButton = new System.Windows.Forms.Button();
      this.ItemNameLabel = new System.Windows.Forms.Label();
      this.childListText = new System.Windows.Forms.TextBox();
      this.childItemText = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // OKButton
      // 
      this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OKButton.Location = new System.Drawing.Point(104, 185);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(75, 23);
      this.OKButton.TabIndex = 0;
      this.OKButton.Text = "&OK";
      this.OKButton.UseVisualStyleBackColor = true;
      // 
      // ItemNameLabel
      // 
      this.ItemNameLabel.AutoSize = true;
      this.ItemNameLabel.Location = new System.Drawing.Point(13, 25);
      this.ItemNameLabel.Name = "ItemNameLabel";
      this.ItemNameLabel.Size = new System.Drawing.Size(188, 13);
      this.ItemNameLabel.TabIndex = 1;
      this.ItemNameLabel.Text = "Please enter the Child List Class Name";
      // 
      // childListText
      // 
      this.childListText.Location = new System.Drawing.Point(13, 58);
      this.childListText.Name = "childListText";
      this.childListText.Size = new System.Drawing.Size(267, 20);
      this.childListText.TabIndex = 2;
      // 
      // childItemText
      // 
      this.childItemText.Location = new System.Drawing.Point(13, 141);
      this.childItemText.Name = "childItemText";
      this.childItemText.Size = new System.Drawing.Size(267, 20);
      this.childItemText.TabIndex = 4;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 108);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(211, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Please enter the Child List Item Class Name";
      // 
      // ListItemNameDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 231);
      this.Controls.Add(this.childItemText);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.childListText);
      this.Controls.Add(this.ItemNameLabel);
      this.Controls.Add(this.OKButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ListItemNameDialog";
      this.Text = "Item Class Name Selection";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button OKButton;
    private System.Windows.Forms.Label ItemNameLabel;
    private System.Windows.Forms.TextBox childListText;
    private System.Windows.Forms.TextBox childItemText;
    private System.Windows.Forms.Label label1;
  }
}