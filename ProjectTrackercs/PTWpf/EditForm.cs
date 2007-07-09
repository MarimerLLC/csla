using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PTWpf
{
  /// <summary>
  /// Base class for edit forms that integrate
  /// with the MainForm navigation code.
  /// </summary>
  public class EditForm : UserControl, IRefresh
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public EditForm()
    {
      this.Loaded += new System.Windows.RoutedEventHandler(EditForm_Loaded);
    }

    void EditForm_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      ApplyAuthorization();
    }

    void IRefresh.Refresh()
    {
      ApplyAuthorization();
    }

    /// <summary>
    /// Override this method and use to apply
    /// authorization rules as the form loads
    /// or the current user changes.
    /// </summary>
    protected virtual void ApplyAuthorization()
    {
    }

    #region Title property

    public event EventHandler TitleChanged;

    /// <summary>
    /// Gets or sets the title of this
    /// edit form.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set
      {
        SetValue(TitleProperty, value);
        if (TitleChanged != null)
          TitleChanged(this, EventArgs.Empty);
      }
    }

    // Using a DependencyProperty as the backing store for _title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(EditForm), null);

    #endregion

    protected virtual void DataChanged(object sender, EventArgs e)
    {
      Csla.Wpf.CslaDataProvider dp = sender as Csla.Wpf.CslaDataProvider;
      if (dp.Error != null)
        MessageBox.Show(dp.Error.ToString(), "Data error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    }
  }
}
