using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Composite.Presentation.Commands;
using DataGrid = Microsoft.Windows.Controls.DataGrid;
using DataGridColumn = Microsoft.Windows.Controls.DataGridColumn;
using DataGridLength = Microsoft.Windows.Controls.DataGridLength;

namespace Rolodex.Silverlight.Core
{
  public class CoreDataGrid : DataGrid
  {
    private const int ScrollBarWidth = 22;

    public CoreDataGrid()
    {
      SizeChanged += CoreDataGrid_SizeChanged;
    }

    public DelegateCommand<object> SelectCommand
    {
      get { return (DelegateCommand<object>) GetValue(SelectCommandProperty); }
      set { SetValue(SelectCommandProperty, value); }
    }

    public static readonly DependencyProperty SelectCommandProperty =
      DependencyProperty.Register("SelectCommand", typeof(DelegateCommand<object>), typeof(CoreDataGrid),
        new PropertyMetadata(null));

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);
      DelegateCommand<object> command = GetValue(CoreDataGrid.SelectCommandProperty) as DelegateCommand<object>;
      if (command != null)
      {
        command.Execute(SelectedItem);
      }
    }

    void CoreDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      DataGrid myDataGrid = (DataGrid) sender;
      // Do not change column size if Visibility State Changed        
      if (myDataGrid.RenderSize.Width != 0)
      {
        double all_columns_sizes = 0.0;
        foreach (DataGridColumn column in myDataGrid.Columns)
        {
          all_columns_sizes += column.ActualWidth;
        }

        // Space available to fill ( -18 Standard vScrollbar)           
        double space_available = (myDataGrid.RenderSize.Width - ScrollBarWidth) - all_columns_sizes;
        foreach (DataGridColumn column in myDataGrid.Columns)
        {
          column.Width = new DataGridLength(column.ActualWidth + (space_available / myDataGrid.Columns.Count));
        }
      }
    }
  }
}