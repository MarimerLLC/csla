namespace PTServiceClient
{
  partial class ResourceName
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
      System.Windows.Forms.Label NameLabel;
      System.Windows.Forms.Label IdLabel;
      this.Cancel_Button = new System.Windows.Forms.Button();
      this.NameLabel1 = new System.Windows.Forms.Label();
      this.OK_Button = new System.Windows.Forms.Button();
      this.IdLabel1 = new System.Windows.Forms.Label();
      this.LastNameTextBox = new System.Windows.Forms.TextBox();
      this.Label2 = new System.Windows.Forms.Label();
      this.FirstNameTextBox = new System.Windows.Forms.TextBox();
      this.Label1 = new System.Windows.Forms.Label();
      this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      NameLabel = new System.Windows.Forms.Label();
      IdLabel = new System.Windows.Forms.Label();
      this.TableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // NameLabel
      // 
      NameLabel.AutoSize = true;
      NameLabel.Location = new System.Drawing.Point(10, 38);
      NameLabel.Name = "NameLabel";
      NameLabel.Size = new System.Drawing.Size(38, 13);
      NameLabel.TabIndex = 15;
      NameLabel.Text = "Name:";
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
      this.Cancel_Button.TabIndex = 1;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // NameLabel1
      // 
      this.NameLabel1.Location = new System.Drawing.Point(97, 38);
      this.NameLabel1.Name = "NameLabel1";
      this.NameLabel1.Size = new System.Drawing.Size(236, 23);
      this.NameLabel1.TabIndex = 17;
      // 
      // OK_Button
      // 
      this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.OK_Button.Location = new System.Drawing.Point(3, 3);
      this.OK_Button.Name = "OK_Button";
      this.OK_Button.Size = new System.Drawing.Size(67, 23);
      this.OK_Button.TabIndex = 0;
      this.OK_Button.Text = "OK";
      this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
      // 
      // IdLabel
      // 
      IdLabel.AutoSize = true;
      IdLabel.Location = new System.Drawing.Point(10, 15);
      IdLabel.Name = "IdLabel";
      IdLabel.Size = new System.Drawing.Size(19, 13);
      IdLabel.TabIndex = 14;
      IdLabel.Text = "Id:";
      // 
      // IdLabel1
      // 
      this.IdLabel1.Location = new System.Drawing.Point(97, 15);
      this.IdLabel1.Name = "IdLabel1";
      this.IdLabel1.Size = new System.Drawing.Size(236, 23);
      this.IdLabel1.TabIndex = 16;
      // 
      // LastNameTextBox
      // 
      this.LastNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.LastNameTextBox.Location = new System.Drawing.Point(100, 106);
      this.LastNameTextBox.Name = "LastNameTextBox";
      this.LastNameTextBox.Size = new System.Drawing.Size(244, 20);
      this.LastNameTextBox.TabIndex = 13;
      // 
      // Label2
      // 
      this.Label2.AutoSize = true;
      this.Label2.Location = new System.Drawing.Point(10, 109);
      this.Label2.Name = "Label2";
      this.Label2.Size = new System.Drawing.Size(59, 13);
      this.Label2.TabIndex = 12;
      this.Label2.Text = "Last name:";
      // 
      // FirstNameTextBox
      // 
      this.FirstNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.FirstNameTextBox.Location = new System.Drawing.Point(100, 80);
      this.FirstNameTextBox.Name = "FirstNameTextBox";
      this.FirstNameTextBox.Size = new System.Drawing.Size(244, 20);
      this.FirstNameTextBox.TabIndex = 11;
      // 
      // Label1
      // 
      this.Label1.AutoSize = true;
      this.Label1.Location = new System.Drawing.Point(10, 83);
      this.Label1.Name = "Label1";
      this.Label1.Size = new System.Drawing.Size(58, 13);
      this.Label1.TabIndex = 10;
      this.Label1.Text = "First name:";
      // 
      // TableLayoutPanel1
      // 
      this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.TableLayoutPanel1.ColumnCount = 2;
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
      this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
      this.TableLayoutPanel1.Location = new System.Drawing.Point(198, 148);
      this.TableLayoutPanel1.Name = "TableLayoutPanel1";
      this.TableLayoutPanel1.RowCount = 1;
      this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
      this.TableLayoutPanel1.TabIndex = 9;
      // 
      // ResourceName
      // 
      this.AcceptButton = this.OK_Button;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.Cancel_Button;
      this.ClientSize = new System.Drawing.Size(355, 193);
      this.Controls.Add(NameLabel);
      this.Controls.Add(this.NameLabel1);
      this.Controls.Add(IdLabel);
      this.Controls.Add(this.IdLabel1);
      this.Controls.Add(this.LastNameTextBox);
      this.Controls.Add(this.Label2);
      this.Controls.Add(this.FirstNameTextBox);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.TableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ResourceName";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ResourceName";
      this.TableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button Cancel_Button;
    internal System.Windows.Forms.Label NameLabel1;
    internal System.Windows.Forms.Button OK_Button;
    internal System.Windows.Forms.Label IdLabel1;
    internal System.Windows.Forms.TextBox LastNameTextBox;
    internal System.Windows.Forms.Label Label2;
    internal System.Windows.Forms.TextBox FirstNameTextBox;
    internal System.Windows.Forms.Label Label1;
    internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
  }
}