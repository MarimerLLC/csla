using System;
using System.Windows.Forms;
using Csla;

namespace WinSortFilter
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      var listPortal = Program.ApplicationContext.GetRequiredService<IDataPortal<DataList>>();
      var itemPortal = Program.ApplicationContext.GetRequiredService<IChildDataPortal<Data>>();
      var list = listPortal.Create();
      list.Add(itemPortal.CreateChild(213, "abc"));
      list.Add(itemPortal.CreateChild(113, "qwe"));
      list.Add(itemPortal.CreateChild(413, "zcx"));
      list.Add(itemPortal.CreateChild(233, "abc"));
      list.Add(itemPortal.CreateChild(215, "ler"));

      this.dataListBindingSource.DataSource = list;

      this.sortedBindingSource.DataSource = new Csla.SortedBindingList<Data>(list);

      var filtered = new Csla.FilteredBindingList<Data>(list);
      this.filteredBindingSource.DataSource = filtered;
      filtered.ApplyFilter("Name", "abc");
    }

    private void dataListBindingNavigator_RefreshItems(object sender, EventArgs e)
    {

    }
  }
}
