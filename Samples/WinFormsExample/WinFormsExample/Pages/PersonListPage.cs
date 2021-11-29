using BusinessLibrary;
using Csla;

namespace WinFormsExample.Pages
{
  public partial class PersonListPage : UserControl
  {
    public PersonListPage(IDataPortal<PersonList> portal)
    {
      _portal = portal;
      InitializeComponent();
    }

    private IDataPortal<PersonList> _portal;

    private async void PersonListPage_Load(object sender, EventArgs e)
    {
      var personList = await _portal.FetchAsync();
      personListBox.DataSource = personList;
    }

    private void personListBox_DoubleClick(object sender, EventArgs e)
    {
      var list = (ListBox)sender;
      if (list.SelectedItem!= null)
      {
        var personInfo = list.SelectedItem as PersonInfo;
        MainForm.Instance.ShowPage(typeof(PersonEditPage), personInfo);
      }
    }
  }
}
