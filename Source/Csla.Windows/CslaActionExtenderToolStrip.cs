//-----------------------------------------------------------------------
// <copyright file="CslaActionExtenderToolStrip.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extender control providing automation around</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Properties;

namespace Csla.Windows
{
  /// <summary>
  /// Extender control (for ToolStripButton only) providing automation around
  /// data binding to CSLA .NET business objects.
  /// </summary>
  [ToolboxItem(true)]
  [ProvideProperty("ActionType", typeof (ToolStripButton))]
  [ProvideProperty("PostSaveAction", typeof (ToolStripButton))]
  [ProvideProperty("RebindAfterSave", typeof (ToolStripButton))]
  [ProvideProperty("DisableWhenClean", typeof (ToolStripButton))]
  [ProvideProperty("DisableWhenUseless", typeof (ToolStripButton))]
  [ProvideProperty("CommandName", typeof (ToolStripButton))]
  public class CslaActionExtenderToolStrip : Component, IExtenderProvider
  {
    #region Constructors

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="container">Container for the component.</param>
    public CslaActionExtenderToolStrip(IContainer container)
    {
      _container = container;
      container.Add(this);
    }

    #endregion

    #region Member variables

    private Dictionary<ToolStripButton, CslaActionExtenderProperties> _sources =
      new Dictionary<ToolStripButton, CslaActionExtenderProperties>();

    private object _dataSource = null;
    private bool _autoShowBrokenRules = true;
    private bool _warnIfCloseOnDirty = true;
    private string _dirtyWarningMessage = Resources.ActionExtenderDirtyWarningMessagePropertyDefault;
    private bool _warnOnCancel = false;
    private string _warnOnCancelMessage = Resources.ActionExtenderWarnOnCancelMessagePropertyDefault;
    private string _objectIsValidMessage = Resources.ActionExtenderObjectIsValidMessagePropertyDefault;
    private IContainer _container = null;
    private BindingSourceNode _bindingSourceTree = null;
    private bool _closeForm = false;

    #endregion

    #region IExtenderProvider implementation

    bool IExtenderProvider.CanExtend(object extendee)
    {
      return extendee is ToolStripButton;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets a reference to the data source object.
    /// </summary>
    [Category("Data")]
    [Description("Gets or sets the data source to which this button is bound for action purposes.")]
    [AttributeProvider(typeof (IListSource))]
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
            throw new ArgumentException(Resources.ActionExtenderSourceMustBeBindingSource);
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically
    /// show broken rules.
    /// </summary>
    [Category("Behavior")]
    [Description("If True, then the broken rules will be displayed in a message box, should the object be invalid.")]
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
    [Description("If True, then the control (when set to Close mode) will warn the user if the object is currently dirty.")]
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
    [Description("Gets or sets the confirmation message that will display if a Close button is pressed and the object is dirty.")]
    [Bindable(true)]
    [DefaultValue("Object is currently in a dirty changed.")]
    [Localizable(true)]
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
    [Description("If True, then the Cancel button will warn when pressed and the object is dirty.")]
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
    [Description("If the WarnOnCancel property is set to True, this is the message to be displayed.")]
    [Bindable(true)]
    [DefaultValue("Are you sure you want to revert to the previous values?")]
    [Localizable(true)]
    public string WarnOnCancelMessage
    {
      get { return _warnOnCancelMessage; }
      set { _warnOnCancelMessage = value; }
    }

    /// <summary>
    /// Gets or sets the message shown to the user when a button with a 
    /// Validate ActionType is pressed when the object is valid.
    /// </summary>
    [Category("Behavior")]
    [Description("When a button with a Validate ActionType is pressed when the object is valid, this is the message to be displayed.")]
    [Bindable(true)]
    [DefaultValue("Object is valid.")]
    [Localizable(true)]
    public string ObjectIsValidMessage
    {
      get { return _objectIsValidMessage; }
      set { _objectIsValidMessage = value; }
    }

    #endregion

    #region Property accessor methods

    #region ActionType

    /// <summary>
    /// Gets the action type.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <returns></returns>
    [Category("Csla")]
    [Description("Gets or sets the action type for this button.")]
    [Bindable(true)]
    [DefaultValue(CslaFormAction.None)]
    public CslaFormAction GetActionType(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].ActionType;

      return CslaActionExtenderProperties.ActionTypeDefault;
    }

