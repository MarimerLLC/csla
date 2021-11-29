namespace WinFormsExample.Pages
{
  partial class PersonListPage
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
      this.personListBox = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // personListBox
      // 
      this.personListBox.DisplayMember = "Name";
      this.personListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.personListBox.FormattingEnabled = true;
      this.personListBox.ItemHeight = 25;
      this.personListBox.Location = new System.Drawing.Point(0, 0);
      this.personListBox.Name = "personListBox";
      this.personListBox.Size = new System.Drawing.Size(1033, 488);
      this.personListBox.TabIndex = 0;
      this.personListBox.DoubleClick += new System.EventHandler(this.personListBox_DoubleClick);
      // 
      // PersonListPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.personListBox);
      this.Name = "PersonListPage";
      this.Size = new System.Drawing.Size(1033, 488);
      this.Load += new System.EventHandler(this.PersonListPage_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private ListBox personListBox;
  }
}
