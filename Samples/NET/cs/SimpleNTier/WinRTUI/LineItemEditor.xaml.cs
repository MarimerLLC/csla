using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WinRTUI
{
  public sealed partial class LineItemEditor : UserControl
  {
    public LineItemEditor(object sender, BusinessLibrary.LineItem item)
    {
      this.InitializeComponent();

      Container = new Popup();
      Item = item;
      var rect = GetElementRect((FrameworkElement)sender);

      Container.Child = this;

      this.Height = rect.Height;
      this.Width = rect.Width;
      Container.Height = this.Height;
      Container.Width = this.Width;
      Container.IsLightDismissEnabled = false;

      Container.ChildTransitions = new TransitionCollection();
      Container.ChildTransitions.Add(new PopupThemeTransition());

      Container.SetValue(Canvas.LeftProperty, rect.Left);
      Container.SetValue(Canvas.TopProperty, rect.Top);
    }

    public void Show()
    {
      Container.Closed += Container_Closed;
      Container.IsOpen = true;
    }

    public event EventHandler Closed;

    void Container_Closed(object sender, object e)
    {
      if (SaveClicked)
        Item.ApplyEdit();
      else
        Item.CancelEdit();
      if (Closed != null)
        Closed(this, EventArgs.Empty);
    }

    private Popup Container { get; set; }
    public bool SaveClicked { get; private set; }

    private BusinessLibrary.LineItem _item;
    private BusinessLibrary.LineItem Item
    {
      get { return _item; }
      set
      {
        _item = value;
        Item.BeginEdit();
        this.DataContext = Item;
      }
    }

    private void SaveItem(object sender, RoutedEventArgs e)
    {
      SaveClicked = true;
      Container.IsOpen = false;
    }

    private void CancelItem(object sender, RoutedEventArgs e)
    {
      Container.IsOpen = false;
    }

    public static Rect GetElementRect(FrameworkElement element)
    {
      GeneralTransform buttonTransform = element.TransformToVisual(null);
      Point point = buttonTransform.TransformPoint(new Point());
      return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
    }
  }
}
