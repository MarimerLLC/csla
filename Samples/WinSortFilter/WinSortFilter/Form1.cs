using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
      var list = DataPortal.Create<DataList>();
      list.Add(DataPortal.CreateChild<Data>(213, "abc"));
      list.Add(DataPortal.CreateChild<Data>(113, "qwe"));
      list.Add(DataPortal.CreateChild<Data>(413, "zcx"));
      list.Add(DataPortal.CreateChild<Data>(233, "abc"));
      list.Add(DataPortal.CreateChild<Data>(215, "ler"));

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
