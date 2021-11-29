namespace WinFormsExample.Pages
{
  public partial class HomePage : UserControl
  {
    public HomePage()
    {
      InitializeComponent();
    }

    private void personListButton_Click(object sender, EventArgs e)
    {
      MainForm.Instance.ShowPage(typeof(PersonListPage));
    }

    private void personEditButton_Click(object sender, EventArgs e)
    {
      MainForm.Instance.ShowPage(typeof(PersonEditPage));
    }
  }
}
