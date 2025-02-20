//-----------------------------------------------------------------------
// <copyright file="BusyAnimation.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>User control that displays busy animation</summary>
//-----------------------------------------------------------------------

namespace Csla.Windows
{
  /// <summary>
  /// User control that displays busy animation
  /// </summary>
  [ToolboxItem(true), ToolboxBitmap(typeof(BusyAnimation), "Csla.Windows.BusyAnimation")]
  public partial class BusyAnimation : UserControl
  {
    /// <summary>
    /// new instance busy animation
    /// </summary>
    public BusyAnimation()
    {
      InitializeComponent();
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BusyProgressBar.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).Invoke(BusyProgressBar, [ControlStyles.SupportsTransparentBackColor, true]);
      if (!IsInDesignMode)
        BusyProgressBar.BackColor = _progressBarBackColor;
    }

    private Color _progressBarForeColor = Color.LawnGreen;
    /// <summary>
    /// Set or get foreground color for busy animation's progress bar
    /// </summary>
    [Category("Csla")]
    [Description("Foreground color for busy animation's progress bar.")]
    [DefaultValue(typeof(Color), "LawnGreen")]
    [Browsable(true)]
    public Color ProgressBarForeColor
    {
      get
      {
        return _progressBarForeColor;
      }
      set
      {
        _progressBarForeColor = value;
        BusyProgressBar.ForeColor = _progressBarForeColor;
      }
    }


    private Color _progressBarBackColor = Color.White;
    /// <summary>
    /// Set or get background color for busy animation's progress bar 
    /// </summary>
    [Category("Csla")]
    [Description("Background color for busy animation's progress bar.")]
    [DefaultValue(typeof(Color), "White")]
    [Browsable(true)]
    public Color ProgressBarBackColor
    {
      get
      {
        return _progressBarBackColor;
      }
      set
      {
        _progressBarBackColor = value;
        BusyProgressBar.BackColor = _progressBarBackColor;
      }
    }

    private bool _isRunning = false;
    /// <summary>
    /// Indicates if animation needs to be shown.  Set to true to start 
    /// progress bar animation
    /// </summary>
    [Category("Csla")]
    [Description("Indicates if animation needs to be shown.  Set to true to start progress bar animation")]
    [DefaultValue(false)]
    [Bindable(true)]
    [Browsable(true)]
    public bool IsRunning
    {
      get
      {
        return _isRunning;
      }
      set
      {
        _isRunning = value;
        Run(_isRunning);
      }
    }

    private void Run(bool run)
    {
      if (!IsInDesignMode)
      {
        Visible = run;
        BusyProgressBar.Visible = run;
        ProgressTimer.Enabled = run;
      }
    }


    private void ProgressTimer_Tick(object sender, EventArgs e)
    {
      if (_isRunning)
      {
        int newValue = BusyProgressBar.Value + BusyProgressBar.Step;
        if (newValue > BusyProgressBar.Maximum)
        {
          BusyProgressBar.Value = 0;
        }
        else
        {
          BusyProgressBar.Value = newValue;
        }
      }
    }

    private bool IsInDesignMode
    {
      get
      {
        if (GetService(typeof(System.ComponentModel.Design.IDesignerHost)) != null)
          return true;
        else
          return false;
      }
    }

    private void BusyAnimation_Load(object sender, EventArgs e)
    {
      if (IsInDesignMode)
      {
        BusyProgressBar.Value = BusyProgressBar.Maximum / 2;
        BusyProgressBar.Visible = true;
      }
    }

  }
}