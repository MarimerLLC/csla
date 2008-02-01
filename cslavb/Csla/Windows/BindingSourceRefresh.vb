Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Windows.Forms

' code from Bill McCarthy
' http://msmvps.com/bill/archive/2005/10/05/69012.aspx
' used with permission

Namespace Windows

#Region "Delegates"

  ''' <summary>
  ''' BindingErrorEventHandler delegates is the event handling definition for handling data binding errors that occurred due to exceptions.
  ''' </summary>
  ''' <param name="sender">The object that triggered the event.</param>
  ''' <param name="e">The event arguments.</param>
  Public Delegate Sub BindingErrorEventHandler(ByVal sender As Object, ByVal e As BindingErrorEventArgs)

#End Region

#Region "BindingErrorEventArgs Class"

  ''' <summary>
  ''' BindingErrorEventArgs defines the event arguments for reporting a data binding error due to a exception.
  ''' </summary>
  Public Class BindingErrorEventArgs
    Inherits EventArgs

#Region "Property Fields"

    Private _exception As Exception = Nothing
    Private _binding As Binding = Nothing

#End Region

#Region "Properties"

    ''' <summary>
    ''' Exception gets the exception that caused the binding error.
    ''' </summary>
    Public ReadOnly Property Exception() As Exception
      Get
        Return (_exception)
      End Get
    End Property

    ''' <summary>
    ''' Binding gets the binding that caused the exception.
    ''' </summary>
    Public ReadOnly Property Binding() As Binding
      Get
        Return (_binding)
      End Get
    End Property

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Constructor creates a new BindingErrorEventArgs object instance using the information specified.
    ''' </summary>
    ''' <param name="binding">The binding that caused th exception.</param>
    ''' <param name="exception">The exception that caused the error.</param>
    Public Sub New(ByVal binding As Binding, ByVal exception As Exception)
      _binding = binding
      _exception = exception
    End Sub

#End Region

  End Class

#End Region

  ''' <summary>
  ''' BindingSourceRefresh contains functionality for refreshing the data bound to controls on Host as well as a mechinism for catching data
  ''' binding errors that occur in Host.
  ''' </summary>
  ''' <remarks>Windows Forms extender control that resolves the
  ''' data refresh issue with data bound detail controls
  ''' as discussed in Chapter 5.</remarks>
  <DesignerCategory(""), Designer(GetType(HostComponentDesigner)), HostProperty("Host"), ProvideProperty("ReadValuesOnChange", GetType(BindingSource))> _
  Public Class BindingSourceRefresh
    Inherits Component

    Implements IExtenderProvider
    Implements ISupportInitialize

#Region "Fields"

    Private _sources As Dictionary(Of BindingSource, Boolean) = New Dictionary(Of BindingSource, Boolean)()

#End Region

#Region "Property Fields"

    Private _host As ContainerControl = Nothing

#End Region

