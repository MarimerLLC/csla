using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PTWin
{
  /// <summary>
  /// Summary description for InputBox.
  /// </summary>
  public class InputBox : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public InputBox(string prompt, string title)
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      _prompt = prompt;
      _title = title;
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
      this.lblPrompt = new System.Windows.Forms.Label();
      this.txtInput = new System.Windows.Forms.TextBox();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lblPrompt
      // 
      this.lblPrompt.Location = new System.Drawing.Point(16, 16);
      this.lblPrompt.Name = "lblPrompt";
      this.lblPrompt.Size = new System.Drawing.Size(248, 72);
      this.lblPrompt.TabIndex = 0;
      this.lblPrompt.Text = "Enter text";
      // 
      // txtInput
      // 
      this.txtInput.Location = new System.Drawing.Point(16, 96);
      this.txtInput.Name = "txtInput";
      this.txtInput.Size = new System.Drawing.Size(344, 20);
      this.txtInput.TabIndex = 1;
      this.txtInput.Text = "";
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(280, 16);
      this.btnOK.Name = "btnOK";
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "OK";
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(280, 48);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // InputBox
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(376, 126);
      this.ControlBox = false;
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.btnCancel,
                                                                  this.btnOK,
                                                                  this.txtInput,
                                                                  this.lblPrompt});
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "InputBox";
      this.ShowInTaskbar = false;
      this.Text = "InputBox";
      this.Load += new System.EventHandler(this.InputBox_Load);
      this.ResumeLayout(false);

    }
		#endregion

    private string _result = string.Empty;
    private string _title = "InputBox";
    private string _prompt = "Enter text";

    private void InputBox_Load(object sender, System.EventArgs e)
    {
      this.Text = _title;
      lblPrompt.Text = _prompt;
    }

    private void btnOK_Click(object sender, System.EventArgs e)
    {
      _result = txtInput.Text;
      Close();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      _result = string.Empty;
      Close();
    }

    public string Result
    {
      get
      {
        return _result;
      }
    }

    public static string GetInput(string prompt, string title)
    {
      InputBox frm = new InputBox(prompt, title);
      frm.ShowDialog();
      return frm.Result;
    }
  }
}
