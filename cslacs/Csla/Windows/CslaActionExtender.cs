#region Namespace imports

using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Csla;
using Csla.Core;
using Csla.Validation;

#endregion

namespace Csla.Windows
{
  /// <summary>
  /// Extender control providing automation around
  /// data binding to CSLA .NET business objects.
  /// </summary>
  [ToolboxItem(true)]
  [ProvideProperty("ActionType", typeof(Control))]
  [ProvideProperty("PostSaveAction", typeof(Control))]
  [ProvideProperty("RebindAfterSave", typeof(Control))]
  [ProvideProperty("DisableWhenClean", typeof(Control))]
  [ProvideProperty("CommandName", typeof(Control))]
  public class CslaActionExtender : Component, IExtenderProvider
  {
    #region Constants

    private const string STR_InvalidDataSource = "DataSource must be a BindingSource control instance.";
    private const string STR_InvalidBindingSourceCast = "DataSource does not cast to a BindingSource.";
    private const string STR_InvalidBusinessObjectBaseCast = "The underlying data source does not cast to a CSLA BusinessBase object.";
    private const string STR_ClickingEventDescription = "Event fires just before the attempted action.";
    private const string STR_ClickedEventDescription = "Event fires after a successful action.  When button is set to Save, this event will only fire upon a successful save.  If button is set to Close, this event will never fire.";
    private const string STR_ErrorEncounteredDescription = "Event fires upon encountering any exception during an action.";
    private const string STR_SetForNewEventDescription = "Event fires upon a successful save when the PostSaveAction property is set to AndNew.";
    private const string STR_BusinessObjectInvalidEventDescription = "Event fires when the object is in an invalid state.  Note that this event will work in conjunction with the InvalidateOnWarnings and InvalidateOnInformation properties.";
    private const string STR_HasBrokenRulesEventDescription = "Event fires if there are any broken rules at all, despite severity.";
    private const string STR_ActionTypePropertyDescription = "Gets or sets the type for this button.";
    private const string STR_PostSaveActionTypeDescription = "Gets or sets the action performed after a save (if ActionType is set to Save).";
    private const string STR_DataSourcePropertyDescription = "Gets or sets the data source to which this button is bound for action purposes.";
    private const string STR_RebindAfterSavePropertyDescription = "Determines if the binding source will rebind after business object saves.";
    private const string STR_AutoShowBrokenRulesPropertyDescription = "If True, then the broken rules will be displayed in a message box, should the object be invalid.";
    private const string STR_InvalidateOnWarningsPropertyDescription = "If True, then the business object will be considered invalid if there are any warnings in the broken rules.";
    private const string STR_InvalidateOnInformationPropertyDescription = "If True, then the business object will be considered invalid if there are any information items in the broken rules.";
    private const string STR_DisableWhenCleanPropertyDescription = "If True, then the dirtiness of the underlying business object will cause this button to enable or disable.";
    private const string STR_ObjectSavedEventDescription = "Fires immediately after the underlying object successfully saves.";
    private const string STR_WarnIfCloseOnDirtyPropertyDescription = "If True, then the control (when set to Close mode) will warn the user if the object is currently dirty.";
    private const string STR_DirtyWarningMessagePropertyDescription = "Gets or sets the confirmation message that will display if a Close button is pressed and the object is dirty.";
    private const string STR_CloseConfirmation = "Are you sure you want to close?";
    private const string STR_WarnOnCancelPropertyDescription = "If True, then the Cancel button will warn when pressed and the object is dirty.";
    private const string STR_WarnOnCancelMessagePropertyDescription = "If the WarnOnCancel property is set to True, this is the message to be displayed.";
    private const string STR_DirtyWarningMessagePropertyDefault = "Object is currently in a dirty changed.";
    private const string STR_WarnOnCancelMessagePropertyDefault = "Are you sure you want to revert to the previous values?";
    private const string STR_PostSaveActionTypePropertyDescription = "Gets or sets the action performed after a save (if ActionType is set to Save).";
    private const string STR_CommandNamePropertyDescription = "Gets or sets the name of this command control for unique identification purposes.";
    private const string STR_ObjectSavingEventDescription = "Fires just before a save action is performed.";

    #endregion

    #region Variable defaults

