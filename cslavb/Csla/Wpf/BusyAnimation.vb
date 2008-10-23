Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Threading
Imports System.Windows.Threading
Imports System.ComponentModel
Imports System.Globalization

Namespace Wpf

  <TemplatePart(Name:="part1", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part2", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part3", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part4", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part5", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part6", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part7", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="part8", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="root", Type:=GetType(Canvas))> _
  <TemplatePart(Name:="normal", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state1", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state2", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state3", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state4", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state5", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state6", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state7", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="state8", Type:=GetType(Storyboard))> _
  Public Class BusyAnimation
    Inherits Control

#Region "Constants"

    Private Const NUM_STATES As Integer = 8

#End Region

#Region "Dependency properties"


    Public Shared ReadOnly IsRunningProperty As DependencyProperty = DependencyProperty.Register( _
                                                                      "IsRunning", _
                                                                      GetType(Object), _
                                                                      GetType(BusyAnimation), _
                                                                      New FrameworkPropertyMetadata( _
                                                                      False, _
                                                                      FrameworkPropertyMetadataOptions.AffectsRender, _
                                                                      New PropertyChangedCallback(AddressOf OnIsRunningPropertyChanged)))

    Private Shared Sub OnIsRunningPropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      CType(o, BusyAnimation).SetState(CType(e.NewValue, Boolean))
    End Sub


    Public Shared ReadOnly StateDurationProperty As DependencyProperty = DependencyProperty.Register( _
                                                                      "StateDuration", _
                                                                      GetType(TimeSpan), _
                                                                      GetType(BusyAnimation), _
                                                                      New FrameworkPropertyMetadata( _
                                                                      TimeSpan.FromMilliseconds(150), _
                                                                      FrameworkPropertyMetadataOptions.AffectsRender, _
                                                                      New PropertyChangedCallback(AddressOf OnStateDurationPropertyChanged)))


    Private Shared Sub OnStateDurationPropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      CType(o, BusyAnimation)._timer.Interval = CType(e.NewValue, TimeSpan)
    End Sub

#End Region

    Private _timer As DispatcherTimer
    Private _root As Canvas
    Private _normalStoryboard As Storyboard
    Private _isRunningStoryboard As Storyboard()


#Region "Member fields and properties"

    Public Property StateDuration() As TimeSpan
      Get
        Return DirectCast(GetValue(StateDurationProperty), TimeSpan)
      End Get
      Set(ByVal value As TimeSpan)
        SetValue(StateDurationProperty, value)
        _timer.Interval = value
      End Set
    End Property

    Public Property IsRunning() As Boolean
      Get
        Return CBool(GetValue(IsRunningProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsRunningProperty, value)
        SetState(value)
      End Set
    End Property

    Private Overloads Sub SetState(ByVal isRunning As Boolean)
      If _isRunningStoryboard IsNot Nothing Then
        If isRunning Then
          _timer.Start()
        Else
          _timer.[Stop]()
          _normalStoryboard.Begin(_root)
        End If
      End If
    End Sub

    Private _frame As Integer = 0

    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
      _isRunningStoryboard(_frame).Begin(_root)
      _frame = (_frame + 1) Mod NUM_STATES
    End Sub

#End Region

#Region "Constructors"
    Public Sub New()

      _timer = New DispatcherTimer()
      _timer.Interval = StateDuration
      AddHandler _timer.Tick, AddressOf timer_Tick

      DefaultStyleKey = GetType(BusyAnimation)

    End Sub

#End Region

    Private Sub BusyAnimation_LayoutUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LayoutUpdated
      ArrangeParts()
    End Sub


    Private Sub BusyAnimation_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
      ArrangeParts()
      BuildStoryboard()
      SetState(IsRunning)

    End Sub

    Private Sub BusyAnimation_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
      ArrangeParts()
    End Sub


    Private Sub ArrangeParts()
      Dim width As Double = ActualWidth
      Dim height As Double = ActualHeight
      Dim scale As Double = Math.Min(ActualWidth, ActualHeight)
      Dim theta As Double = (2 * Math.PI) / NUM_STATES
      For n As Integer = 0 To NUM_STATES - 1

        Dim item As FrameworkElement = DirectCast(Template.FindName("part" + (n + 1).ToString(), Me), FrameworkElement)
        If item IsNot Nothing Then
          Dim itemTheta As Double = theta * CDbl(n)

          Dim itemScale As Double = scale / 4
          Dim radius As Double = (scale - itemScale) / 2

          Dim x As Double = (radius * Math.Cos(itemTheta)) - (itemScale / 2) + (ActualWidth / 2)
          Dim y As Double = (radius * Math.Sin(itemTheta)) - (itemScale / 2) + (ActualHeight / 2)

          item.Width = itemScale
          item.Height = itemScale
          item.SetValue(Canvas.LeftProperty, x)
          item.SetValue(Canvas.TopProperty, y)
        End If
      Next
    End Sub

    Private Sub BuildStoryboard()
      _root = DirectCast(Template.FindName("root", Me), Canvas)
      _isRunningStoryboard = New Storyboard(7) {}
      _isRunningStoryboard(0) = DirectCast(Template.Resources("state1"), Storyboard)
      _isRunningStoryboard(1) = DirectCast(Template.Resources("state2"), Storyboard)
      _isRunningStoryboard(2) = DirectCast(Template.Resources("state3"), Storyboard)
      _isRunningStoryboard(3) = DirectCast(Template.Resources("state4"), Storyboard)
      _isRunningStoryboard(4) = DirectCast(Template.Resources("state5"), Storyboard)
      _isRunningStoryboard(5) = DirectCast(Template.Resources("state6"), Storyboard)
      _isRunningStoryboard(6) = DirectCast(Template.Resources("state7"), Storyboard)
      _isRunningStoryboard(7) = DirectCast(Template.Resources("state8"), Storyboard)
      _normalStoryboard = DirectCast(Template.Resources("normal"), Storyboard)
    End Sub
  End Class
End Namespace
