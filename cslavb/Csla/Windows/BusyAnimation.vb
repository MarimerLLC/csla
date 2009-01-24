Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Namespace Windows

  ''' <summary>
  ''' User control that displays busy animation
  ''' </summary>
  ''' <remarks></remarks>
  <ToolboxItem(True), ToolboxBitmap(GetType(BusyAnimation), "Csla.Windows.BusyAnimation")> _
  Public Class BusyAnimation
    Inherits UserControl

    ''' <summary>
    ''' new instance busy animation
    ''' </summary>
    Public Sub New()
      InitializeComponent()
      Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
      Me.BusyProgressBar.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.FlattenHierarchy Or System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.IgnoreCase).Invoke(Me.BusyProgressBar, New Object() {ControlStyles.SupportsTransparentBackColor, True})

      If Not IsInDesignMode Then
        Me.BusyProgressBar.BackColor = _progressBarBackColor
      End If
    End Sub

    Private _progressBarForeColor As Color = System.Drawing.Color.LawnGreen

    ''' <summary>
    ''' Set or get foreground color for busy animation's progress bar
    ''' </summary>
    <Category("Csla"), Description("Foreground color for busy animation's progress bar."), DefaultValue(GetType(System.Drawing.Color), "LawnGreen"), Browsable(True)> _
    Public Property ProgressBarForeColor() As Color
      Get
        Return _progressBarForeColor
      End Get
      Set(ByVal value As Color)
        _progressBarForeColor = value
        Me.BusyProgressBar.ForeColor = _progressBarForeColor
      End Set
    End Property

    Private _progressBarBackColor As Color = System.Drawing.Color.White
    Private WithEvents ProgressTimer As System.Windows.Forms.Timer
    Private components As System.ComponentModel.IContainer
    Private WithEvents BusyProgressBar As System.Windows.Forms.ProgressBar
    ''' <summary>
    ''' Set or get background color for busy animation's progress bar 
    ''' </summary>
    <Category("Csla"), Description("Background color for busy animation's progress bar."), DefaultValue(GetType(System.Drawing.Color), "White"), Browsable(True)> _
    Public Property ProgressBarBackColor() As Color
      Get
        Return _progressBarBackColor
      End Get
      Set(ByVal value As Color)
        _progressBarBackColor = value
        Me.BusyProgressBar.BackColor = _progressBarBackColor
      End Set
    End Property

    Private _isRunning As Boolean = False
    ''' <summary>
    ''' Indicates if animation needs to be shown.  Set to true to start 
    ''' progress bar animation
    ''' </summary>
    <Category("Csla"), _
    Description("Indicates if animation needs to be shown.  Set to true to start progress bar animation"), _
    DefaultValue(False), Bindable(True), Browsable(True)> _
    Public Property IsRunning() As Boolean
      Get
        Return _isRunning
      End Get
      Set(ByVal value As Boolean)
        _isRunning = value
        Run(_isRunning)
      End Set
    End Property

    Private Sub Run(ByVal run As Boolean)
      If Not IsInDesignMode Then
        Me.Visible = run
        Me.BusyProgressBar.Visible = run
        Me.ProgressTimer.Enabled = run
      End If
    End Sub

    Private Sub ProgressTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
      If _isRunning Then
        Dim newValue As Integer = Me.BusyProgressBar.Value + Me.BusyProgressBar.Step
        If newValue > Me.BusyProgressBar.Maximum Then
          Me.BusyProgressBar.Value = 0
        Else
          Me.BusyProgressBar.Value = newValue
        End If
      End If
    End Sub

    Private ReadOnly Property IsInDesignMode() As Boolean
      Get
        If GetService(GetType(System.ComponentModel.Design.IDesignerHost)) IsNot Nothing Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    Private Sub BusyAnimation_Load(ByVal sender As Object, ByVal e As EventArgs)
      If IsInDesignMode Then
        Me.BusyProgressBar.Value = CType(Me.BusyProgressBar.Maximum / 2, Integer)
        Me.BusyProgressBar.Visible = True
      End If
    End Sub

    Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container
      Me.ProgressTimer = New System.Windows.Forms.Timer(Me.components)
      Me.BusyProgressBar = New System.Windows.Forms.ProgressBar
      Me.SuspendLayout()
      '
      'ProgressTimer
      '
      Me.ProgressTimer.Interval = 1
      '
      'BusyProgressBar
      '
      Me.BusyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill
      Me.BusyProgressBar.ForeColor = System.Drawing.Color.LawnGreen
      Me.BusyProgressBar.Location = New System.Drawing.Point(0, 0)
      Me.BusyProgressBar.Maximum = 30
      Me.BusyProgressBar.Name = "BusyProgressBar"
      Me.BusyProgressBar.Size = New System.Drawing.Size(103, 27)
      Me.BusyProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous
      Me.BusyProgressBar.TabIndex = 0
      '
      'BusyAnimation
      '
      Me.Controls.Add(Me.BusyProgressBar)
      Me.Name = "BusyAnimation"
      Me.Size = New System.Drawing.Size(103, 27)
      Me.ResumeLayout(False)

    End Sub
  End Class
End Namespace

