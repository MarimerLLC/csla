using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
	/// <summary>
	/// Summary description for ResourceEdit.
	/// </summary>
	public class ResourceEdit : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ResourceEdit()
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
      // ResourceEdit
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Name = "ResourceEdit";
      this.Text = "ResourceEdit";
      this.Load += new System.EventHandler(this.ResourceEdit_Load);

    }
		#endregion

    Resource _Resource;

    public Resource Resource
    {
      get
      {
        return _Resource;
      }
      set
      {
        _Resource = value;
      }
    }

    private void ResourceEdit_Load(object sender, System.EventArgs e)
    {
    
    }
	}
}