    /// <summary>
    /// Sets the action type.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("Gets or sets the action type for this button.")]
    [Bindable(true)]
    [DefaultValue(CslaFormAction.None)]
    public void SetActionType(ToolStripButton ctl, CslaFormAction value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].ActionType = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.ActionType = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #region PostSaveAction

    /// <summary>
    /// Gets the post save action.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <returns></returns>
    [Category("Csla")]
    [Description("Gets or sets the action performed after a save (if ActionType is set to Save).")]
    [Bindable(true)]
    [DefaultValue(PostSaveActionType.None)]
    public PostSaveActionType GetPostSaveAction(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].PostSaveAction;

      return CslaActionExtenderProperties.PostSaveActionDefault;
    }

    /// <summary>
    /// Sets the post save action.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("Gets or sets the action performed after a save (if ActionType is set to Save).")]
    [Bindable(true)]
    [DefaultValue(PostSaveActionType.None)]
    public void SetPostSaveAction(ToolStripButton ctl, PostSaveActionType value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].PostSaveAction = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.PostSaveAction = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #region RebindAfterSave

    /// <summary>
    /// Gets the rebind after save value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    [Category("Csla")]
    [Description("Determines if the binding source will rebind after business object saves.")]
    [Bindable(true)]
    [DefaultValue(true)]
    public bool GetRebindAfterSave(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].RebindAfterSave;