    private static object DataSourceDefault = null;
    private static bool AutoShowBrokenRulesDefault = true;
    private static bool WarnIfCloseOnDirtyDefault = true;
    private static string DirtyWarningMessageDefault = STR_DirtyWarningMessagePropertyDefault;
    private static bool WarnOnCancelDefault = false;
    private static string WarnOnCancelMessageDefault = STR_WarnOnCancelMessagePropertyDefault;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="container">Container for the component.</param>
    public CslaActionExtender(IContainer container)
      : base()
    {
      _container = container;
      container.Add(this);
    }

    #endregion

    #region Member variables

    private Dictionary<Control, CslaActionExtenderProperties> _Sources = 
      new Dictionary<Control, CslaActionExtenderProperties>();

    private object _dataSource = DataSourceDefault;
    private bool _autoShowBrokenRules = AutoShowBrokenRulesDefault;
    private bool _warnIfCloseOnDirty = WarnIfCloseOnDirtyDefault;
    private string _dirtyWarningMessage = DirtyWarningMessageDefault;
    private bool _warnOnCancel = WarnOnCancelDefault;
    private string _warnOnCancelMessage = WarnOnCancelMessageDefault;
    private IContainer _container = null;
    private BindingSourceNode _bindingSourceTree = null;
    private bool _closeForm = false;

    #endregion

    #region IExtenderProvider implementation

