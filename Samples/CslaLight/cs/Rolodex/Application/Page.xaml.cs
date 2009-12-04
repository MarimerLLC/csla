using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Csla.Silverlight;

namespace Rolodex
{
  public partial class Page : UserControl
  {
    public Page()
    {
      InitializeComponent();
    }

    private void Logger_LoginSuccessfull(object sender, EventArgs e)
    {
      CompaniesList list = new CompaniesList();
      list.CompanySelected += new EventHandler<CompanySelectedEventArgs>(list_CompanySelected);
      list.ShowRanksRequested += new EventHandler(list_ShowRanksRequested);
      list.NewCompanyRequested += new EventHandler(list_NewCompanyRequested);
      ShowControl(list);
    }

    void list_NewCompanyRequested(object sender, EventArgs e)
    {
      CompanyEditor editor = new CompanyEditor();
      editor.CreateNewCompanyData();
      editor.CloseRequested += new EventHandler(editor_CloseRequested);
      editor.DataLoaded += (o1, e1) =>
      { ShowControl(editor); };
    }

    void list_ShowRanksRequested(object sender, EventArgs e)
    {
      RanksEditor editor = new RanksEditor();
      editor.CloseRequested += new EventHandler(ranksEditor_CloseRequested);
      ShowControl(editor);
    }

    void ranksEditor_CloseRequested(object sender, EventArgs e)
    {
      Logger_LoginSuccessfull(this, EventArgs.Empty);
    }

    void list_CompanySelected(object sender, CompanySelectedEventArgs e)
    {
      CompanyEditor editor = new CompanyEditor();
      editor.LoadCompanyData(e.CompanyID);
      editor.CloseRequested += new EventHandler(editor_CloseRequested);
      editor.DataLoaded += (o1, e1) =>
        { ShowControl(editor); };
    }

    void editor_CloseRequested(object sender, EventArgs e)
    {
      Logger_LoginSuccessfull(sender, e);
    }

    private void ShowControl(UserControl control)
    {
      if (this.LayoutRoot.Children.Count > 0)
        this.LayoutRoot.Children.Remove(this.LayoutRoot.Children[0]);
      this.LayoutRoot.Children.Add(control);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
    }

    void Content_Resized(object sender, EventArgs e)
    {
       double scaleY = (System.Windows.Application.Current.Host.Content.ActualHeight/ this.Height);
       double scaleX = (System.Windows.Application.Current.Host.Content.ActualWidth / this.Width);
      this.scaleTransform.ScaleX = scaleX;
      this.scaleTransform.ScaleY = scaleY;
    }
  }
}
