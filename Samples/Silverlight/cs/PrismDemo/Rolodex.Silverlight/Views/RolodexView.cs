using Csla.Xaml;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Rolodex.Silverlight.Converters;
using Rolodex.Silverlight.ViewModels;

namespace Rolodex.Silverlight.Views
{
    public abstract class RolodexView : UserControl, IRolodexView
    {
        public RolodexView()
        {

            TabNavigation = System.Windows.Input.KeyboardNavigationMode.Cycle;
            Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {


            if (this.DataContext is IRolodexViewModel)
            {
                var model = this.DataContext as IRolodexViewModel;
                Grid root = null;
                if (this.Content is Grid)
                {
                    root = this.Content as Grid;
                }
                else if (this.Content is Border)
                {
                    if ((this.Content as Border).Child is Grid)
                    {
                        root = (this.Content as Border).Child as Grid;
                    }
                }
                if (root != null)
                {
                    if (root.Background == null)
                    {
                        root.Background = new SolidColorBrush(Colors.LightGray);
                    }
                    Grid curtainGrid = new Grid();
                    curtainGrid.SetValue(Canvas.ZIndexProperty, 9999);
                    curtainGrid.Opacity = 0.6;
                    curtainGrid.SetValue(Grid.RowSpanProperty, root.RowDefinitions.Count + 1);
                    curtainGrid.SetValue(Grid.ColumnSpanProperty, root.ColumnDefinitions.Count + 1);

                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.EndPoint = new Point(0.5, 1);
                    brush.StartPoint = new Point(0.5, 0);
                    GradientStopCollection stops = new GradientStopCollection();

                    GradientStop stop = new GradientStop();
                    stop.Color = new Color() { R = 0x80, G = 0x74, B = 0xD4 };
                    stops.Add(stop);

                    stop = new GradientStop();
                    stop.Color = new Color() { R = 0x80, G = 0x74, B = 0xD4 };
                    stop.Offset = 1;
                    stops.Add(stop);

                    stop = new GradientStop();
                    stop.Color = new Color() { R = 0xB7, G = 0x84, B = 0xD0 };
                    stop.Offset = 0.5;
                    stops.Add(stop);
                    brush.GradientStops = stops;

                    curtainGrid.Background = brush;

                    BusyAnimation busyAnimation = new BusyAnimation();
                    busyAnimation.MinHeight = 48;
                    busyAnimation.MinWidth = 48;
                    busyAnimation.MaxHeight = 300;
                    busyAnimation.MaxWidth = 300;
                    busyAnimation.TabNavigation = System.Windows.Input.KeyboardNavigationMode.Cycle;
                    Binding binding = new Binding("IsBusy");
                    busyAnimation.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    busyAnimation.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    busyAnimation.SetBinding(BusyAnimation.IsRunningProperty, binding);
                    curtainGrid.Children.Add(busyAnimation);

                    binding = new Binding("IsBusy");
                    binding.Converter = new BooleanToVisibilityConverter();
                    curtainGrid.SetBinding(Grid.VisibilityProperty, binding);

                    root.Children.Add(curtainGrid);
                    if (model.IsBusy)
                    {
                        Dispatcher.BeginInvoke(() => busyAnimation.Focus());
                    }

                    model.PropertyChanged += (o1, e1) =>
                        {
                            if (e1.PropertyName == "IsBusy")
                            {
                                if (model.IsBusy)
                                    busyAnimation.Focus();
                                else
                                    this.Focus();
                            }
                        };

                    this.Loaded -= View_Loaded;
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                if (DataContext != null)
                {
                    if (DataContext is IRolodexViewModel)
                    {
                        return (DataContext as IRolodexViewModel).IsDirty;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
