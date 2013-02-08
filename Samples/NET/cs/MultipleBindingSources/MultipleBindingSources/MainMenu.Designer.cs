namespace MultipleBindingSources
{
  partial class MainMenu
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
      this.RootEditbutton = new System.Windows.Forms.Button();
      this.RootAndChildListButton = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.closeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // RootEditbutton
      // 
      this.RootEditbutton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RootEditbutton.Location = new System.Drawing.Point(0, 3);
      this.RootEditbutton.Name = "RootEditbutton";
      this.RootEditbutton.Size = new System.Drawing.Size(231, 31);
      this.RootEditbutton.TabIndex = 0;
      this.RootEditbutton.Text = "Root - 2 BindingSources";
      this.RootEditbutton.UseVisualStyleBackColor = true;
      this.RootEditbutton.Click += new System.EventHandler(this.RootEditbutton_Click);
      // 
      // RootAndChildListButton
      // 
      this.RootAndChildListButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RootAndChildListButton.Location = new System.Drawing.Point(0, 34);
      this.RootAndChildListButton.Name = "RootAndChildListButton";
      this.RootAndChildListButton.Size = new System.Drawing.Size(231, 31);
      this.RootAndChildListButton.TabIndex = 1;
      this.RootAndChildListButton.Text = "RootAndChildList";
      this.RootAndChildListButton.UseVisualStyleBackColor = true;
      this.RootAndChildListButton.Click += new System.EventHandler(this.RootAndChildListButton_Click);
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(0, 65);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(231, 31);
      this.button1.TabIndex = 2;
      this.button1.Text = "ActionExtender";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.Location = new System.Drawing.Point(0, 461);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(231, 31);
      this.closeButton.TabIndex = 3;
      this.closeButton.Text = "Close";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // MainMenu
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(232, 493);
      this.Controls.Add(this.closeButton);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.RootAndChildListButton);
      this.Controls.Add(this.RootEditbutton);
      this.Name = "MainMenu";
      this.Text = "MainMenu";
      this.Load += new System.EventHandler(this.MainMenu_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button RootEditbutton;
    private System.Windows.Forms.Button RootAndChildListButton;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button closeButton;
  }
}