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

namespace Csla.Wpf
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

    #region Dependency properties

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
        (o, e) => 
          (
            (BusyAnimation)o).SetState((bool)e.NewValue))
          );

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

    #endregion

    #region Member fields and properties

    private DispatcherTimer _timer;
    private Canvas _root;
    private Storyboard _normalStoryboard;
    private Storyboard[] _isRunningStoryboard;

    /// <summary>
    /// Gets or sets the state duration for the animation.
    /// </summary>
    public TimeSpan StateDuration
    {
      get { return (TimeSpan)GetValue(StateDurationProperty); }
      set
      {
        SetValue(StateDurationProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the busy
    /// animation is running.
    /// </summary>
    public bool IsRunning
    {
      get { return (bool)GetValue(IsRunningProperty); }
      set 
      {
        SetValue(IsRunningProperty, value);
        SetState(value);
      }
    }

    private void SetState(bool isRunning)
    {      
        if(_isRunningStoryboard!=null)
        {
          if (isRunning)
          {
            StopTimer();
            StartTimer();
          }
          else
          {
            StopTimer();
            if (_root == null)
              _root = (Canvas)Template.FindName("root", this);
            if (_root != null)
              _normalStoryboard.Begin(_root);
          }
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
      if (_root != null)
      {
        _isRunningStoryboard[_frame].Begin(_root);
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
        BuildStoryboard();
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
      double width = ActualWidth;
      double height = ActualHeight;
      double scale = Math.Min(ActualWidth, ActualHeight);
      double theta = (2.0 * Math.PI) / NUM_STATES;

      if (_root == null)
        _root = (Canvas)Template.FindName("root", this);

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

    private void BuildStoryboard()
    {
      _root = (Canvas)Template.FindName("root", this);
      _isRunningStoryboard = new Storyboard[8];
      _isRunningStoryboard[0] = (Storyboard)Template.Resources["state1"];
      _isRunningStoryboard[1] = (Storyboard)Template.Resources["state2"];
      _isRunningStoryboard[2] = (Storyboard)Template.Resources["state3"];
      _isRunningStoryboard[3] = (Storyboard)Template.Resources["state4"];
      _isRunningStoryboard[4] = (Storyboard)Template.Resources["state5"];
      _isRunningStoryboard[5] = (Storyboard)Template.Resources["state6"];
      _isRunningStoryboard[6] = (Storyboard)Template.Resources["state7"];
      _isRunningStoryboard[7] = (Storyboard)Template.Resources["state8"];
      _normalStoryboard = (Storyboard)Template.Resources["normal"];

      //_isRunningStoryboard = new Storyboard();
      //for (int n = 0; n < NUM_STATES; n++)
      //{
      //  string name = "part" + (n + 1).ToString();
      //  DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
      //  anim.Duration = new Duration(TimeSpan.FromMilliseconds(NUM_STATES * FRAME_DURATION));
      //  anim.RepeatBehavior = RepeatBehavior.Forever;

      //  Storyboard.SetTargetName(anim, name);
      //  Storyboard.SetTargetProperty(anim, new PropertyPath(Control.OpacityProperty));

      //  double tailSize = (NUM_STATES / 2);
      //  for (double i = 1; i <= NUM_STATES; i++)
      //  {
      //    double index = (n + i) % (NUM_STATES);
      //    double val = Math.Max(tailSize - i, 0.0) / tailSize;
      //    TimeSpan time = TimeSpan.FromMilliseconds(index * FRAME_DURATION);

      //    KeyTime keyTime = KeyTime.FromTimeSpan(time);
      //    LinearDoubleKeyFrame key = new LinearDoubleKeyFrame(val, keyTime);
      //    anim.KeyFrames.Add(key);
      //  }

      //  _isRunningStoryboard.Children.Add(anim);
      //}

    }

    #endregion
  }
}
