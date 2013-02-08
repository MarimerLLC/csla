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
        protected RolodexView()
        {

            TabNavigation = System.Windows.Input.KeyboardNavigationMode.Cycle;
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {


            if (DataContext is IRolodexViewModel)
            {
                var model = DataContext as IRolodexViewModel;
                Grid root = null;
                if (Content is Grid)
                {
                    root = Content as Grid;
                }
                else if (Content is Border)
                {
                    if ((Content as Border).Child is Grid)
                    {
                        root = (Content as Border).Child as Grid;
                    }
                }
                if (root != null)
                {
                    if (root.Background == null)
                    {
                        root.Background = new SolidColorBrush(Colors.LightGray);
                    }
                    var curtainGrid = new Grid();
                    curtainGrid.SetValue(Canvas.ZIndexProperty, 9999);
                    curtainGrid.Opacity = 0.6;
                    curtainGrid.SetValue(Grid.RowSpanProperty, root.RowDefinitions.Count + 1);
                    curtainGrid.SetValue(Grid.ColumnSpanProperty, root.ColumnDefinitions.Count + 1);

                    var brush = new LinearGradientBrush {EndPoint = new Point(0.5, 1), StartPoint = new Point(0.5, 0)};
                    var stops = new GradientStopCollection();

                    var stop = new GradientStop {Color = new Color {R = 0x80, G = 0x74, B = 0xD4}};
                    stops.Add(stop);

                    stop = new GradientStop {Color = new Color {R = 0x80, G = 0x74, B = 0xD4}, Offset = 1};
                    stops.Add(stop);

                    stop = new GradientStop {Color = new Color {R = 0xB7, G = 0x84, B = 0xD0}, Offset = 0.5};
                    stops.Add(stop);
                    brush.GradientStops = stops;

                    curtainGrid.Background = brush;

                    var busyAnimation = new BusyAnimation
                                            {
                                                MinHeight = 48,
                                                MinWidth = 48,
                                                MaxHeight = 300,
                                                MaxWidth = 300,
                                                TabNavigation = System.Windows.Input.KeyboardNavigationMode.Cycle
                                            };
                    var binding = new Binding("IsBusy");
                    busyAnimation.HorizontalAlignment = HorizontalAlignment.Stretch;
                    busyAnimation.VerticalAlignment = VerticalAlignment.Stretch;
                    busyAnimation.SetBinding(BusyAnimation.IsRunningProperty, binding);
                    curtainGrid.Children.Add(busyAnimation);

                    binding = new Binding("IsBusy") {Converter = new BooleanToVisibilityConverter()};
                    curtainGrid.SetBinding(VisibilityProperty, binding);

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
                                    Focus();
                            }
                        };

                    Loaded -= ViewLoaded;
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
                    return false;
                }
                return false;
            }
        }
    }
}
