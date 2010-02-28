using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.ComponentModel;

namespace Csla.Xaml
{
  /// <summary>
  /// Control that displays a busy animation.
  /// </summary>
  [TemplatePart(Name = "part1", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part2", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part3", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part4", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part5", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part6", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part7", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "part8", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "root", Type=typeof(Canvas))]
  [TemplatePart(Name = "normal", Type=typeof(Storyboard))]
  [TemplatePart(Name = "state1", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state2", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state3", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state4", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state5", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state6", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state7", Type = typeof(Storyboard))]
  [TemplatePart(Name = "state8", Type = typeof(Storyboard))]
  public class BusyAnimation : Control
  {
    #region Constants

    private const int NUM_STATES = 8;

    #endregion

    #region Member fields and properties

    private DispatcherTimer _timer;
    private Canvas _root;

    /// <summary>
    /// Gets or sets the state duration for the animation.
    /// </summary>
    public static readonly DependencyProperty StateDurationProperty = DependencyProperty.Register(
      "StateDuration",
      typeof(TimeSpan),
      typeof(BusyAnimation),
      new FrameworkPropertyMetadata(
        TimeSpan.FromMilliseconds(150),
        FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the state duration for the animation.
    /// </summary>
    [Category("Common")]
    public TimeSpan StateDuration
    {
      get { return (TimeSpan)GetValue(StateDurationProperty); }
      set { SetValue(StateDurationProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the busy
    /// animation is running.
    /// </summary>
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
      "IsRunning",
      typeof(bool),
      typeof(BusyAnimation),
      new FrameworkPropertyMetadata(
        false,
        FrameworkPropertyMetadataOptions.AffectsRender,
        (o, e) => ((BusyAnimation)o).SetState((bool)e.NewValue)));

    /// <summary>
    /// Gets or sets a value indicating whether the busy
    /// animation is running.
    /// </summary>
    [Category("Common")]
    public bool IsRunning
    {
      get { return (bool)GetValue(IsRunningProperty); }
      set 
      {
        SetValue(IsRunningProperty, value);
        SetState(value);
      }
    }

    private FrameworkElement Root
    {
      get
      {
        if (_root == null)
          _root = (Canvas)Template.FindName("root", this);

        return _root;
      }
    }

    private void SetState(bool isRunning)
    {
      if (isRunning)
      {
        StartTimer();
      }
      else
      {
        StopTimer();
        if (Root != null)
          VisualStateManager.GoToState(_root, "normal", true);
      }
    }

    private void StartTimer()
    {
      StopTimer();
      _timer = new DispatcherTimer();
      _timer.Interval = StateDuration;
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

    private int _frame = 0;
    void timer_Tick(object sender, EventArgs e)
    {
      if (Root != null)
      {
        VisualStateManager.GoToState(Root, "state" + _frame, true);
        _frame = (_frame + 1) % NUM_STATES;
      }
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
        SetState(IsRunning);
      };
      SizeChanged += new SizeChangedEventHandler(BusyAnimation_SizeChanged);
      LayoutUpdated += new EventHandler(BusyAnimation_LayoutUpdated);
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
      if (Template == null)
        return;

      double width = ActualWidth;
      double height = ActualHeight;
      double scale = Math.Min(ActualWidth, ActualHeight);
      double theta = (2.0 * Math.PI) / NUM_STATES;

      for (int n = 0; n < NUM_STATES; n++)
      {
        FrameworkElement item = (FrameworkElement)Template.FindName("part" + (n + 1).ToString(), this);
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
  }
}
