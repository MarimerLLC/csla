using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using CSLA.BatchQueue.Server;

namespace BatchQueue
{
  public class BatchQueue : System.ServiceProcess.ServiceBase
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public BatchQueue()
    {
      // This call is required by the Windows.Forms Component Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitComponent call
    }

    // The main entry point for the process
    static void Main()
    {
      System.ServiceProcess.ServiceBase[] ServicesToRun;
	
      // More than one user Service may run within the same process. To add
      // another service to this process, change the following line to
      // create a second service object. For example,
      //
      //   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new BatchQueue(), new MySecondUserService()};
      //
      ServicesToRun = new System.ServiceProcess.ServiceBase[] { new BatchQueue() };

      System.ServiceProcess.ServiceBase.Run(ServicesToRun);
    }

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      this.ServiceName = "BatchQueue";
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if (components != null) 
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// Set things in motion so your service can do its work.
    /// </summary>
    protected override void OnStart(string[] args)
    {
      BatchQueueService.Start();
    }
 
    /// <summary>
    /// Stop this service.
    /// </summary>
    protected override void OnStop()
    {
      BatchQueueService.Stop();
    }
  }
}
