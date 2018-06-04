namespace PTWisej
{
  partial class ResourceSelect
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

    #region Wisej Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.ResourceListListBox = new Wisej.Web.ListBox();
      this.ResourceListBindingSource = new Wisej.Web.BindingSource(this.components);
      this.TableLayoutPanel1 = new Wisej.Web.TableLayoutPanel();
      this.OK_Button = new Wisej.Web.Button();
      this.Cancel_Button = new Wisej.Web.Button();
      ((System.ComponentModel.ISupportInitialize)(this.ResourceListBindingSource)).BeginInit();
      this.TableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // ResourceListListBox
      // 
      this.ResourceListListBox.Anchor = ((Wisej.Web.AnchorStyles)((((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom) 
            | Wisej.Web.AnchorStyles.Left) 
            | Wisej.Web.AnchorStyles.Right)));
      this.ResourceListListBox.DataSource = this.ResourceListBindingSource;
      this.ResourceListListBox.Location = new System.Drawing.Point(12, 12);
      this.ResourceListListBox.Name = "ResourceListListBox";
      this.ResourceListListBox.Size = new System.Drawing.Size(416, 264);
      this.ResourceListListBox.TabIndex = 0;
      this.ResourceListListBox.DoubleClick += new System.EventHandler(this.ResourceListListBox_DoubleClick);
      // 
      // ResourceListBindingSource
      // 
      this.ResourceListBindingSource.DataSource = typeof(ProjectTracker.Library.ResourceList);
      // 
      // TableLayoutPanel1
      // 
      this.TableLayoutPanel1.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right)));
      this.TableLayoutPanel1.ColumnCount = 2;
      this.TableLayoutPanel1.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
      this.TableLayoutPanel1.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
      this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
      this.TableLayoutPanel1.Location = new System.Drawing.Point(285, 282);
      this.TableLayoutPanel1.Name = "TableLayoutPanel1";
      this.TableLayoutPanel1.RowCount = 1;
      this.TableLayoutPanel1.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
      this.TableLayoutPanel1.TabIndex = 1;
      // 
      // OK_Button
      // 
      this.OK_Button.Anchor = Wisej.Web.AnchorStyles.None;
      this.OK_Button.DialogResult = Wisej.Web.DialogResult.OK;
      this.OK_Button.Location = new System.Drawing.Point(3, 3);
      this.OK_Button.Name = "OK_Button";
      this.OK_Button.Size = new System.Drawing.Size(67, 23);
      this.OK_Button.TabIndex = 0;
      this.OK_Button.Text = "OK";
      this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = Wisej.Web.AnchorStyles.None;
      this.Cancel_Button.DialogResult = Wisej.Web.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
      this.Cancel_Button.TabIndex = 1;
      this.Cancel_Button.Text = "Cancel";
      this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
      // 
      // ResourceSelect
      // 
      this.ClientSize = new System.Drawing.Size(443, 323);
      this.Controls.Add(this.ResourceListListBox);
      this.Controls.Add(this.TableLayoutPanel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ResourceSelect";
      this.ShowInTaskbar = false;
      this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
      this.Text = "ResourceSelect";
      this.Load += new System.EventHandler(this.ResourceSelect_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ResourceListBindingSource)).EndInit();
      this.TableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    internal Wisej.Web.ListBox ResourceListListBox;
    internal Wisej.Web.TableLayoutPanel TableLayoutPanel1;
    internal Wisej.Web.Button OK_Button;
    internal Wisej.Web.Button Cancel_Button;
    internal Wisej.Web.BindingSource ResourceListBindingSource;
  }
}