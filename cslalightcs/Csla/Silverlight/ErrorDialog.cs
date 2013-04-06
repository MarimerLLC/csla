using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Csla.Silverlight
{
  /// <summary>
  /// Displays an error dialog for any exceptions
  /// that occur in a CslaDataProvider.
  /// </summary>
  public class ErrorDialog : Control, IErrorDialog
  {
    /// <summary>
    /// Creates an instance of the control.
    /// </summary>
    public ErrorDialog()
    {
      this.DialogTitle = "Error";
    }

    /// <summary>
    /// Gets or sets the CslaDataProvider
    /// control to which the ErrorDialog control
    /// is bound.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(ErrorDialog),
      new PropertyMetadata((o, e) => ((ErrorDialog)o).Source = e.NewValue));

    private object _source;

    /// <summary>
    /// Gets or sets the CslaDataProvider
    /// control to which the ErrorDialog control
    /// is bound.
    /// </summary>
    public object Source
    {
      get { return _source; }
      set
      {
        DetachSource(_source);
        _source = value;
        AttachSource(_source);
      }
    }

    /// <summary>
    /// Gets or sets the title of the error
    /// dialog.
    /// </summary>
    public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
      "DialogTitle",
      typeof(string),
      typeof(ErrorDialog),
      null);

    /// <summary>
    /// Gets or sets the title of the error
    /// dialog.
    /// </summary>
    public string DialogTitle
    {
      get { return (string)GetValue(DialogTitleProperty); }
      set { SetValue(DialogTitleProperty, value); }
    }

    /// <summary>
    /// Gets or sets the first line of text displayed
    /// within the error dialog (before the
    /// exception message).
    /// </summary>
    public static readonly DependencyProperty DialogFirstLineProperty = DependencyProperty.Register(
      "DialogFirstLine",
      typeof(string),
      typeof(ErrorDialog),
      null);

    /// <summary>
    /// Gets or sets the first line of text displayed
    /// within the error dialog (before the
    /// exception message).
    /// </summary>
    public string DialogFirstLine
    {
      get { return (string)GetValue(DialogFirstLineProperty); }
      set { SetValue(DialogFirstLineProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the dialog should include exception details
    /// or just the exception summary message.
    /// </summary>
    public static readonly DependencyProperty ShowExceptionDetailProperty = DependencyProperty.Register(
      "ShowExceptionDetail",
      typeof(bool),
      typeof(ErrorDialog),
      null);

    /// <summary>
    /// Gets or sets the first line of text displayed
    /// within the error dialog (before the
    /// exception message).
    /// </summary>
    public bool ShowExceptionDetail
    {
      get { return (bool)GetValue(ShowExceptionDetailProperty); }
      set { SetValue(ShowExceptionDetailProperty, value); }
    }

    private void AttachSource(object source)
    {
      var dp = source as CslaDataProvider;
      if (dp != null)
        dp.DataChanged += source_DataChanged;
    }

    private void DetachSource(object source)
    {
      var dp = source as CslaDataProvider;
      if (dp != null)
        dp.DataChanged -= source_DataChanged;
    }

    private void source_DataChanged(object sender, EventArgs e)
    {
      var dp = sender as Csla.Silverlight.CslaDataProvider;
      if (dp != null && dp.Error != null)
      {
        string error;
        if (this.ShowExceptionDetail)
          error = dp.Error.ToString();
        else
          error = dp.Error.Message;

        string output;
        if (string.IsNullOrEmpty(this.DialogFirstLine))
          output = error;
        else
          output = string.Format("{0}{1}{2}", this.DialogFirstLine, Environment.NewLine, error);

        MessageBox.Show(
          output, 
          this.DialogTitle, 
          MessageBoxButton.OK);
      }
    }

    #region IErrorDialog Members

    void IErrorDialog.Register(object source)
    {
      AttachSource(source);
    }

    #endregion
  }
}