    bool IExtenderProvider.CanExtend(object extendee)
    {
      bool ret = false;
      if (extendee is IButtonControl)
        ret = true;
      return ret;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets a reference to the data source object.
    /// </summary>
    [Category("Data")]
    [Description(STR_DataSourcePropertyDescription)]
    [AttributeProvider(typeof(IListSource))]
    public object DataSource
    {
      get { return _dataSource; }
      set
      {
        if (value != null)
        {
          if (value is BindingSource)
            _dataSource = value;
          else
            throw new ArgumentException(STR_InvalidDataSource);
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically
    /// show broken rules.
    /// </summary>
    [Category("Behavior")]
    [Description(STR_AutoShowBrokenRulesPropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool AutoShowBrokenRules
    {
      get { return _autoShowBrokenRules; }
      set { _autoShowBrokenRules = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to warn the
    /// user on close when the object is dirty.
    /// </summary>
    [Category("Behavior")]
    [Description(STR_WarnIfCloseOnDirtyPropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool WarnIfCloseOnDirty
    {
      get { return _warnIfCloseOnDirty; }
      set { _warnIfCloseOnDirty = value; }
    }

    /// <summary>
    /// Gets or sets the message shown to the user
    /// in a close on dirty warning.
    /// </summary>
    [Category("Behavior")]
    [Description(STR_DirtyWarningMessagePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(STR_DirtyWarningMessagePropertyDefault)]
    public string DirtyWarningMessage
    {
      get { return _dirtyWarningMessage; }
      set { _dirtyWarningMessage = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to warn
    /// the user on cancel.
    /// </summary>
    [Category("Behavior")]
    [Description(STR_WarnOnCancelPropertyDescription)]
    [Bindable(true)]
    [DefaultValue(false)]
    public bool WarnOnCancel
    {
      get { return _warnOnCancel; }
      set { _warnOnCancel = value; }
    }

    /// <summary>
    /// Gets or sets the message shown to the user
    /// in a warn on cancel.
    /// </summary>
    [Category("Behavior")]
    [Description(STR_WarnOnCancelMessagePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(STR_WarnOnCancelMessagePropertyDefault)]
    public string WarnOnCancelMessage
    {
      get { return _warnOnCancelMessage; }
      set { _warnOnCancelMessage = value; }
    }

    #endregion

    #region Property accessor methods

    #region ActionType

    /// <summary>
    /// Gets the action type.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <returns></returns>
    [Category("Csla")]
    [Description(STR_ActionTypePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(CslaFormAction.None)]
    public CslaFormAction GetActionType(Control ctl)
    {
      if (_Sources.ContainsKey(ctl))
        return ((CslaActionExtenderProperties)_Sources[ctl]).ActionType;
      else
        return CslaActionExtenderProperties.ActionTypeDefault;
    }

    /// <summary>
    /// Sets the action type.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description(STR_ActionTypePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(CslaFormAction.None)]
    public void SetActionType(Control ctl, CslaFormAction value)
    {
      if (_Sources.ContainsKey(ctl))
        _Sources[ctl].ActionType = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.ActionType = value;
        _Sources.Add(ctl, props);
      }
    }

    #endregion

    #region PostSaveAction

    /// <summary>
    /// Gets the post save action.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <returns></returns>
    [Category("Csla")]
    [Description(STR_PostSaveActionTypePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(PostSaveActionType.None)]
    public PostSaveActionType GetPostSaveAction(Control ctl)
    {
      if (_Sources.ContainsKey(ctl))
        return ((CslaActionExtenderProperties)_Sources[ctl]).PostSaveAction;
      else
        return CslaActionExtenderProperties.PostSaveActionDefault;
    }

    /// <summary>
    /// Sets the post save action.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description(STR_PostSaveActionTypePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(PostSaveActionType.None)]
    public void SetPostSaveAction(Control ctl, PostSaveActionType value)
    {
      if (_Sources.ContainsKey(ctl))
        _Sources[ctl].PostSaveAction = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.PostSaveAction = value;
        _Sources.Add(ctl, props);
      }
    }

    #endregion

    #region RebindAfterSave

    /// <summary>
    /// Gets the rebind after save value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    [Category("Csla")]
    [Description(STR_RebindAfterSavePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool GetRebindAfterSave(Control ctl)
    {
      if (_Sources.ContainsKey(ctl))
        return ((CslaActionExtenderProperties)_Sources[ctl]).RebindAfterSave;
      else
        return CslaActionExtenderProperties.RebindAfterSaveDefault;
    }

    /// <summary>
    /// Sets the rebind after save value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description(STR_RebindAfterSavePropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public void SetRebindAfterSave(Control ctl, bool value)
    {
      if (_Sources.ContainsKey(ctl))
        _Sources[ctl].RebindAfterSave = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.RebindAfterSave = value;
        _Sources.Add(ctl, props);
      }
    }

    #endregion

    #region DisableWhenClean

    /// <summary>
    /// Gets the disable when clean value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    [Category("Csla")]
    [Description(STR_DisableWhenCleanPropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool GetDisableWhenClean(Control ctl)
    {
      if (_Sources.ContainsKey(ctl))
        return ((CslaActionExtenderProperties)_Sources[ctl]).DisableWhenClean;
      else
        return CslaActionExtenderProperties.DisableWhenCleanDefault;
    }

    /// <summary>
    /// Sets the disable when clean value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description(STR_DisableWhenCleanPropertyDescription)]
    [Bindable(true)]
    [DefaultValue(true)]
    public void SetDisableWhenClean(Control ctl, bool value)
    {
      if (_Sources.ContainsKey(ctl))
        _Sources[ctl].DisableWhenClean = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.DisableWhenClean = value;
        _Sources.Add(ctl, props);
      }
    }

    #endregion

    #region CommandName

    /// <summary>
    /// Gets the command name value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    [Category("Csla")]
    [Description(STR_CommandNamePropertyDescription)]
    [Bindable(true)]
    [DefaultValue("")]
    public string GetCommandName(Control ctl)
    {
      if (_Sources.ContainsKey(ctl))
        return ((CslaActionExtenderProperties)_Sources[ctl]).CommandName;
      else
        return CslaActionExtenderProperties.CommandNameDefault;
    }

    /// <summary>
    /// Sets the command name value.
    /// </summary>
    /// <param name="ctl">Reference to control.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description(STR_CommandNamePropertyDescription)]
    [Bindable(true)]
    [DefaultValue("")]
    public void SetCommandName(Control ctl, string value)
    {
      if (_Sources.ContainsKey(ctl))
        _Sources[ctl].CommandName = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.CommandName = value;
        _Sources.Add(ctl, props);
      }
    }

    #endregion

    #endregion

    #region Event declarations

    /// <summary>
    /// Event indicating the user is clicking on the control.
    /// </summary>
    [Category("Csla")]
    [Description(STR_ClickingEventDescription)]
    public event EventHandler<CslaActionCancelEventArgs> Clicking;

    /// <summary>
    /// Event indicating the user clicked on the control.
    /// </summary>
    [Category("Csla")]
    [Description(STR_ClickedEventDescription)]
    public event EventHandler<CslaActionEventArgs> Clicked;

    /// <summary>
    /// Event indicating an error was encountered.
    /// </summary>
    [Category("Csla")]
    [Description(STR_ErrorEncounteredDescription)]
    public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;

    /// <summary>
    /// Event indicating the object is set for new.
    /// </summary>
    [Category("Csla")]
    [Description(STR_SetForNewEventDescription)]
    public event EventHandler<CslaActionEventArgs> SetForNew;

    /// <summary>
    /// Event indicating the business object is in an invalid state.
    /// </summary>
    [Category("Csla")]
    [Description(STR_BusinessObjectInvalidEventDescription)]
    public event EventHandler<CslaActionEventArgs> BusinessObjectInvalid;

    /// <summary>
    /// Event indicating the business object has broken rules.
    /// </summary>
    [Category("Csla")]
    [Description(STR_HasBrokenRulesEventDescription)]
    public event EventHandler<HasBrokenRulesEventArgs> HasBrokenRules;

    /// <summary>
    /// Event indicating that the object is saving.
    /// </summary>
    [Category("Csla")]
    [Description(STR_ObjectSavingEventDescription)]
    public event EventHandler<CslaActionCancelEventArgs> ObjectSaving;

    /// <summary>
    /// Event indicating that the object has been saved.
    /// </summary>
    [Category("Csla")]
    [Description(STR_ObjectSavedEventDescription)]
    public event EventHandler<CslaActionEventArgs> ObjectSaved;

    #endregion

    #region OnEvent methods

    /// <summary>
    /// Raises the Clicking event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnClicking(CslaActionCancelEventArgs e)
    {
      if (Clicking != null)
        Clicking(this, e);
    }

    /// <summary>
    /// Raises the Clicked event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnClicked(CslaActionEventArgs e)
    {
      if (Clicked != null)
        Clicked(this, e);
    }

    /// <summary>
    /// Raises the ErrorEncountered event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnErrorEncountered(ErrorEncounteredEventArgs e)
    {
      if (ErrorEncountered != null)
        ErrorEncountered(this, e);
    }

    /// <summary>
    /// Raises the SetForNew event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnSetForNew(CslaActionEventArgs e)
    {
      if (SetForNew != null)
        SetForNew(this, e);
    }

    /// <summary>
    /// Raises the BusinessObjectInvalid event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnBusinessObjectInvalid(CslaActionEventArgs e)
    {
      if (BusinessObjectInvalid != null)
        BusinessObjectInvalid(this, e);
    }

    /// <summary>
    /// Raises the HasBrokenRules event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnHasBrokenRules(HasBrokenRulesEventArgs e)
    {
      if (HasBrokenRules != null)
        HasBrokenRules(this, e);
    }

    /// <summary>
    /// Raises the ObjectSaving event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnObjectSaving(CslaActionCancelEventArgs e)
    {
      if (ObjectSaving != null)
        ObjectSaving(this, e);
    }

    /// <summary>
    /// Raises the ObjectSaved event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnObjectSaved(CslaActionEventArgs e)
    {
      if (ObjectSaved != null)
        ObjectSaved(this, e);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Resets all action behaviors.
    /// </summary>
    /// <param name="objectToBind">Target object.</param>
    public void ResetActionBehaviors(ISavable objectToBind)
    {
      InitializeControls(true);

      BindingSource rootSource = _dataSource as BindingSource;

      if (rootSource != null)
      {
        rootSource.CurrentItemChanged -= BusinessObject_PropertyChanged;
        rootSource.CurrentItemChanged += BusinessObject_PropertyChanged;
      }

      _bindingSourceTree = BindingSourceHelper.InitializeBindingSourceTree(_container, rootSource);
      _bindingSourceTree.Bind(objectToBind);

    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Method invoked when the target control is clicked.
    /// </summary>
    /// <param name="sender">Object originating action.</param>
    /// <param name="e">Arguments.</param>
    protected void OnClick(object sender, System.EventArgs e)
    {
      Control ctl = (Control)sender;
      CslaActionExtenderProperties props = _Sources[ctl] as CslaActionExtenderProperties;
      if (props.ActionType != CslaFormAction.None)
      {
        try
        {
          bool raiseClicked = true;
          CslaActionCancelEventArgs args = new CslaActionCancelEventArgs(false, props.CommandName);
          OnClicking(args);
          if (!args.Cancel)
          {
            // perform action
            Csla.Core.ISavable savableObject = null;
            Csla.Core.ITrackStatus trackableObject = null;

            BindingSource source = null;
            if (_dataSource != null)
            {
              source = _dataSource as BindingSource;

              if (source != null)
              {
                savableObject = source.DataSource as Csla.Core.ISavable;
                trackableObject = source.DataSource as Csla.Core.ITrackStatus;
              }
              else
                OnErrorEncountered(new ErrorEncounteredEventArgs(
                  props.CommandName, new InvalidCastException(STR_InvalidBindingSourceCast)));

              if (savableObject == null || trackableObject == null)
                OnErrorEncountered(new ErrorEncounteredEventArgs(
                  props.CommandName, new InvalidCastException(STR_InvalidBusinessObjectBaseCast)));
            }

            DialogResult diagResult;

            switch (props.ActionType)
            {
              case CslaFormAction.Save:

                bool okToContinue = true;

                if (savableObject is Csla.Core.BusinessBase)
                {
                  Csla.Core.BusinessBase businessObject = savableObject as Csla.Core.BusinessBase;
                  if (businessObject.BrokenRulesCollection.Count > 0)
                  {
                    HasBrokenRulesEventArgs argsHasBrokenRules = new HasBrokenRulesEventArgs(
                      props.CommandName,
                      businessObject.BrokenRulesCollection.ErrorCount > 0,
                      businessObject.BrokenRulesCollection.WarningCount > 0,
                      businessObject.BrokenRulesCollection.InformationCount > 0,
                      _autoShowBrokenRules);

                    OnHasBrokenRules(argsHasBrokenRules);

                    okToContinue = !argsHasBrokenRules.Cancel;
                    //in case the client changed it
                    _autoShowBrokenRules = argsHasBrokenRules.AutoShowBrokenRules;
                  }
                }

                if (okToContinue)
                {
                  if (savableObject is Csla.Core.BusinessBase)
                  {
                    Csla.Core.BusinessBase businessObject = savableObject as Csla.Core.BusinessBase;
                    if (_autoShowBrokenRules && businessObject.BrokenRulesCollection.Count > 0)
                      MessageBox.Show(businessObject.BrokenRulesCollection.ToString());
                  }

                  bool objectValid = trackableObject.IsValid;

                  if (objectValid)
                  {
                    CslaActionCancelEventArgs savingArgs = new CslaActionCancelEventArgs(
                      false, props.CommandName);
                    OnObjectSaving(savingArgs);

                    if (!args.Cancel)
                    {
                      _bindingSourceTree.Apply();

                      //save
                      Csla.Core.ISavable objectToSave = savableObject;

                      if (Csla.ApplicationContext.AutoCloneOnUpdate)
                        objectToSave = ((ICloneable)savableObject).Clone() as Csla.Core.ISavable;

                      if (objectToSave != null)
                      {
                        Csla.Core.ISavable saveableObject = objectToSave as Csla.Core.ISavable;

                        try
                        {
                          savableObject = savableObject.Save() as Csla.Core.ISavable;

                          OnObjectSaved(new CslaActionEventArgs(props.CommandName));

                          switch (props.PostSaveAction)
                          {
                            case PostSaveActionType.None:

                              if (source != null && props.RebindAfterSave)
                                _bindingSourceTree.Bind(savableObject);

                              break;

                            case PostSaveActionType.AndClose:

                              CloseForm();
                              break;

                            case PostSaveActionType.AndNew:

                              OnSetForNew(new CslaActionEventArgs(props.CommandName));
                              break;
                          }
                        }
                        catch (Exception ex)
                        {
                          OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, new ObjectSaveException(ex)));
                          raiseClicked = false;
                        }
                      }
                      else
                      {
                        // did not find bound object so don't bother raising the Clicked event
                        raiseClicked = false;
                      }

                      _bindingSourceTree.SetEvents(true);
                    }
                  }
                  else
                  {
                    OnBusinessObjectInvalid(new CslaActionEventArgs(props.CommandName));
                    // object not valid or has broken rules set to invalidate it due to this control's properties
                    raiseClicked = false;
                  }
                }
                else
                {
                  // process was canceled from the HasBrokenRules event
                  raiseClicked = false;
                }

                break;

              case CslaFormAction.Cancel:

                diagResult = System.Windows.Forms.DialogResult.Yes;
                if (_warnOnCancel && trackableObject.IsDirty)
                  diagResult = MessageBox.Show(
                    _warnOnCancelMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (diagResult == System.Windows.Forms.DialogResult.Yes)
                  _bindingSourceTree.Cancel(savableObject);

                break;

              case CslaFormAction.Close:

                diagResult = System.Windows.Forms.DialogResult.Yes;
                if (trackableObject.IsDirty || trackableObject.IsNew)
                {
                  if (_warnIfCloseOnDirty)
                    diagResult = MessageBox.Show(
                      _dirtyWarningMessage + Environment.NewLine + STR_CloseConfirmation, "Warning",
                      MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }

                if (diagResult == System.Windows.Forms.DialogResult.Yes)
                {
                  _bindingSourceTree.Close();
                  _closeForm = true;
                }

                break;
            }

            if (raiseClicked)
            {
              if (props.ActionType == CslaFormAction.Save && source != null && props.RebindAfterSave)
              {
                // For some strange reason, this has to be done down here.
                // Putting it in the Select Case AfterSave... does not work.
                _bindingSourceTree.ResetBindings(false);
                InitializeControls(true);
              }

              OnClicked(new CslaActionEventArgs(props.CommandName));
            }
          }

          if (_closeForm)
            CloseForm();
          
        }
        catch (Exception ex)
        {
          OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, ex));
        }
      }

    }

    #endregion

    #region Private methods

    private void ResetControls()
    {
      InitializeControls(false);
    }

    private void InitializeControls(bool initialEnabling)
    {
      // controls will not be enabled until the BusinessObjectPropertyChanged event fires or if it's in an appropriate state now
      List<Control> extendedControls = new List<Control>();
      foreach (KeyValuePair<Control, CslaActionExtenderProperties> pair in _Sources)
      {
        if (pair.Value.ActionType != CslaFormAction.None)
        {
          Control ctl = pair.Key;
          if (initialEnabling)
          {
            ChangeEnabled(ctl, !pair.Value.DisableWhenClean);
            pair.Key.Click -= OnClick;
            pair.Key.Click += OnClick;
          }
          InitializeControl(ctl);
          extendedControls.Add(ctl);
        }
      }
    }

    private void InitializeControl(Control ctl)
    {
      if (!ctl.Enabled)
      {
        Csla.Core.ISavable businessObject = GetBusinessObject();
        if (businessObject != null)
        {
          Csla.Core.ITrackStatus trackableObject = businessObject as ITrackStatus;
          if (trackableObject != null)
            ChangeEnabled(ctl, trackableObject.IsNew || trackableObject.IsDirty || trackableObject.IsDeleted);
        }
      }
    }

    private void ChangeEnabled(Control ctl, bool newEnabled)
    {
      // only do this if it's changed to avoid flicker
      if (ctl.Enabled != newEnabled)
        ctl.Enabled = newEnabled;
    }

    private void CloseForm()
    {
      if (_Sources.Count > 0)
      {
        Dictionary<Control, CslaActionExtenderProperties>.Enumerator enumerator = _Sources.GetEnumerator();
        if (enumerator.MoveNext())
        {
          Control ctl = enumerator.Current.Key;
          Form frm = GetParentForm(ctl);
          if (frm != null)
            frm.Close();
        }
      }
    }

    private Form GetParentForm(Control thisControl)
    {
      Form frm = null;

      if (thisControl.Parent is Form)
        frm = (Form)thisControl.Parent;
      else
        frm = GetParentForm(thisControl.Parent);

      return frm;
    }

    private Csla.Core.ISavable GetBusinessObject()
    {
      Csla.Core.ISavable businessObject = null;
      BindingSource source = _dataSource as BindingSource;
      if (source != null)
        businessObject = source.DataSource as Csla.Core.ISavable;

      return businessObject;
    }

    #endregion

    #region Event methods

    private void BusinessObject_PropertyChanged(object sender, EventArgs e)
    {
      ResetControls();
    }

    #endregion

  }
}
