using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Csla.Xaml.Xamarin
{
  public class CslaXamlXamarin : ContentPage
  {
    public CslaXamlXamarin()
    {
      var button = new Button
      {
        Text = "Click Me!",
        VerticalOptions = LayoutOptions.CenterAndExpand,
        HorizontalOptions = LayoutOptions.CenterAndExpand,
      };

      int clicked = 0;
      button.Clicked += (s, e) => button.Text = "Clicked: " + clicked++;

      Content = button;
    }
  }
}
