using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace BatchQueue
{
  [RunInstaller(true)]
  public class BatchQueueInstaller : System.Configuration.Install.Installer
  {

    ServiceInstaller _serviceInstaller = new ServiceInstaller();
    ServiceProcessInstaller _processInstaller = new ServiceProcessInstaller();

    void InitInstaller()
    {
      _processInstaller.Account = ServiceAccount.LocalSystem;
      _serviceInstaller.StartType = ServiceStartMode.Automatic;
      _serviceInstaller.ServiceName = "CSLABatchQueue";
      Installers.Add(_serviceInstaller);
      Installers.Add(_processInstaller);
    }

    private System.ComponentModel.Container components = null;

    public BatchQueueInstaller()
    {
      // This call is required by the Designer.
      InitializeComponent();
    }

		#region Component Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      // 
      // BatchQueueInstaller
      // 
      this.BeforeUninstall += new System.Configuration.Install.InstallEventHandler(this.BatchQueueInstaller_BeforeUninstall);
      this.BeforeRollback += new System.Configuration.Install.InstallEventHandler(this.BatchQueueInstaller_BeforeRollback);
      this.BeforeInstall += new System.Configuration.Install.InstallEventHandler(this.BatchQueueInstaller_BeforeInstall);

    }
		#endregion

    private void BatchQueueInstaller_BeforeInstall(object sender, 
      System.Configuration.Install.InstallEventArgs e)
    {
      InitInstaller();
    }

    private void BatchQueueInstaller_BeforeRollback(object sender, 
      System.Configuration.Install.InstallEventArgs e)
    {
      InitInstaller();
    }

    private void BatchQueueInstaller_BeforeUninstall(object sender, 
      System.Configuration.Install.InstallEventArgs e)
    {
      InitInstaller();
    }
  }
}