#Region "Events"

    ''' <summary>
    ''' BindingError event is raised when a data binding error occurs due to a exception.
    ''' </summary>
    Public Event BindingError As BindingErrorEventHandler

#End Region

#Region "Properties"

    ''' <summary>
    ''' Host gets/sets the component's containing host control (form).
    ''' </summary>
    <Browsable(False)> _
    <DefaultValue(CType(Nothing, Object))> _
    Public Property Host() As ContainerControl
      Get
        Return (_host)
      End Get
      Set(ByVal value As ContainerControl)
        If _host IsNot value Then
          ' If we are not initialising then unregister any existing host events.
          If (Not _isInitialising) AndAlso (_host IsNot Nothing) Then
            RegisterControlEvents(_host, False)
          End If
          _host = value
          ' If we are not initialisin then register the host events.
          If (Not _isInitialising) AndAlso (_host IsNot Nothing) Then
            RegisterControlEvents(_host, True)
          End If
        End If
      End Set
    End Property

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Constructor creates a new BindingSourceRefresh component then initialises all the different sub components.
    ''' </summary>
    Public Sub New()
      InitializeComponent()
    End Sub

    ''' <summary>
    ''' Constructor creates a new BindingSourceRefresh component, adds the component to the container supplied before initialising all the different sub components.
    ''' </summary>
    ''' <param name="container">The container the component is to be added to.</param>
    Public Sub New(ByVal container As IContainer)
      container.Add(Me)

      InitializeComponent()
    End Sub

#End Region


#Region "Designer Functionality"

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary> 
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso (components IsNot Nothing) Then
        components.Dispose()
      End If
      MyBase.Dispose(disposing)
    End Sub

#Region "Component Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
      components = New System.ComponentModel.Container()
    End Sub

#End Region

#End Region

#Region "Public Methods"


    ''' <summary>
    ''' CanExtend() returns true if extendee is a binding source.
    ''' </summary>
    ''' <param name="extendee">The control to be extended.</param>
    ''' <returns>True if the control is a binding source, else false.</returns>
    Public Function CanExtend(ByVal extendee As Object) As Boolean Implements IExtenderProvider.CanExtend
      Return (TypeOf extendee Is BindingSource)
    End Function

    ''' <summary>
    ''' GetReadValuesOnChange() gets the value of the custom ReadValuesOnChange extender property added to extended controls.
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    Public Function GetReadValuesOnChange(ByVal source As BindingSource) As Boolean
      Dim result As Boolean
      If _sources.TryGetValue(source, result) Then
        Return result
      Else
        Return False
      End If
    End Function

    ''' <summary>
    ''' SetReadValuesOnChange() sets the value of the custom ReadValuesOnChange extender
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    ''' <param name="value">New value of property.</param>
    ''' <remarks></remarks>
    Public Sub SetReadValuesOnChange(ByVal source As BindingSource, ByVal value As Boolean)
      _sources(source) = value
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' RegisterControlEvents() registers all the relevant events for the container control supplied and also to all child controls
    ''' in the oontainer control.
    ''' </summary>
    ''' <param name="container">The control (including child controls) to have the refresh events registered.</param>
    ''' <param name="register">True to register the events, false to unregister them.</param>
    Private Sub RegisterControlEvents(ByVal container As Control, ByVal register As Boolean)
      ' If we are to register the events the do so.
      If register Then
        AddHandler container.DataBindings.CollectionChanged, AddressOf DataBindings_CollectionChanged
        AddHandler container.ControlAdded, AddressOf Container_ControlAdded
        AddHandler container.ControlRemoved, AddressOf Container_ControlRemoved
        ' Else unregister them.
      Else
        RemoveHandler container.DataBindings.CollectionChanged, AddressOf DataBindings_CollectionChanged
        RemoveHandler container.ControlAdded, AddressOf Container_ControlAdded
        RemoveHandler container.ControlRemoved, AddressOf Container_ControlRemoved
      End If

      ' Reigster the binding complete events for the control.
      RegisterBindingEvents(container, register)

      ' Register/unregister the events on all child controls.
      For Each control As Control In container.Controls
        RegisterControlEvents(control, register)
      Next control
    End Sub

    ''' <summary>
    ''' RegisterBindingEvents() registers the binding complete event to all data bindings in control.
    ''' </summary>
    ''' <param name="control">The control whose binding complete events are to be registered.</param>
    ''' <param name="register">True to register the events, false to unregister them.</param>
    Private Sub RegisterBindingEvents(ByVal control As Control, ByVal register As Boolean)
      For Each binding As Binding In control.DataBindings
        If register Then
          AddHandler binding.BindingComplete, AddressOf Control_BindingComplete
        Else
          RemoveHandler binding.BindingComplete, AddressOf Control_BindingComplete
        End If
      Next binding
    End Sub

#End Region

#Region "Event Methods"

    ''' <summary>
    ''' DataBindings_CollectionChanged() is the data bindings collection change event for a control.
    ''' DataBindings_CollectionChanged() simply updates our binding events hookins correctly based on the collections
    ''' current state.
    ''' </summary>
    ''' <param name="sender">The object that triggered the event.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub DataBindings_CollectionChanged(ByVal sender As Object, ByVal e As CollectionChangeEventArgs)
      Select Case e.Action
        Case CollectionChangeAction.Add
          ' To prevent duplicate binding complete events unregister the existing bindings first then re-register
          ' them all for the control.
          RegisterBindingEvents((CType(sender, ControlBindingsCollection)).Control, False)
          RegisterBindingEvents((CType(sender, ControlBindingsCollection)).Control, True)
        Case CollectionChangeAction.Remove
          RegisterBindingEvents((CType(sender, ControlBindingsCollection)).Control, False)
      End Select
    End Sub

    ''' <summary>
    ''' Container_ControlAdded() is the control add event for a control's control collection.
    ''' Container_ControlAdded() simply registers the relevant controller events for the new control as well as registering
    ''' any required bindings - including those for child controls (based on the controls binding source).
    ''' </summary>
    ''' <param name="sender">The object that triggered the event.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub Container_ControlAdded(ByVal sender As Object, ByVal e As ControlEventArgs)
      RegisterControlEvents(e.Control, True)
    End Sub

    ''' <summary>
    ''' Container_ControlRemoved() is the control remove event for a control's control collection.
    ''' Container_ControlRemoved() unregisters all events associated with the control - including those for child controls.
    ''' </summary>
    ''' <param name="sender">The object that triggered the event.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub Container_ControlRemoved(ByVal sender As Object, ByVal e As ControlEventArgs)
      RegisterControlEvents(e.Control, False)
    End Sub

    ''' <summary>
    ''' Control_BindingComplete() is a event driven routine triggered when one of the control's bindings has been completed.
    ''' Control_BindingComplete() simply validates the result where if the result was a exception then the BindingError
    ''' event is raised, else if the binding was a successful data source update and we are to re-read the value on changed then
    ''' the binding value is reread into the control.
    ''' </summary>
    ''' <param name="sender">The object that triggered the event.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub Control_BindingComplete(ByVal sender As Object, ByVal e As BindingCompleteEventArgs)
      Select Case e.BindingCompleteState
        Case BindingCompleteState.Exception
          RaiseEvent BindingError(Me, New BindingErrorEventArgs(e.Binding, e.Exception))
        Case Else
          If (e.BindingCompleteContext = BindingCompleteContext.DataSourceUpdate) AndAlso TypeOf e.Binding.DataSource Is BindingSource AndAlso GetReadValuesOnChange(CType(e.Binding.DataSource, BindingSource)) Then
            e.Binding.ReadValue()
          End If
      End Select
    End Sub

#End Region

#Region "ISupportInitialize Interface"

    Private _isInitialising As Boolean = False

    ''' <summary>
    ''' BeginInit() is called when the component is starting to be initialised. BeginInit() simply sets the initialisation flag to true.
    ''' </summary>
    Private Sub BeginInit() Implements ISupportInitialize.BeginInit
      _isInitialising = True
    End Sub

    ''' <summary>
    ''' EndInit() is called when the component has finished being initialised.  EndInt() sets the initialise flag to false then runs
    ''' through registering all the different events that the component needs to hook into in Host.
    ''' </summary>
    Private Sub EndInit() Implements ISupportInitialize.EndInit
      _isInitialising = False
      If Host IsNot Nothing Then
        RegisterControlEvents(Host, True)
      End If
    End Sub

#End Region

  End Class

End Namespace
