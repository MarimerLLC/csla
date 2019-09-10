using System.Windows.Forms;
using Csla;
using Csla.Core;

namespace BusinessRuleDemo
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      var root = DataPortal.Create<Root>();
      statesNVLBindingSource.DataSource = DataPortal.Fetch<StatesNVL>();
      countryNVLBindingSource.DataSource = DataPortal.Fetch<CountryNVL>();
      rootBindingSource.DataSource = root;
    }

    private void rootBindingSource_CurrentItemChanged(object sender, System.EventArgs e)
    {
      readWriteAuthorization1.ResetControlAuthorization();
    }
  }
}
