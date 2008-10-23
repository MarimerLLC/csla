using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

// code from Bill McCarthy
// http://msmvps.com/bill/archive/2005/10/05/69012.aspx
// used with permission

namespace Csla.Windows
{
  /// <summary>
  /// BindingSourceRefresh contains functionality for refreshing the data bound to controls on Host as well as a mechinism for catching data
  /// binding errors that occur in Host.
  /// </summary>
  /// <remarks>Windows Forms extender control that resolves the
  /// data refresh issue with data bound detail controls
  /// as discussed in Chapter 5.</remarks>
  [DesignerCategory("")]
#if !CLIENTONLY
  [Designer(typeof(HostComponentDesigner))]
#endif
  [HostProperty("Host")]
  [ProvideProperty("ReadValuesOnChange", typeof(BindingSource))]
  public class BindingSourceRefresh : Component, IExtenderProvider, ISupportInitialize
  {

    #region Fields

    private Dictionary<BindingSource, bool> _sources = new Dictionary<BindingSource, bool>();

    #endregion

    #region Property Fields

    private ContainerControl _host = null;

    #endregion

    #region Events

    /// <summary>
    /// BindingError event is raised when a data binding error occurs due to a exception.
    /// </summary>
    public event BindingErrorEventHandler BindingError = null;

    #endregion

    #region Properties

