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

		public ResourceSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

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
      // 
      // ResourceSelect
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Name = "ResourceSelect";
      this.Text = "ResourceSelect";
      this.Load += new System.EventHandler(this.ResourceSelect_Load);

    }
		#endregion

    string _Result = string.Empty;

    public string Result
    {
      get
      {
        return _Result;
      }
    }

    private void ResourceSelect_Load(object sender, System.EventArgs e)
    {
    
    }
	}
}
