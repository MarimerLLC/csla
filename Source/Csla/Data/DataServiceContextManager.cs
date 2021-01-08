#if !NETFX_CORE && !MONO && !(ANDROID || IOS) && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="DataServiceContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse 
  /// an ADO.NET Data Services context object within 
  /// the context of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of context object to use.
  /// </typeparam>
  public class DataServiceContextManager<C> where C : System.Data.Services.Client.DataServiceContext
  {

    private static object _lock = new object();
    private C _context;

    /// <summary>
    /// Gets the DataServiceContext object for the 
    /// specified URI.
    /// </summary>
    /// <param name="path">
    /// URI to the server-side services.
    /// </param>
    public static DataServiceContextManager<C> GetManager(Uri path)
    {

      lock (_lock)
      {
        var contextLabel = GetContextName(path.ToString());
        DataServiceContextManager<C> mgr = null;
        if (ApplicationContext.LocalContext.Contains(contextLabel))
        {
          mgr = (DataServiceContextManager<C>)(ApplicationContext.LocalContext[contextLabel]);
        }
        else
        {
          mgr = new DataServiceContextManager<C>(path);
          ApplicationContext.LocalContext[contextLabel] = mgr;
        }
        return mgr;
      }
    }

    private DataServiceContextManager(Uri path)
    {
      _context = (C)(Reflection.MethodCaller.CreateInstance(typeof(C), path));
    }

    /// <summary>
    /// Gets the DataServiceContext object.
    /// </summary>
    public C DataServiceContext
    {
      get
      {
        return _context;
      }
    }

    private static string GetContextName(string path)
    {
      return "__ctx:" + path;
    }

    /// <summary>
    /// Gets a list of the entities of the
    /// specified type from the context.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity.
    /// </typeparam>
    /// <returns></returns>
    public List<T> GetEntities<T>()
    {
      List<T> returnValue = new List<T>();
      foreach (var oneEntityDescriptor in _context.Entities)
      {
        if (oneEntityDescriptor.Entity is T)
        {
          returnValue.Add((T)oneEntityDescriptor.Entity);
        }
      }
      return returnValue;
    }

    /// <summary>
    /// Gets a list of the entities by key.
    /// </summary>
    /// <typeparam name="T">
    /// Type of entity.
    /// </typeparam>
    /// <param name="keyPropertyName">
    /// Name of the key property.
    /// </param>
    /// <param name="keyPropertyValue">
    /// Key value to match.
    /// </param>
    public T GetEntity<T>(string keyPropertyName, object keyPropertyValue)
    {
      T returnValue = default(T);
      foreach (T oneEntity in GetEntities<T>())
      {
        if (keyPropertyValue.Equals(Csla.Reflection.MethodCaller.CallPropertyGetter(oneEntity, keyPropertyName)))
        {
          returnValue = oneEntity;
          break;
        }
      }
      return returnValue;
    }
  }
}
#endif