using System.Windows.Forms;

namespace BusinessRuleDemo
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      rootBindingSource.DataSource = Root.NewEditableRoot();
    }
  }
}
