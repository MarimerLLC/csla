//-----------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>ConfigurationManager that abstracts underlying configuration</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Csla.Configuration
{
#if PCL46 || PCL259
  /// <summary>
  /// Dummy type for PCL
  /// </summary>
  public class NameValueCollection : System.Collections.IDictionary
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFixedSize => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public bool IsReadOnly => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public ICollection Keys => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public ICollection Values => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public int Count => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public bool IsSynchronized => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public object SyncRoot => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(object key, object value)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(object key)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IDictionaryEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void Remove(object key)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Dummy type for PCL
  /// </summary>
  public class ConnectionStringSettingsCollection : System.Collections.IDictionary
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// 
    /// </summary>
    public bool IsFixedSize => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public bool IsReadOnly => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public ICollection Keys => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public ICollection Values => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public int Count => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public bool IsSynchronized => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public object SyncRoot => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(object key, object value)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(object key)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IDictionaryEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void Remove(object key)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
#endif

  /// <summary>
  /// ConfigurationManager that abstracts underlying configuration
  /// management implementations and infrastructure.
  /// </summary>
  public static class ConfigurationManager
  {
#if NETSTANDARD2_0 || PCL46 || PCL259
    private static NameValueCollection _settings = new NameValueCollection();
#else
    private static NameValueCollection _settings = System.Configuration.ConfigurationManager.AppSettings;
#endif

    /// <summary>
    /// Gets or sets the app settings for the application's default settings.
    /// </summary>
    public static NameValueCollection AppSettings
    {
      get
      {
        return _settings;
      }
      set
      {
        _settings = value;
        ApplicationContext.SettingsChanged();
      }
    }

#if NETSTANDARD2_0 || PCL46 || PCL259
    private static ConnectionStringSettingsCollection _connectionStrings = new ConnectionStringSettingsCollection();
#else
    private static System.Configuration.ConnectionStringSettingsCollection  _connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;
#endif

    /// <summary>
    /// Gets or sets the connection strings from the 
    /// application's default settings.
    /// </summary>
#if NETSTANDARD2_0 || PCL46 || PCL259
    public static ConnectionStringSettingsCollection ConnectionStrings
#else
    public static System.Configuration.ConnectionStringSettingsCollection ConnectionStrings
#endif
    {
      get
      {
        return _connectionStrings;
      }
      set
      {
        _connectionStrings = value;
      }
    }
  }
}