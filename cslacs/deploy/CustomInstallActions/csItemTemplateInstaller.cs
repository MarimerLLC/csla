using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;


namespace CustomInstallActions
{
  [RunInstaller(true)]
  public partial class csItemTemplateInstaller : Installer
  {
    public csItemTemplateInstaller()
    {
      InitializeComponent();
    }
    public override void Install(System.Collections.IDictionary stateSaver)
    {
      base.Install(stateSaver);

      try
      {
        var vsi =
          new Process
          {
            StartInfo =
            {
              FileName = Context.Parameters["path"] + "install\\cslacs.vsi",
              UseShellExecute = true,
              CreateNoWindow = true
            }
          };
        vsi.Start();

        vsi.WaitForExit();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString());
        throw;
      }




    }
  }
}
