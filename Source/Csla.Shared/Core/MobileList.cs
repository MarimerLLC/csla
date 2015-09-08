//-----------------------------------------------------------------------
// <copyright file="MobileList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a list that is serializable using</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization.Mobile;
using serialization = System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using Csla.Reflection;

namespace Csla.Core
{
  /// <summary>
  /// Implements a list that is serializable using
  /// the MobileFormatter.
  /// </summary>
  /// <typeparam name="T">
  /// Type of object contained in the list.
  /// </typeparam>
  [Serializable]
  public class MobileList<T> : List<T>, IMobileObject
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public MobileList() : base() { }
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="capacity">Capacity of the list.</param>
    public MobileList(int capacity) : base(capacity) { }
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="collection">Data to add to list.</param>
    public MobileList(IEnumerable<T> collection) : base(collection) { }

    #region IMobileObject Members

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info);
    }

    /// <summary>
    /// Override this method to add extra field values to
    /// the serialization stream.
    /// </summary>
    /// <param name="info">Object containing field values.</param>
    protected virtual void OnGetState(SerializationInfo info)
    {
    }

    /// <summary>
    /// Override this method to manually serialize child objects
    /// contained within the current object.
    /// </summary>
    /// <param name="info">Object containing serialized values.</param>
    /// <param name="formatter">Reference to the current MobileFormatter.</param>
    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      bool mobileChildren = typeof(IMobileObject).IsAssignableFrom(typeof(T));
      if (mobileChildren)
      {
        List<int> references = new List<int>();
        foreach (IMobileObject child in this)
        {
          SerializationInfo childInfo = formatter.SerializeObject(child);
          references.Add(childInfo.ReferenceId);
        }
        if (references.Count > 0)
          info.AddValue("$list", references);
      }
      else
      {
        using (MemoryStream stream = new MemoryStream())
        {
          serialization.DataContractSerializer serializer = new serialization.DataContractSerializer(GetType());
          serializer.WriteObject(stream, this);
          stream.Flush();
          info.AddValue("$list", stream.ToArray());
        }
      }
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve extra field values to
    /// the serialization stream.
    /// </summary>
    /// <param name="info">Object containing field values.</param>
    protected virtual void OnSetState(SerializationInfo info) { }

    /// <summary>
    /// Override this method to manually deserialize child objects
    /// from data in the serialization stream.
    /// </summary>
    /// <param name="info">Object containing serialized values.</param>
    /// <param name="formatter">Reference to the current MobileFormatter.</param>
    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Values.ContainsKey("$list"))
      {
        bool mobileChildren = typeof(IMobileObject).IsAssignableFrom(typeof(T));
        if (mobileChildren)
        {
          List<int> references = (List<int>)info.Values["$list"].Value;
          foreach (int reference in references)
          {
            T child = (T)formatter.GetObject(reference);
            this.Add(child);
          }
        }
        else
        {
          byte[] buffer = (byte[])info.Values["$list"].Value;
          using (MemoryStream stream = new MemoryStream(buffer))
          {
            serialization.DataContractSerializer dcs = new serialization.DataContractSerializer(GetType());
            MobileList<T> list = (MobileList<T>)dcs.ReadObject(stream);
            AddRange(list);
          }
        }
      }
    }

    #endregion
  }
}