      return CslaActionExtenderProperties.RebindAfterSaveDefault;
    }

    /// <summary>
    /// Sets the rebind after save value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("Determines if the binding source will rebind after business object saves.")]
    [Bindable(true)]
    [DefaultValue(true)]
    public void SetRebindAfterSave(ToolStripButton ctl, bool value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].RebindAfterSave = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.RebindAfterSave = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #region DisableWhenClean

    /// <summary>
    /// Gets the disable when clean value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    [Category("Csla")]
    [Description("If True, then the dirtiness of the underlying business object will cause this button to be enabled or disabled.")]
    [Bindable(true)]
    [DefaultValue(false)]
    [Obsolete("Use instead DisableWhenUseless")]
    [Browsable(false)]
    public bool GetDisableWhenClean(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].DisableWhenClean;

      return CslaActionExtenderProperties.DisableWhenCleanDefault;
    }

    /// <summary>
    /// Sets the disable when clean value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("If True, then the dirtiness of the underlying business object will cause this button to be enabled or disabled.")]
    [Bindable(true)]
    [DefaultValue(true)]
    [Obsolete("Use instead DisableWhenUseless")]
    [Browsable(false)]
    public void SetDisableWhenClean(ToolStripButton ctl, bool value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].DisableWhenClean = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.DisableWhenClean = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #region DisableWhenUseless

    /// <summary>
    /// Gets the disable when useless value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    [Category("Csla")]
    [Description("If True, then the status of the underlying business object will cause this button to be enabled or disabled.")]
    [Bindable(true)]
    [DefaultValue(false)]
    public bool GetDisableWhenUseless(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].DisableWhenUseless;

      return CslaActionExtenderProperties.DisableWhenUselessDefault;
    }

    /// <summary>
    /// Sets the disable when useless value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("If True, then the status of the underlying business object will cause this button to be enabled or disabled.")]
    [Bindable(true)]
    [DefaultValue(true)]
    public void SetDisableWhenUseless(ToolStripButton ctl, bool value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].DisableWhenUseless = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.DisableWhenUseless = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #region CommandName

    /// <summary>
    /// Gets the command name value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    [Category("Csla")]
    [Description("Gets or sets the name of this command control for unique identification purposes.")]
    [Bindable(true)]
    [DefaultValue("")]
    public string GetCommandName(ToolStripButton ctl)
    {
      if (_sources.ContainsKey(ctl))
        return _sources[ctl].CommandName;

      return CslaActionExtenderProperties.CommandNameDefault;
    }

    /// <summary>
    /// Sets the command name value.
    /// </summary>
    /// <param name="ctl">Reference to ToolStripButton.</param>
    /// <param name="value">Value for property.</param>
    [Category("Csla")]
    [Description("Gets or sets the name of this command control for unique identification purposes.")]
    [Bindable(true)]
    [DefaultValue("")]
    public void SetCommandName(ToolStripButton ctl, string value)
    {
      if (_sources.ContainsKey(ctl))
        _sources[ctl].CommandName = value;
      else
      {
        CslaActionExtenderProperties props = new CslaActionExtenderProperties();
        props.CommandName = value;
        _sources.Add(ctl, props);
      }
    }

    #endregion

    #endregion

    #region Event declarations

    /// <summary>
    /// Event indicating the user is clicking on the ToolStripButton.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires just before the attempted action.")]
    public event EventHandler<CslaActionCancelEventArgs> Clicking;

    /// <summary>
    /// Event indicating the user clicked on the ToolStripButton.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires after a successful action.  When button is set to Save, this event will only fire upon a successful save.  If button is set to Close, this event will never fire.")]
    public event EventHandler<CslaActionEventArgs> Clicked;

    /// <summary>
    /// Event indicating an error was encountered.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires upon encountering any exception during an action.")]
    public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;

    /// <summary>
    /// Event indicating the object is set for new.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires upon a successful save when the PostSaveAction property is set to AndNew.")]
    public event EventHandler<CslaActionEventArgs> SetForNew;

    /// <summary>
    /// Event indicating the business object is in an invalid state.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires when the object is in an invalid state.  Note that this event will work in conjunction with the InvalidateOnWarnings and InvalidateOnInformation properties.")]
    public event EventHandler<CslaActionEventArgs> BusinessObjectInvalid;

    /// <summary>
    /// Event indicating the business object has broken rules.
    /// </summary>
    [Category("Csla")]
    [Description("Event fires if there are any broken rules at all, despite severity.")]
    public event EventHandler<HasBrokenRulesEventArgs> HasBrokenRules;

    /// <summary>
    /// Event indicating that the object is saving.
    /// </summary>
    [Category("Csla")]
    [Description("Fires just before a save action is performed.")]
    public event EventHandler<CslaActionCancelEventArgs> ObjectSaving;

    /// <summary>
    /// Event indicating that the object has been saved.
    /// </summary>
    [Category("Csla")]
    [Description("Fires immediately after the underlying object successfully saves.")]
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
        AddEventHooks(objectToBind);
      }

      _bindingSourceTree = BindingSourceHelper.InitializeBindingSourceTree(_container, rootSource);
      _bindingSourceTree.Bind(objectToBind);
    }

    private void AddEventHooks(ISavable objectToBind)
    {
      // make sure to not attach many times
      RemoveEventHooks(objectToBind);

      INotifyPropertyChanged propChangedObjParent = objectToBind as INotifyPropertyChanged;
      if (propChangedObjParent != null)
      {
        propChangedObjParent.PropertyChanged += propChangedObj_PropertyChanged;
      }

      INotifyChildChanged propChangedObjChild = objectToBind as INotifyChildChanged;
      if (propChangedObjChild != null)
      {
        propChangedObjChild.ChildChanged += propChangedObj_ChildChanged;
      }
    }

    private void RemoveEventHooks(ISavable objectToBind)
    {
      INotifyPropertyChanged propChangedObjParent = objectToBind as INotifyPropertyChanged;
      if (propChangedObjParent != null)
      {
        propChangedObjParent.PropertyChanged -= propChangedObj_PropertyChanged;
      }

      INotifyChildChanged propChangedObjChild = objectToBind as INotifyChildChanged;
      if (propChangedObjChild != null)
      {
        propChangedObjChild.ChildChanged -= propChangedObj_ChildChanged;
      }
    }

    private void propChangedObj_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      ResetControls();
    }

    private void propChangedObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ResetControls();
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Method invoked when the target control is clicked.
    /// </summary>
    /// <param name="sender">Object originating action.</param>
    /// <param name="e">Arguments.</param>
    protected void OnClick(object sender, EventArgs e)
    {
      ToolStripButton ctl = (ToolStripButton) sender;
      CslaActionExtenderProperties props = _sources[ctl];
      if (props.ActionType != CslaFormAction.None)
      {
        try
        {
          bool raiseClicked = true;
          CslaActionCancelEventArgs args = new CslaActionCancelEventArgs(false, props.CommandName);
          OnClicking(args);
          if (!args.Cancel)
          {
            ISavable savableObject = null;
            ITrackStatus trackableObject = null;
            BindingSource source = null;

            var sourceObjectError = false;
            if (_dataSource != null)
            {
              source = _dataSource as BindingSource;

              if (source != null)
              {
                savableObject = source.DataSource as ISavable;
                trackableObject = source.DataSource as ITrackStatus;
              }
              else
              {
                OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, new InvalidCastException(Resources.ActionExtenderInvalidBindingSourceCast)));
                sourceObjectError = true;
              }

              if (savableObject == null || trackableObject == null)
              {
                OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, new InvalidCastException(Resources.ActionExtenderInvalidBusinessObjectBaseCast)));
                sourceObjectError = true;
              }
            }

            if (!sourceObjectError)
            {
              DialogResult diagResult;

              switch (props.ActionType)
              {
                case CslaFormAction.Save:
                  raiseClicked = ExecuteSaveAction(savableObject, trackableObject, props);
                  break;
                // case CslaFormAction.Save

                case CslaFormAction.Cancel:

                  diagResult = DialogResult.Yes;
                  if (_warnOnCancel && trackableObject.IsDirty)
                    diagResult = MessageBox.Show(
                      _warnOnCancelMessage, Resources.Warning,
                      MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                  if (diagResult == DialogResult.Yes)
                    _bindingSourceTree.Cancel(savableObject);

                  break;
                // case CslaFormAction.Cancel

                case CslaFormAction.Close:

                  diagResult = DialogResult.Yes;
                  if (trackableObject.IsDirty || trackableObject.IsNew)
                  {
                    if (_warnIfCloseOnDirty)
                      diagResult = MessageBox.Show(
                        _dirtyWarningMessage + Environment.NewLine + Resources.ActionExtenderCloseConfirmation,
                        Resources.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                  }

                  if (diagResult == DialogResult.Yes)
                  {
                    _bindingSourceTree.Close();
                    _closeForm = true;
                  }

                  break;
                // case CslaFormAction.Close

                case CslaFormAction.Validate:

                  if (savableObject is BusinessBase)
                  {
                    BusinessBase businessObject = savableObject as BusinessBase;
                    if (!businessObject.IsValid)
                    {
                      string brokenRules = string.Empty;
                      foreach (var brokenRule in businessObject.GetBrokenRules())
                      {
                        var lambdaBrokenRule = brokenRule;
                        var friendlyName =
                          PropertyInfoManager.GetRegisteredProperties(businessObject.GetType()).Find(
                            c => c.Name == lambdaBrokenRule.Property).FriendlyName;
                        brokenRules += string.Format("{0}: {1}{2}", friendlyName, brokenRule, Environment.NewLine);
                      }
                      MessageBox.Show(brokenRules, Resources.ActionExtenderErrorCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                      MessageBox.Show(_objectIsValidMessage, Resources.ActionExtenderInformationCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                  }

                  break;
                //case CslaFormAction.Validate

              } // switch (props.ActionType)

              // raiseClicked is true if
              // ActionType == CslaFormAction.Save and everything is ok
              if (raiseClicked)
              {
                if (props.ActionType == CslaFormAction.Save && source != null)
                {
                  if (props.RebindAfterSave)
                  {
                    // For some strange reason, this has to be done down here.
                    // Putting it in the Select Case AfterSave... does not work.
                    _bindingSourceTree.ResetBindings(false);
                    InitializeControls(true);
                  }
                }
                else
                {
                  if (props.ActionType == CslaFormAction.Cancel)
                    InitializeControls(true);
                }

                OnClicked(new CslaActionEventArgs(props.CommandName));
              }

            } // if (!sourceObjectError)

          } // if (!args.Cancel)

          if (_closeForm)
            CloseForm();
        }
        catch (Exception ex)
        {
          OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, ex));
        }
      } // if (props.ActionType != CslaFormAction.None)
    }

    #endregion

    #region Private methods

    private bool ExecuteSaveAction(ISavable savableObject, ITrackStatus trackableObject, CslaActionExtenderProperties props)
    {
      var result = true;
      bool okToContinue = true;

      BusinessBase businessObject = null;
      bool savableObjectIsBusinessBase = savableObject is BusinessBase;
      if (savableObjectIsBusinessBase)
        businessObject = savableObject as BusinessBase;

      if (savableObjectIsBusinessBase)
      {
        if (!businessObject.IsValid)
        {
          HasBrokenRulesEventArgs argsHasBrokenRules = new HasBrokenRulesEventArgs(
            props.CommandName,
            businessObject.GetBrokenRules().ErrorCount > 0,
            businessObject.GetBrokenRules().WarningCount > 0,
            businessObject.GetBrokenRules().InformationCount > 0,
            _autoShowBrokenRules);

          OnHasBrokenRules(argsHasBrokenRules);

          okToContinue = !argsHasBrokenRules.Cancel;
          //in case the client changed it
          _autoShowBrokenRules = argsHasBrokenRules.AutoShowBrokenRules;
        }
      }

      if (okToContinue)
      {
        if (savableObjectIsBusinessBase)
        {
          if (_autoShowBrokenRules && !businessObject.IsValid)
          {
            string brokenRules = string.Empty;
            foreach (var brokenRule in businessObject.GetBrokenRules())
            {
              var lambdaBrokenRule = brokenRule;
              var friendlyName =
                PropertyInfoManager.GetRegisteredProperties(businessObject.GetType()).Find(
                  c => c.Name == lambdaBrokenRule.Property).FriendlyName;
              brokenRules += string.Format("{0}: {1}{2}", friendlyName, brokenRule, Environment.NewLine);
            }
            MessageBox.Show(brokenRules, Resources.ActionExtenderErrorCaption,
              MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }

        if (trackableObject.IsValid)
        {
          CslaActionCancelEventArgs savingArgs = new CslaActionCancelEventArgs(false, props.CommandName);
          OnObjectSaving(savingArgs);

          if (!savingArgs.Cancel)
          {
            _bindingSourceTree.Apply();
            ISavable objectToSave;

            if (Csla.ApplicationContext.AutoCloneOnUpdate == false)
              objectToSave = ((ICloneable)savableObject).Clone() as ISavable;// if not AutoClone, clone manually
            else
              objectToSave = savableObject;

            if (objectToSave != null)
            {
              try
              {
                RemoveEventHooks(savableObject);
                savableObject = savableObject.Save() as ISavable;

                OnObjectSaved(new CslaActionEventArgs(props.CommandName));

                switch (props.PostSaveAction)
                {
                  case PostSaveActionType.None:

                    if (props.RebindAfterSave)
                    {
                      _bindingSourceTree.Bind(savableObject);
                      AddEventHooks(savableObject);
                    }
                    break;

                  case PostSaveActionType.AndClose:

                    CloseForm();
                    break;

                  case PostSaveActionType.AndNew:

                    OnSetForNew(new CslaActionEventArgs(props.CommandName));
                    AddEventHooks(savableObject);
                    break;
                }
              }
              catch (Exception ex)
              {
                _bindingSourceTree.Bind(objectToSave);
                AddEventHooks(objectToSave);
                OnErrorEncountered(new ErrorEncounteredEventArgs(props.CommandName, new ObjectSaveException(ex)));
                // there was some problem
                result = false;
              }
            }
            else
            {
              // did not find bound object so don't bother raising the Clicked event
              result = false;
            }

            _bindingSourceTree.SetEvents(true);
          }
        }
        else
        {
          OnBusinessObjectInvalid(new CslaActionEventArgs(props.CommandName));
          // object not valid or has broken rules set to invalidate it due to this control's properties
          result = false;
        }
      }
      else
      {
        // process was canceled from the HasBrokenRules event (okToContinue = false)
        result = false;
      }

      return result;
    }

    private void ResetControls()
    {
      InitializeControls(false);
    }

    private void InitializeControls(bool initialEnabling)
    {
      // controls will not be enabled until the BusinessObjectPropertyChanged event fires or if it's in an appropriate state now
      List<ToolStripButton> extendedControls = new List<ToolStripButton>();
      foreach (KeyValuePair<ToolStripButton, CslaActionExtenderProperties> pair in _sources)
      {
        if (pair.Value.ActionType != CslaFormAction.None)
        {
          ToolStripButton ctl = pair.Key;
          if (initialEnabling)
          {
            if (pair.Value.DisableWhenUseless || pair.Value.DisableWhenClean)
              ChangeEnabled(ctl, !(pair.Value.DisableWhenUseless || pair.Value.DisableWhenClean));
            pair.Key.Click -= OnClick;
            pair.Key.Click += OnClick;
          }
          InitializeControl(ctl, pair);
          extendedControls.Add(ctl);
        }
      }
    }

    private void InitializeControl(ToolStripButton ctl, KeyValuePair<ToolStripButton, CslaActionExtenderProperties> pair)
    {
      if (pair.Value.DisableWhenUseless || (pair.Value.DisableWhenClean && !ctl.Enabled))
      {
        ISavable businessObject = GetBusinessObject();
        if (businessObject != null)
        {
          ITrackStatus trackableObject = businessObject as ITrackStatus;
          if (trackableObject != null)
          {
            if (pair.Value.ActionType == CslaFormAction.Cancel || pair.Value.DisableWhenClean)
              ChangeEnabled(ctl, trackableObject.IsNew || trackableObject.IsDirty || trackableObject.IsDeleted);
            if (pair.Value.ActionType == CslaFormAction.Save)
              ChangeEnabled(ctl, (trackableObject.IsNew || trackableObject.IsDirty || trackableObject.IsDeleted)
                && trackableObject.IsValid);
          }
        }
      }
    }

    private void ChangeEnabled(ToolStripButton ctl, bool newEnabled)
    {
      // only do this if it's changed to avoid flicker
      if (ctl.Enabled != newEnabled)
        ctl.Enabled = newEnabled;
    }

    private void CloseForm()
    {
      if (_sources.Count > 0)
      {
        Dictionary<ToolStripButton, CslaActionExtenderProperties>.Enumerator enumerator = _sources.GetEnumerator();
        if (enumerator.MoveNext())
        {
          ToolStripButton ctl = enumerator.Current.Key;
          Form frm = GetParentForm(ctl);
          if (frm != null)
            frm.Close();
        }
      }
    }

    private Form GetParentForm(ToolStripButton thisToolStripButton)
    {
      return GetParentForm(thisToolStripButton.GetCurrentParent());
    }

    private Form GetParentForm(Control thisControl)
    {
      Form frm;

      if (thisControl.Parent is Form)
        frm = (Form) thisControl.Parent;
      else
        frm = GetParentForm(thisControl.Parent);

      return frm;
    }

    private ISavable GetBusinessObject()
    {
      ISavable businessObject = null;
      BindingSource source = _dataSource as BindingSource;
      if (source != null)
        businessObject = source.DataSource as ISavable;

      return businessObject;
    }

    #endregion
  }
}
