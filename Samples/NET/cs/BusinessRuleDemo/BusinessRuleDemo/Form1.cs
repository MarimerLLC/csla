using System.Windows.Forms;
using Csla.Core;

namespace BusinessRuleDemo
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      var root = Root.NewEditableRoot();
      statesNVLBindingSource.DataSource = StatesNVL.GetNameValueList();
      countryNVLBindingSource.DataSource = CountryNVL.GetNameValueList();
      rootBindingSource.DataSource = root;
    }

    private void rootBindingSource_CurrentItemChanged(object sender, System.EventArgs e)
    {
      readWriteAuthorization1.ResetControlAuthorization();
    }
  }
}
