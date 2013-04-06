using System.Windows.Forms;

namespace CslaItemTemplateWizards
{
  public partial class ListItemNameDialog : Form
  {
    public ListItemNameDialog()
    {
      InitializeComponent();
    }

    public string GetChilListdName()
    {
      return childListText != null ? childListText.Text : "ChildList";
    }

    public string GetChildItemName()
    {
      return childItemText != null ? childItemText.Text : "ChildItem";
    }
  }
}
