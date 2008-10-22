using System.Windows.Forms;

namespace CslaItemTemplateWizards
{
  public partial class ItemNameDialog : Form
  {
    public ItemNameDialog()
    {
      InitializeComponent();
    }

    public string GetChildItemName()
    {
      return childItemText != null ? childItemText.Text : "Item";
    }
  }
}
