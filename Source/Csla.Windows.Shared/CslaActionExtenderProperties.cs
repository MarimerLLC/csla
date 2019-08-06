//-----------------------------------------------------------------------
// <copyright file="CslaActionExtenderProperties.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Windows
{
  internal class CslaActionExtenderProperties
  {
    #region Defaults

    static internal CslaFormAction ActionTypeDefault = CslaFormAction.None;
    static internal PostSaveActionType PostSaveActionDefault = PostSaveActionType.None;
    static internal bool RebindAfterSaveDefault = true;
    static internal bool DisableWhenCleanDefault = false;
    static internal bool DisableWhenUselessDefault = false;
    static internal string CommandNameDefault = string.Empty;

    #endregion

    #region Member variables

    protected CslaFormAction _actionType = ActionTypeDefault;
    protected PostSaveActionType _postSaveAction = PostSaveActionDefault;
    protected bool _rebindAfterSave = RebindAfterSaveDefault;
    protected bool _disableWhenClean = DisableWhenCleanDefault;
    protected bool _disableWhenUseless = DisableWhenUselessDefault;
    protected string _commandName = CommandNameDefault;

    #endregion

    #region Public properties

    public CslaFormAction ActionType
    {
      get { return _actionType; }
      set { _actionType = value; }
    }

    public PostSaveActionType PostSaveAction
    {
      get { return _postSaveAction; }
      set { _postSaveAction = value; }
    }

    public bool RebindAfterSave
    {
      get { return _rebindAfterSave; }
      set { _rebindAfterSave = value; }
    }

    public bool DisableWhenClean
    {
      get { return _disableWhenClean; }
      set { _disableWhenClean = value; }
    }

    public bool DisableWhenUseless
    {
      get { return _disableWhenUseless; }
      set { _disableWhenUseless = value; }
    }

    public string CommandName
    {
      get { return _commandName; }
      set { _commandName = value; }
    }

    #endregion
  }
}