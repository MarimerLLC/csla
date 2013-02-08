using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
      var list = new DataList();
      list.Add(new Data { Id = 213, Name = "abc" });
      list.Add(new Data { Id = 113, Name = "qwe" });
      list.Add(new Data { Id = 413, Name = "zcx" });
      list.Add(new Data { Id = 233, Name = "abc" });
      list.Add(new Data { Id = 215, Name = "ler" });

      this.dataListBindingSource.DataSource = list;

      this.sortedBindingSource.DataSource = new Csla.SortedBindingList<Data>(list);

      var filtered = new Csla.FilteredBindingList<Data>(list);
      this.filteredBindingSource.DataSource = filtered;
      //filtered.ApplyFilter("Name", "abc");
    }

    private void dataListBindingNavigator_RefreshItems(object sender, EventArgs e)
    {

    }
  }
}
