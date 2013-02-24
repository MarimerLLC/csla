﻿//-----------------------------------------------------------------------
// <copyright file="SetAppSettingValueCmd.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
#if SILVERLIGHT
using Csla.DataPortalClient;
using Csla.Serialization;
#else
using System.Configuration;
using System.Reflection;

#endif


namespace  Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class SetAppSettingValueCmd : Csla.CommandBase<SetAppSettingValueCmd>
  {
    public static readonly PropertyInfo<string> AppSettingsKeyProperty = RegisterProperty(
      typeof(SetAppSettingValueCmd),
      new PropertyInfo<string>("AppSettingsKey"));

    public static readonly PropertyInfo<string> AppSettingsValueProperty = RegisterProperty(
      typeof(SetAppSettingValueCmd),
      new PropertyInfo<string>("AppSettingsValue"));

    public string AppSettingKey
    {
      get { return ReadProperty(AppSettingsKeyProperty); }
      protected set { LoadProperty(AppSettingsKeyProperty, value); }
    }

    public string AppSettingValue
    {
      get { return ReadProperty(AppSettingsValueProperty); }
      protected set { LoadProperty(AppSettingsValueProperty, value); }
    }

    public SetAppSettingValueCmd(string appSettingKey, string appSettingValue)
    {
      AppSettingKey = appSettingKey;
      AppSettingValue = appSettingValue;
    }

#if SILVERLIGHT
    public SetAppSettingValueCmd(){}
#else
    protected SetAppSettingValueCmd() { }
#endif

    public static void ExecuteCommand(string appSettingKey, string appSettingValue, EventHandler<DataPortalResult<SetAppSettingValueCmd>> handler)
    {
      var command = new SetAppSettingValueCmd(appSettingKey, appSettingValue);
      Csla.DataPortal.BeginExecute<SetAppSettingValueCmd>(command, handler);
    }

#if !SILVERLIGHT
    protected override void DataPortal_Execute()
    {
      //As the value of the _authorizer is loaded from App.Config we consider it as variable that is
      //set once per AppDomain - it had initial value of null and once it is set to a certain type its
      //value does not change.  However for unit tests we want that value to change.  Therefore we need 
      //to be able to reset it back to _null prior to executing the test
      typeof(Csla.Server.DataPortal)
        .GetField("_authorizer", BindingFlags.NonPublic | BindingFlags.Static)
        .SetValue(null, null);
      ConfigurationManager.AppSettings[AppSettingKey] = AppSettingValue;
    }
#else
    protected override void DataPortal_Execute()
    {
    }
#endif
  }
}