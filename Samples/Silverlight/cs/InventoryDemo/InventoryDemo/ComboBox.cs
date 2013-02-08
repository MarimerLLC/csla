using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace InventoryDemo
{
  public class ComboBox : System.Windows.Controls.ComboBox
  {
    public ComboBox()
    {
      this.Loaded += new RoutedEventHandler(ComboBox_Loaded);
      this.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
    }

    void ComboBox_Loaded(object sender, RoutedEventArgs e)
    {
      SetSelectionFromValue();
    }

    private object _selection;

    void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        _selection = e.AddedItems[0];
        SelectedValue = GetMemberValue(_selection);
      }
      else
      {
        _selection = null;
        SelectedValue = null;
      }
    }

    private object GetMemberValue(object item)
    {
      return item.GetType().GetProperty(SelectedValuePath).GetValue(item, null);
    }

    private void SetSelectionFromValue()
    {
      var value = SelectedValue;
      if (Items.Count > 0 && value != null)
      {
        var sel = (from item in Items
                   where GetMemberValue(item).Equals(value)
                   select item).Single();
        _selection = sel;
        SelectedItem = sel;
      }
    }

    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      SetSelectionFromValue();
    }
  }
}
