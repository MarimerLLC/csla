#if !NETFX_CORE && !XAMARIN
//-----------------------------------------------------------------------
// <copyright file="ErrorDialog.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Displays an error dialog for any exceptions</summary>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;

namespace Csla.Xaml
{
  /// <summary>
  /// Displays an error dialog for any exceptions
  /// that occur in a CslaDataProvider.
  /// </summary>
  public class ErrorDialog : Control, IErrorDialog
  {
    /// <summary>
    /// Creates a new instance of the control.
    /// </summary>
    public ErrorDialog()
    {
      this.DialogIcon = MessageBoxImage.Exclamation;
      this.DataContextChanged += ErrorDialog_DataContextChanged;
    }

    void ErrorDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      DetachSource(e.OldValue);
      AttachSource(e.NewValue);
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

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the dialog should include exception details
    /// or just the exception summary message.
    /// </summary>
    public static readonly DependencyProperty DialogIconProperty = DependencyProperty.Register(
      "DialogIcon",
      typeof(MessageBoxImage),
      typeof(ErrorDialog),
      null);

    /// <summary>
    /// Gets or sets the icon displayed in
    /// the dialog.
    /// </summary>
    public MessageBoxImage DialogIcon
    {
      get { return (MessageBoxImage)GetValue(DialogIconProperty); }
      set { SetValue(DialogIconProperty, value); }
    }

    internal void Register(object source)
    {
      AttachSource(source);
    }

    private void AttachSource(object source)
    {
      var dp = source as System.Windows.Data.DataSourceProvider;
      if (dp != null)
        dp.DataChanged += source_DataChanged;
    }

    private void DetachSource(object source)
    {
      var dp = source as System.Windows.Data.DataSourceProvider;
      if (dp != null)
        dp.DataChanged -= source_DataChanged;
    }

    private void source_DataChanged(object sender, EventArgs e)
    {
      var dp = sender as System.Windows.Data.DataSourceProvider;
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
          MessageBoxButton.OK, 
          this.DialogIcon);
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
#endif