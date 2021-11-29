namespace WinFormsExample.Pages
{
  partial class HomePage
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
      this.personEditButton = new System.Windows.Forms.Button();
      this.personListButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // personEditButton
      // 
      this.personEditButton.Location = new System.Drawing.Point(17, 73);
      this.personEditButton.Name = "personEditButton";
      this.personEditButton.Size = new System.Drawing.Size(193, 34);
      this.personEditButton.TabIndex = 0;
      this.personEditButton.Text = "Add person";
      this.personEditButton.UseVisualStyleBackColor = true;
      this.personEditButton.Click += new System.EventHandler(this.personEditButton_Click);
      // 
      // personListButton
      // 
      this.personListButton.Location = new System.Drawing.Point(17, 23);
      this.personListButton.Name = "personListButton";
      this.personListButton.Size = new System.Drawing.Size(193, 34);
      this.personListButton.TabIndex = 1;
      this.personListButton.Text = "Person list";
      this.personListButton.UseVisualStyleBackColor = true;
      this.personListButton.Click += new System.EventHandler(this.personListButton_Click);
      // 
      // HomePage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.personListButton);
      this.Controls.Add(this.personEditButton);
      this.Name = "HomePage";
      this.Size = new System.Drawing.Size(248, 157);
      this.ResumeLayout(false);

    }

    #endregion

    private Button personEditButton;
    private Button personListButton;
  }
}
