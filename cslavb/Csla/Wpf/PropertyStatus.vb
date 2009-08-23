Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Reflection
Imports System.Windows
Imports Csla.Core
Imports System.Windows.Media
Imports System.Windows.Controls.Primitives
Imports Csla.Validation
Imports System.Windows.Controls
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Windows.Media.Animation

Namespace Wpf

  ''' <summary>
  ''' Control providing services around business object
  ''' validation, authorization and async busy status.
  ''' </summary>
  <TemplatePart(Name:="root", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="popup", Type:=GetType(Popup))> _
  <TemplatePart(Name:="errorImage", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="warningImage", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="informationImage", Type:=GetType(FrameworkElement))> _
  <TemplatePart(Name:="busy", Type:=GetType(BusyAnimation))> _
  <TemplatePart(Name:="Valid", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="Error", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="Warning", Type:=GetType(Storyboard))> _
  <TemplatePart(Name:="Information", Type:=GetType(Storyboard))> _
  Public Class PropertyStatus
    Inherits ContentControl
#Region "Constructors"

    ''' <summary>
    ''' Creates an instance of the type.
    ''' </summary>
    Public Sub New()
      DefaultStyleKey = GetType(PropertyStatus)
      BrokenRules = New ObservableCollection(Of BrokenRule)()

      'TODO: Need to implement Lambda expression
      AddHandler Loaded, Function(o, e) (UpdateState())

    End Sub

    ''' <summary>
    ''' Invoked whenever application code
    ''' or internal processes call System.Windows.FrameworkElement.ApplyTemplate()
    ''' Once template is applied to the control,force state update.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
      MyBase.ApplyTemplate()
      UpdateState()
    End Sub

#End Region

    Private Sub PropertyStatus_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
      GoToState(True)
    End Sub


#Region "Dependency properties"

    ''' <summary>
    ''' Reference to the data source object.
    ''' </summary>
    Public Shared ReadOnly SourceProperty As DependencyProperty = DependencyProperty.Register( _
    "Source", _
    GetType(Object), _
    GetType(PropertyStatus), _
    New FrameworkPropertyMetadata( _
      Nothing, _
      FrameworkPropertyMetadataOptions.AffectsRender, _
      New PropertyChangedCallback(AddressOf OnSourcePropertyChanged))) 'TODO I think this is right but not sure

    Private Shared Sub OnSourcePropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
      CType(o, PropertyStatus).SetSource(e.OldValue, e.NewValue)
    End Sub

    ''' <summary>
    ''' Defines the business object property to watch for
    ''' validation, authorization and busy status.
    ''' </summary>
    Public Shared ReadOnly PropertyProperty As DependencyProperty = DependencyProperty.Register("Property", GetType(String), GetType(PropertyStatus))

    ''' <summary>
    ''' Gets a reference to the business object's
    ''' broken rules collection.
    ''' </summary>
    Public Shared ReadOnly BrokenRulesProperty As DependencyProperty = DependencyProperty.Register("BrokenRules", GetType(ObservableCollection(Of BrokenRule)), GetType(PropertyStatus))

    ''' <summary>
    ''' Reference to the target UI control to be managed
    ''' for authorization rules.
    ''' </summary>
    Public Shared ReadOnly TargetProperty As DependencyProperty = DependencyProperty.Register("Target", GetType(DependencyObject), GetType(PropertyStatus))

    ''' <summary>
    ''' Gets or sets a value indicating whether the PropertyStatus
    ''' control should be in busy mode.
    ''' </summary>
    Public Shared ReadOnly IsBusyProperty As DependencyProperty = DependencyProperty.Register("IsBusy", GetType(Boolean), GetType(PropertyStatus))

    ''' <summary>
    ''' Reference to the template for the validation rule popup.
    ''' </summary>
    Public Shared ReadOnly PopupTemplateProperty As DependencyProperty = DependencyProperty.Register("PopupTemplate", GetType(ControlTemplate), GetType(PropertyStatus))

#End Region

#Region " Member fields and properties "

    Private _isReadOnly As Boolean = False
    Private _isValid As Boolean = True
    Private _worst As RuleSeverity
    Private _lastImage As FrameworkElement

    ''' <summary>
    ''' Gets or sets a reference to the data source object.
    ''' </summary>
    Public Property Source() As Object
      Get
        Return GetValue(SourceProperty)
      End Get
      Set(ByVal value As Object)
        Dim oldValue As Object = Source
        Dim newValue As Object = value
        SetValue(SourceProperty, value)
        SetSource(oldValue, newValue)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the business object
    ''' property to be monitored.
    ''' </summary>
    Public Property [Property]() As String

      Get
        Return DirectCast(GetValue(PropertyProperty), String)
      End Get
      Set(ByVal value As String)
        SetValue(PropertyProperty, value)
        CheckProperty()
      End Set
    End Property

    ''' <summary>
    ''' Gets a reference to the business object's
    ''' broken rules collection.
    ''' </summary>
    Public Property BrokenRules() As ObservableCollection(Of BrokenRule)
      Get
        Return DirectCast(GetValue(BrokenRulesProperty), ObservableCollection(Of BrokenRule))
      End Get
      Private Set(ByVal value As ObservableCollection(Of BrokenRule))
        SetValue(BrokenRulesProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a reference to the UI control to
    ''' be managed based on authorization rules.
    ''' </summary>
    Public Property Target() As DependencyObject
      Get
        Return DirectCast(GetValue(TargetProperty), DependencyObject)
      End Get
      Set(ByVal value As DependencyObject)
        SetValue(TargetProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the PropertyStatus
    ''' control should be in busy mode.
    ''' </summary>
    Public Property IsBusy() As Boolean
      Get
        Return CBool(GetValue(IsBusyProperty))
      End Get
      Set(ByVal value As Boolean)
        SetValue(IsBusyProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the template for the validation rules
    ''' popup.
    ''' </summary>
    Public Property PopupTemplate() As ControlTemplate
      Get
        Return DirectCast(GetValue(PopupTemplateProperty), ControlTemplate)
      End Get
      Set(ByVal value As ControlTemplate)
        SetValue(PopupTemplateProperty, value)
      End Set
    End Property

#End Region

#Region " Source "

    Private Sub SetSource(ByVal oldSource As Object, ByVal newSource As Object)
      DetachSource(oldSource)
      AttachSource(newSource)

      Dim bb As BusinessBase = TryCast(newSource, BusinessBase)
      If bb IsNot Nothing AndAlso Not String.IsNullOrEmpty([Property]) Then
        IsBusy = bb.IsPropertyBusy([Property])
      End If

      CheckProperty()
      UpdateState()
    End Sub

    Private Sub AttachSource(ByVal source As Object)
      Dim busy As INotifyBusy = TryCast(source, INotifyBusy)
      If busy IsNot Nothing Then
        AddHandler busy.BusyChanged, AddressOf source_BusyChanged
      End If

      Dim changed As INotifyPropertyChanged = TryCast(source, INotifyPropertyChanged)
      If changed IsNot Nothing Then
        AddHandler changed.PropertyChanged, AddressOf source_PropertyChanged
      End If
    End Sub

    Private Sub DetachSource(ByVal source As Object)
      Dim busy As INotifyBusy = TryCast(source, INotifyBusy)
      If busy IsNot Nothing Then
        RemoveHandler busy.BusyChanged, AddressOf source_BusyChanged
      End If

      Dim changed As INotifyPropertyChanged = TryCast(source, INotifyPropertyChanged)
      If changed IsNot Nothing Then
        RemoveHandler changed.PropertyChanged, AddressOf source_PropertyChanged
      End If
    End Sub

    Private Sub source_BusyChanged(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
      If e.PropertyName = [Property] Then
        IsBusy = e.Busy
      End If

      UpdateState()
    End Sub

    Private Sub source_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
      If e.PropertyName = [Property] OrElse String.IsNullOrEmpty(e.PropertyName) Then
        UpdateState()
      End If
    End Sub

    Private Function UpdateState() As Boolean
      Dim popup As Popup = CType(FindName("popup"), Popup)

      If popup IsNot Nothing Then
        popup.IsOpen = False
      End If

      Dim businessObject As BusinessBase = TryCast(Source, BusinessBase)
      If businessObject IsNot Nothing Then
        'for some reason Linq does not work against BrokenRulesCollection...
        Dim allRules As List(Of BrokenRule) = New List(Of BrokenRule)

        For Each r In businessObject.BrokenRulesCollection
          If r.Property = [Property] Then
            allRules.Add(r)
          End If
        Next

        Dim removeRules = (From r In BrokenRules _
                           Where Not allRules.Contains(r) _
                           Select r).ToArray()

        Dim addRules = (From r In allRules _
                        Where Not BrokenRules.Contains(r) _
                        Select r).ToArray()

        For Each rule In removeRules
          BrokenRules.Remove(rule)
        Next

        For Each rule In addRules
          BrokenRules.Add(rule)
        Next

        Dim worst As BrokenRule = (From r In BrokenRules _
                               Order By r.Severity _
                               Select r).FirstOrDefault()


        If worst IsNot Nothing Then
          _worst = worst.Severity

          _isValid = (BrokenRules.Count = 0)
          GoToState(True)
        Else
          BrokenRules.Clear()
          _isValid = True
          GoToState(True)
        End If
      End If

    End Function

#End Region

#Region " Image "

    Private Sub EnablePopup(ByVal image As FrameworkElement)
      If image IsNot Nothing Then
        AddHandler image.MouseEnter, AddressOf image_MouseEnter
        AddHandler image.MouseLeave, AddressOf image_MouseLeave
      End If
    End Sub

    Private Sub DisablePopup(ByVal image As FrameworkElement)
      If image IsNot Nothing Then
        RemoveHandler image.MouseEnter, AddressOf image_MouseEnter
        RemoveHandler image.MouseLeave, AddressOf image_MouseLeave
      End If
    End Sub

    Private Sub image_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim popup As Popup = DirectCast(Template.FindName("popup", Me), Popup)
      popup.IsOpen = True
    End Sub

    Private Sub image_MouseLeave(ByVal sender As Object, ByVal e As MouseEventArgs)
      Dim popup As Popup = DirectCast(Template.FindName("popup", Me), Popup)
      popup.IsOpen = False
    End Sub

#End Region

#Region " State management "

    Private Sub GoToState(ByVal useTransitions As Boolean)
      If IsLoaded AndAlso Not DataPortal.IsInDesignMode Then
        DisablePopup(_lastImage)
        HandleTarget()

        Dim root As FrameworkElement = DirectCast(Template.FindName("root", Me), FrameworkElement)

        If root IsNot Nothing Then
          If _isValid Then
            Dim validStoryboard As Storyboard = DirectCast(Template.Resources("Valid"), Storyboard)
            validStoryboard.Begin(root)
          Else
            Dim errorStoryboard As Storyboard = DirectCast(Template.Resources(_worst.ToString()), Storyboard)
            errorStoryboard.Begin(root)
            _lastImage = DirectCast(Template.FindName(String.Format("{0}Image", _worst.ToString().ToLower()), Me), FrameworkElement)
            EnablePopup(_lastImage)
          End If
        End If
      End If
    End Sub

#End Region

#Region " RelativeTarget "

    Private Sub CheckProperty()
      If Source IsNot Nothing Then
        Dim desc = Csla.Reflection.MethodCaller.GetPropertyDescriptor(Source.GetType(), [Property])
        If desc IsNot Nothing Then
          _isReadOnly = desc.IsReadOnly
        Else
          _isReadOnly = False
        End If
      Else
        _isReadOnly = True
      End If
    End Sub

    Private Sub HandleTarget()
      If Target IsNot Nothing AndAlso Not String.IsNullOrEmpty([Property]) Then
        Dim b As Security.IAuthorizeReadWrite = TryCast(Source, Security.IAuthorizeReadWrite)
        If b IsNot Nothing Then
          Dim canRead As Boolean = b.CanReadProperty([Property])
          Dim canWrite As Boolean = b.CanWriteProperty([Property])

          If canWrite AndAlso Not _isReadOnly Then
            MethodCaller.CallMethodIfImplemented(Target, "set_IsReadOnly", False)
            MethodCaller.CallMethodIfImplemented(Target, "set_IsEnabled", True)
          Else
            MethodCaller.CallMethodIfImplemented(Target, "set_IsReadOnly", True)
            MethodCaller.CallMethodIfImplemented(Target, "set_IsEnabled", False)
          End If

          If Not canRead Then
            MethodCaller.CallMethodIfImplemented(Target, "set_Content", Nothing)
            MethodCaller.CallMethodIfImplemented(Target, "set_Text", "")
          End If
        End If
      End If
    End Sub

#End Region

  End Class
End Namespace