//-----------------------------------------------------------------------
// <copyright file="BindingSourceRefresh.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>BindingSourceRefresh contains functionality for refreshing the data bound to controls on Host as well as a mechinism for catching data</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
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
  [ProvideProperty("ReadValuesOnChange", typeof(BindingSource))]
  public class BindingSourceRefresh : Component, IExtenderProvider, ISupportInitialize
  {
    #region Fields
    private readonly Dictionary<BindingSource, bool> _sources = new Dictionary<BindingSource, bool>();
    #endregion
    #region Events
    /// <summary>
    /// BindingError event is raised when a data binding error occurs due to a exception.
    /// </summary>
    public event BindingErrorEventHandler BindingError = null;
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
      if (!_isInitialising)
      {
        RegisterControlEvents(source, value);
      }
    }
    #endregion

    #region Properties

    /// <summary>
    /// Not in use - kept for backward  compatibility
    /// </summary>
    [Browsable(false)]
    [DefaultValue(null)]
#if NETSTANDARD2_0 || NET5_0
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
#endif
    public ContainerControl Host { get; set; }

    /// <summary>
    /// Forces the binding to re-read after an exception is thrown when changing the binding value
    /// </summary>
    [Browsable(true)]
    [DefaultValue(false)]
    public bool RefreshOnException { get; set; }

    #endregion

    #region Private Methods
    /// <summary>
    /// RegisterControlEvents() registers all the relevant events for the container control supplied and also to all child controls
    /// in the oontainer control.
    /// </summary>
    /// <param name="container">The control (including child controls) to have the refresh events registered.</param>
    /// <param name="register">True to register the events, false to unregister them.</param>
    private void RegisterControlEvents(ICurrencyManagerProvider container, bool register)
    {
      var currencyManager = container.CurrencyManager;
      // If we are to register the events the do so.
      if (register)
      {
        currencyManager.Bindings.CollectionChanged += Bindings_CollectionChanged;
        currencyManager.Bindings.CollectionChanging += Bindings_CollectionChanging;
      }
      // Else unregister them.
      else
      {
        currencyManager.Bindings.CollectionChanged -= Bindings_CollectionChanged;
        currencyManager.Bindings.CollectionChanging -= Bindings_CollectionChanging;
      }
      // Reigster the binding complete events for the currencymanagers bindings.
      RegisterBindingEvents(currencyManager.Bindings, register);
    }


    /// <summary>
    /// Registers the control events.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="register">if set to <c>true</c> [register].</param>
    private void RegisterBindingEvents(BindingsCollection source, bool register)
    {
      foreach (Binding binding in source)
      {
        RegisterBindingEvent(binding, register);
      }
    }
    /// <summary>
    /// Registers the binding event.
    /// </summary>
    /// <param name="register">if set to <c>true</c> [register].</param>
    /// <param name="binding">The binding.</param>
    private void RegisterBindingEvent(Binding binding, bool register)
    {
      if (register)
      {
        binding.BindingComplete += Control_BindingComplete;
      }
      else
      {
        binding.BindingComplete -= Control_BindingComplete;
      }
    }
    #endregion
    #region Event Methods

    /// <summary>
    /// Handles the CollectionChanging event of the Bindings control.
    ///
    /// Remove event hooks for element or entire list depending on CollectionChangeAction.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.CollectionChangeEventArgs"/> instance containing the event data.</param>
    private void Bindings_CollectionChanging(object sender, CollectionChangeEventArgs e)
    {
      switch (e.Action)
      {
        case CollectionChangeAction.Refresh:
          // remove events for entire list
          RegisterBindingEvents((BindingsCollection)sender, false);
          break;
        case CollectionChangeAction.Add:
          // adding new element -  remove events for element
          RegisterBindingEvent((Binding)e.Element, false);
          break;
        case CollectionChangeAction.Remove:
          // removing element - remove events for element
          RegisterBindingEvent((Binding)e.Element, false);
          break;
      }
    }

    /// <summary>
    /// Handles the CollectionChanged event of the Bindings control.
    ///    
    /// Add event hooks for element or entire list depending on CollectionChangeAction.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.CollectionChangeEventArgs"/> instance containing the event data.</param>
    private void Bindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
    {
      switch (e.Action)
      {
        case CollectionChangeAction.Refresh:
          // refresh entire list  - add event to all items
          RegisterBindingEvents((BindingsCollection)sender, true);
          break;
        case CollectionChangeAction.Add:
          // new element added - add event to element
          RegisterBindingEvent((Binding)e.Element, true);
          break;
        case CollectionChangeAction.Remove:
          // element has been removed - do nothing
          break;
      }
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
          if ((RefreshOnException)
        && e.Binding.DataSource is BindingSource
        && GetReadValuesOnChange((BindingSource)e.Binding.DataSource))
          {
            e.Binding.ReadValue();
          }
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
      foreach (var source in _sources)
      {
        if (source.Value)
          RegisterControlEvents(source.Key, true);
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