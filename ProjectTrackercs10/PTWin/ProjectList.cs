using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
  /// <summary>
  /// Summary description for ProjectList.
  /// </summary>
  public class ProjectList : System.Windows.Forms.Form
  {
    private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
    private PTWin.ProjectReport projectReport1;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ProjectList()
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
      this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
      this.projectReport1 = new PTWin.ProjectReport();
      this.SuspendLayout();
      // 
      // crystalReportViewer1
      // 
      this.crystalReportViewer1.ActiveViewIndex = -1;
      this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.crystalReportViewer1.Name = "crystalReportViewer1";
      this.crystalReportViewer1.ReportSource = null;
      this.crystalReportViewer1.Size = new System.Drawing.Size(456, 302);
      this.crystalReportViewer1.TabIndex = 0;
      // 
      // projectReport1
      // 
      this.projectReport1.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;
      this.projectReport1.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
      this.projectReport1.PrintOptions.PaperSource = CrystalDecisions.Shared.PaperSource.Upper;
      this.projectReport1.PrintOptions.PrinterDuplex = CrystalDecisions.Shared.PrinterDuplex.Default;
      // 
      // ProjectList
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(456, 302);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.crystalReportViewer1});
      this.Name = "ProjectList";
      this.Text = "ProjectList";
      this.Load += new System.EventHandler(this.ProjectList_Load);
      this.ResumeLayout(false);

    }
		#endregion

    Project _project;

    private void ProjectList_Load(object sender, System.EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      MainForm.Status("Generating report...");
      try
      {
        CSLA.Data.ObjectAdapter da = 
          new CSLA.Data.ObjectAdapter();
        Projects ds = new Projects();
        da.Fill(ds, _project);
        da.Fill(ds, _project.Resources);

        projectReport1.SetDataSource(ds);

        //projectReport1.ExportToDisk(CrystalDecisions.[Shared].ExportFormatType.CrystalReport, "c:\x.rpt");
        //projectReport1.Close();
        //CrystalReportViewer1.ReportSource = "c:\x.rpt";

        crystalReportViewer1.ReportSource = projectReport1;
      }
      finally
      {
        MainForm.Status(string.Empty);
        Cursor.Current = Cursors.Default;
      }
    }

    public Project Project
    {
      get
      {
        return _project;
      }
      set
      {
        _project = value;
      }
    }

  }
}
