using System.Windows.Forms;
using Csla;

namespace BusinessRuleDemo
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      var root = App.ApplicationContext.GetRequiredService<IDataPortal<Root>>().Create();
      statesNVLBindingSource.DataSource = App.ApplicationContext.GetRequiredService<IDataPortal<StatesNVL>>().Fetch();
      countryNVLBindingSource.DataSource = App.ApplicationContext.GetRequiredService<IDataPortal<CountryNVL>>().Fetch();
      rootBindingSource.DataSource = root;
    }

    private void rootBindingSource_CurrentItemChanged(object sender, System.EventArgs e)
    {
      readWriteAuthorization1.ResetControlAuthorization();
    }
  }
}