    /// <summary>
    /// Host gets/sets the component's containing host control (form).
    /// </summary>
    [Browsable(false)]
    [DefaultValue(null)]
    public ContainerControl Host
    {
      get { return (_host); }
      set
      {
        if (_host != value)
        {
          // If we are not initialising then unregister any existing host events.
          if (!_isInitialising && (_host != null))
          {
            RegisterControlEvents(_host, false);
          }
          _host = value;
          // If we are not initialisin then register the host events.
          if (!_isInitialising && (_host != null))
          {
            RegisterControlEvents(_host, true);
          }
        }
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor creates a new BindingSourceRefresh component then initialises all the different sub components.
    /// </summary>
    public BindingSourceRefresh()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Constructor creates a new BindingSourceRefresh component, adds the component to the container supplied before initialising all the different sub components.
    /// </summary>
    /// <param name="container">The container the component is to be added to.</param>
    public BindingSourceRefresh(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }

    #endregion
    
    #region Designer Functionality

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }

    #endregion

    #endregion

    #region Public Methods


    /// <summary>
    /// CanExtend() returns true if extendee is a binding source.
    /// </summary>
    /// <param name="extendee">The control to be extended.</param>
    /// <returns>True if the control is a binding source, else false.</returns>
    public bool CanExtend(object extendee)
    {
      return (extendee is BindingSource);
    }

    /// <summary>
    /// GetReadValuesOnChange() gets the value of the custom ReadValuesOnChange extender property added to extended controls.
    /// property added to extended controls.
    /// </summary>
    /// <param name="source">Control being extended.</param>
    [Category("Csla")]
    public bool GetReadValuesOnChange(BindingSource source)
    {
      bool result;
      if (_sources.TryGetValue(source, out result))
        return result;
      else
        return false;
    }

    /// <summary>
    /// SetReadValuesOnChange() sets the value of the custom ReadValuesOnChange extender
    /// property added to extended controls.
    /// </summary>
    /// <param name="source">Control being extended.</param>
    /// <param name="value">New value of property.</param>
    /// <remarks></remarks>
    [Category("Csla")]
    public void SetReadValuesOnChange(
      BindingSource source, bool value)
    {
      _sources[source] = value;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// RegisterControlEvents() registers all the relevant events for the container control supplied and also to all child controls
    /// in the oontainer control.
    /// </summary>
    /// <param name="container">The control (including child controls) to have the refresh events registered.</param>
    /// <param name="register">True to register the events, false to unregister them.</param>
    private void RegisterControlEvents(Control container, bool register)
    {
      // If we are to register the events the do so.
      if (register)
      {
        container.DataBindings.CollectionChanged += new CollectionChangeEventHandler(DataBindings_CollectionChanged);
        container.ControlAdded += new ControlEventHandler(Container_ControlAdded);
        container.ControlRemoved += new ControlEventHandler(Container_ControlRemoved);
      }
      // Else unregister them.
      else
      {
        container.DataBindings.CollectionChanged -= new CollectionChangeEventHandler(DataBindings_CollectionChanged);
        container.ControlAdded -= new ControlEventHandler(Container_ControlAdded);
        container.ControlRemoved -= new ControlEventHandler(Container_ControlRemoved);
      }

      // Reigster the binding complete events for the control.
      RegisterBindingEvents(container, register);

      // Register/unregister the events on all child controls.
      foreach (Control control in container.Controls)
      {
        RegisterControlEvents(control, register);
      }
    }

    /// <summary>
    /// RegisterBindingEvents() registers the binding complete event to all data bindings in control.
    /// </summary>
    /// <param name="control">The control whose binding complete events are to be registered.</param>
    /// <param name="register">True to register the events, false to unregister them.</param>
    private void RegisterBindingEvents(Control control, bool register)
    {
      foreach (Binding binding in control.DataBindings)
      {
        if (register)
        {
          binding.BindingComplete += new BindingCompleteEventHandler(Control_BindingComplete);
        }
        else
        {
          binding.BindingComplete -= new BindingCompleteEventHandler(Control_BindingComplete);
        }
      }
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// DataBindings_CollectionChanged() is the data bindings collection change event for a control.
    /// DataBindings_CollectionChanged() simply updates our binding events hookins correctly based on the collections
    /// current state.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
    {
      switch (e.Action)
      {
        case CollectionChangeAction.Add:
          // To prevent duplicate binding complete events unregister the existing bindings first then re-register
          // them all for the control.
          RegisterBindingEvents(((ControlBindingsCollection)sender).Control, false);
          RegisterBindingEvents(((ControlBindingsCollection)sender).Control, true);
          break;
        case CollectionChangeAction.Remove:
          RegisterBindingEvents(((ControlBindingsCollection)sender).Control, false);
          break;
      }
    }

    /// <summary>
    /// Container_ControlAdded() is the control add event for a control's control collection.
    /// Container_ControlAdded() simply registers the relevant controller events for the new control as well as registering
    /// any required bindings - including those for child controls (based on the controls binding source).
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private void Container_ControlAdded(Object sender, ControlEventArgs e)
    {
      RegisterControlEvents(e.Control, true);
    }

    /// <summary>
    /// Container_ControlRemoved() is the control remove event for a control's control collection.
    /// Container_ControlRemoved() unregisters all events associated with the control - including those for child controls.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private void Container_ControlRemoved(object sender, ControlEventArgs e)
    {
      RegisterControlEvents(e.Control, false);
    }

    /// <summary>
    /// Control_BindingComplete() is a event driven routine triggered when one of the control's bindings has been completed.
    /// Control_BindingComplete() simply validates the result where if the result was a exception then the BindingError
    /// event is raised, else if the binding was a successful data source update and we are to re-read the value on changed then
    /// the binding value is reread into the control.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments.</param>
    private void Control_BindingComplete(object sender, BindingCompleteEventArgs e)
    {
      switch (e.BindingCompleteState)
      {
        case BindingCompleteState.Exception:
          if (BindingError != null)
          {
            BindingError(this, new BindingErrorEventArgs(e.Binding, e.Exception));
          }
          break;
        default:
          if ((e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
                  && e.Binding.DataSource is BindingSource
                  && GetReadValuesOnChange((BindingSource)e.Binding.DataSource))
          {
            e.Binding.ReadValue();
          }
          break;
      }
    }

    #endregion

    #region ISupportInitialize Interface

    private bool _isInitialising = false;

    /// <summary>
    /// BeginInit() is called when the component is starting to be initialised. BeginInit() simply sets the initialisation flag to true.
    /// </summary>
    void ISupportInitialize.BeginInit()
    {
      _isInitialising = true;
    }

    /// <summary>
    /// EndInit() is called when the component has finished being initialised.  EndInt() sets the initialise flag to false then runs
    /// through registering all the different events that the component needs to hook into in Host.
    /// </summary>
    void ISupportInitialize.EndInit()
    {
      _isInitialising = false;
      if (Host != null)
      {
        RegisterControlEvents(Host, true);
      }
    }

    #endregion

  }

  #region Delegates

  /// <summary>
  /// BindingErrorEventHandler delegates is the event handling definition for handling data binding errors that occurred due to exceptions.
  /// </summary>
  /// <param name="sender">The object that triggered the event.</param>
  /// <param name="e">The event arguments.</param>
  public delegate void BindingErrorEventHandler(object sender, BindingErrorEventArgs e);

  #endregion

  #region BindingErrorEventArgs Class

  /// <summary>
  /// BindingErrorEventArgs defines the event arguments for reporting a data binding error due to a exception.
  /// </summary>
  public class BindingErrorEventArgs : EventArgs
  {

    #region Property Fields

    private Exception _exception = null;
    private Binding _binding = null;

    #endregion

    #region Properties

    /// <summary>
    /// Exception gets the exception that caused the binding error.
    /// </summary>
    public Exception Exception
    {
      get { return (_exception); }
    }

    /// <summary>
    /// Binding gets the binding that caused the exception.
    /// </summary>
    public Binding Binding
    {
      get { return (_binding); }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor creates a new BindingErrorEventArgs object instance using the information specified.
    /// </summary>
    /// <param name="binding">The binding that caused th exception.</param>
    /// <param name="exception">The exception that caused the error.</param>
    public BindingErrorEventArgs(Binding binding, Exception exception)
    {
      _binding = binding;
      _exception = exception;
    }

    #endregion

  }

  #endregion
}
