using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
	/// <summary>
	/// Summary description for ResourceSelect.
	/// </summary>
  public class ResourceSelect : System.Windows.Forms.Form
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.dvDisplay = new MSDN.DataListView.DataListView();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(304, 40);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 5;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnOK
      // 
      this.btnOK.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnOK.Location = new System.Drawing.Point(304, 8);
      this.btnOK.Name = "btnOK";
      this.btnOK.TabIndex = 4;
      this.btnOK.Text = "OK";
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // dvDisplay
      // 
      this.dvDisplay.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.dvDisplay.AutoDiscover = false;
      this.dvDisplay.DataSource = null;
      this.dvDisplay.DisplayMember = "";
      this.dvDisplay.FullRowSelect = true;
      this.dvDisplay.MultiSelect = false;
      this.dvDisplay.Name = "dvDisplay";
      this.dvDisplay.Size = new System.Drawing.Size(296, 272);
      this.dvDisplay.TabIndex = 3;
      this.dvDisplay.View = System.Windows.Forms.View.Details;
      this.dvDisplay.DoubleClick += new System.EventHandler(this.btnOK_Click);
      // 
      // ResourceSelect
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(392, 266);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.btnCancel,
                                                                  this.btnOK,
                                                                  this.dvDisplay});
      this.Name = "ResourceSelect";
      this.Text = "ResourceSelect";
      this.ResumeLayout(false);

    }
		#endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private MSDN.DataListView.DataListView dvDisplay;

    string _result = string.Empty;

    public ResourceSelect(string title)
    {
      InitializeComponent();
      Text = title;
      ResourceList list = ResourceList.GetResourceList();
      dvDisplay.AutoDiscover = false;
      dvDisplay.Columns.Add("ID", 0);
      dvDisplay.Columns.Add("Name", dvDisplay.Width);
      dvDisplay.DataSource = list;
      dvDisplay.Focus();
    }

    public string Result
    {
      get
      {
        return _result;
      }
    }

    private void btnOK_Click(object sender, System.EventArgs e)
    {
      if(dvDisplay.SelectedItems.Count > 0)
        _result = dvDisplay.SelectedItems[0].Text;
      else
        _result = string.Empty;
      Close();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      _result = string.Empty;
      Close();
    }
  }
}
