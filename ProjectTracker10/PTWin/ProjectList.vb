Public Class ProjectList
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
  Friend WithEvents projectReport1 As PTWin.ProjectReport
  Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.projectReport1 = New PTWin.ProjectReport
    Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer
    Me.SuspendLayout()
    '
    'projectReport1
    '
    Me.projectReport1.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation
    Me.projectReport1.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize
    Me.projectReport1.PrintOptions.PaperSource = CrystalDecisions.Shared.PaperSource.Upper
    Me.projectReport1.PrintOptions.PrinterDuplex = CrystalDecisions.Shared.PrinterDuplex.Default
    '
    'CrystalReportViewer1
    '
    Me.CrystalReportViewer1.ActiveViewIndex = -1
    Me.CrystalReportViewer1.DisplayGroupTree = False
    Me.CrystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
    Me.CrystalReportViewer1.EnableDrillDown = False
    Me.CrystalReportViewer1.Location = New System.Drawing.Point(0, 0)
    Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
    Me.CrystalReportViewer1.Size = New System.Drawing.Size(632, 406)
    Me.CrystalReportViewer1.TabIndex = 0
    '
    'ProjectList
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(632, 406)
    Me.Controls.Add(Me.CrystalReportViewer1)
    Me.Name = "ProjectList"
    Me.Text = "ProjectList"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mProject As Project

  Public Property Project() As Project
    Get
      Return mProject
    End Get
    Set(ByVal Value As Project)
      mProject = Value
    End Set
  End Property

  Private Sub ProjectList_Load(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    Cursor.Current = Cursors.WaitCursor
    Main.Status("Generating report...")
    Try
      Dim da As New CSLA.Data.ObjectAdapter
      Dim ds As New Projects
      da.Fill(ds, mProject)
      da.Fill(ds, mProject.Resources)

      projectReport1.SetDataSource(ds)

      'projectReport1.ExportToDisk(CrystalDecisions.[Shared].ExportFormatType.CrystalReport, "c:\x.rpt")
      'projectReport1.Close()
      'CrystalReportViewer1.ReportSource = "c:\x.rpt"

      CrystalReportViewer1.ReportSource = projectReport1

    Finally
      Main.Status("")
      Cursor.Current = Cursors.Default
    End Try



  End Sub

End Class
