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
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Globalization;

namespace Csla.Silverlight
{
  /// <summary>
  /// Displays a busy animation.
  /// </summary>
  [TemplatePart(Name = "part1", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part2", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part3", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part4", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part5", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part6", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part7", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part8", Type = typeof(FrameworkElement))]
  [TemplateVisualState(Name = "normal", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state1", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state2", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state3", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state4", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state5", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state6", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state7", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "state8", GroupName = "CommonStates")]
  public class BusyAnimation : Control
  {
    #region Constants

    private const int NUM_STATES = 8;

    #endregion

    #region Dependency properties

    /// <summary>
    /// StepInterval property to control speed of animation.
    /// </summary>
    public static readonly DependencyProperty StepIntervalProperty = DependencyProperty.Register(
      "StepInterval",
      typeof(int),
      typeof(BusyAnimation),
      new PropertyMetadata((o, e) => ((BusyAnimation)o).StepInterval = (int)e.NewValue));

    /// <summary>
    /// IsRunning property to control whether the 
    /// animation is running.
    /// </summary>
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
      "IsRunning",
      typeof(object),
      typeof(BusyAnimation),
      new PropertyMetadata((o, e) => ((BusyAnimation)o).SetupRunningState(e.NewValue)));

    #endregion

    #region Member fields and properties

    private DispatcherTimer _timer;
    private int _state = -1;

    /// <summary>
    /// Gets or sets a property controlling
    /// the speed of the animation.
    /// </summary>
    public int StepInterval
    {
      get { return (int)GetValue(StepIntervalProperty); }
      set
      {
        SetValue(StepIntervalProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a property controlling
    /// whether the animation is running.
    /// </summary>
    public object IsRunning
    {
      get
      {
        object val = GetValue(IsRunningProperty);
        return (val != null ?
          (bool)Convert.ChangeType(val, typeof(bool), CultureInfo.InvariantCulture) :
          false);
      }
      set
      {
        bool val = (bool)Convert.ChangeType(value, typeof(bool), CultureInfo.InvariantCulture);
        SetValue(IsRunningProperty, val);
      }
    }

    private void SetupRunningState(object isRunning)
    {
      bool val = (bool)Convert.ChangeType(isRunning, typeof(bool), CultureInfo.InvariantCulture);
      if (val)
        StartTimer();
      else
        StopTimer();

      GoToState(true);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates an instance of the control.
    /// </summary>
    public BusyAnimation()
    {
      DefaultStyleKey = typeof(BusyAnimation);
      Loaded += (o, e) =>
      {
        ArrangeParts();
        GoToState(true);
      };
      SizeChanged += new SizeChangedEventHandler(BusyAnimation_SizeChanged);
      LayoutUpdated += new EventHandler(BusyAnimation_LayoutUpdated);
    }

    #endregion

    #region Timer

    private void StartTimer()
    {
      StopTimer();
      _timer = new DispatcherTimer();
      _timer.Interval = TimeSpan.FromMilliseconds(StepInterval);
      _timer.Tick += new EventHandler(timer_Tick);
      _timer.Start();
    }

    private void StopTimer()
    {
      if (_timer != null)
      {
        _timer.Stop();
        _timer.Tick -= new EventHandler(timer_Tick);
        _timer = null;
      }
    }

    void timer_Tick(object sender, EventArgs e)
    {
      _state++;
      if (_state >= NUM_STATES)
        _state = 0;

      GoToState(true);
    }

    #endregion

    #region State

    private void GoToState(bool useTransitions)
    {
      if ((bool)IsRunning)
      {
        bool result = VisualStateManager.GoToState(this, string.Format("state{0}", _state + 1), useTransitions);
      }
      else
        VisualStateManager.GoToState(this, "normal", useTransitions);
    }

    #endregion

    #region Parts

    void BusyAnimation_LayoutUpdated(object sender, EventArgs e)
    {
      ArrangeParts();
    }

    void BusyAnimation_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ArrangeParts();
    }

    private void ArrangeParts()
    {
      double width = ActualWidth;
      double height = ActualHeight;
      double scale = Math.Min(ActualWidth, ActualHeight);
      double theta = (2.0 * Math.PI) / NUM_STATES;

      for (int n = 0; n < NUM_STATES; n++)
      {
        FrameworkElement item = (FrameworkElement)FindChild(this, "part" + (n + 1).ToString());
        if (item != null)
        {
          double itemTheta = theta * (double)n;

          double itemScale = scale / 4.0;
          double radius = (scale - itemScale) / 2.0;

          double x = (radius * Math.Cos(itemTheta)) - (itemScale / 2) + (ActualWidth / 2);
          double y = (radius * Math.Sin(itemTheta)) - (itemScale / 2) + (ActualHeight / 2);

          item.Width = itemScale;
          item.Height = itemScale;
          item.SetValue(Canvas.LeftProperty, x);
          item.SetValue(Canvas.TopProperty, y);
        }
      }
    }

    #endregion

    #region Helpers

    private DependencyObject FindChild(DependencyObject parent, string name)
    {
      DependencyObject found = null;
      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int x = 0; x < count; x++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, x);
        string childName = child.GetValue(FrameworkElement.NameProperty) as string;
        if (childName == name)
        {
          found = child;
          break;
        }
        else found = FindChild(child, name);
      }

      return found;
    }

    #endregion
  }
}
