using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable list class.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the items contained in the list.
  /// </typeparam>
  [Serializable]
  public class MobileList<T> : BindingList<T>, IMobileObject
  {
    #region IMobileObject Members

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    { 
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info);
    }

    protected virtual void OnGetState(SerializationInfo info)
    {
      info.AddValue("Csla.Core.MobileList.AllowEdit", AllowEdit);
      info.AddValue("Csla.Core.MobileList.AllowNew", AllowNew);
      info.AddValue("Csla.Core.MobileList.AllowRemove", AllowRemove);
      info.AddValue("Csla.Core.MobileList.RaiseListChangedEvents", RaiseListChangedEvents);
      info.AddValue("Csla.Core.MobileList.SupportsChangeNotificationCore", SupportsChangeNotificationCore); 
    }

    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter) 
    {
      if (!typeof(IMobileObject).IsAssignableFrom(typeof(T)))
        throw new NotSupportedException(Resources.CannotSerializeCollectionsNotOfIMobileObject);
      
      List<int> references = new List<int>();
      for (int x = 0; x < this.Count; x++)
      {
        T child = this[x];
        if (child != null)
        {
          SerializationInfo childInfo = formatter.SerializeObject(child);
          references.Add(childInfo.ReferenceId);
        }
      }
      if (references.Count > 0)
        info.AddValue("$list", references);
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    protected virtual void OnSetState(SerializationInfo info)
    {
      AllowEdit = info.GetValue<bool>("Csla.Core.MobileList.AllowEdit");
      AllowNew = info.GetValue<bool>("Csla.Core.MobileList.AllowNew");
      AllowRemove = info.GetValue<bool>("Csla.Core.MobileList.AllowRemove");
      RaiseListChangedEvents = info.GetValue<bool>("Csla.Core.MobileList.RaiseListChangedEvents");
      SupportsChangeNotificationCore = info.GetValue<bool>("Csla.Core.MobileList.SupportsChangeNotificationCore");
    }

    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (!typeof(IMobileObject).IsAssignableFrom(typeof(T)))
        throw new NotSupportedException("Cannot deserialize collections not of type IMobileObject");

      if (info.Values.ContainsKey("$list"))
      {      
        List<int> references = (List<int>)info.Values["$list"].Value;
        foreach (int reference in references)
        {
          T child = (T)formatter.GetObject(reference);
          this.Add(child);
        }
      }
    }

    #endregion
  }
}
