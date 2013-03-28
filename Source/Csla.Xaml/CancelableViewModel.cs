﻿/*
 * Created by: Велижанин Николай Александрович
 * Created: 28 марта 2013 г.
*/

using System;
using System.Collections.Generic;

using Csla.Reflection;

namespace Csla.Xaml
{
  /// <summary>
  /// ViewModel without multithreading (concurrency) bugs.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class CancelableViewModel<T> : ViewModel<T>
  {
    private CslaOperation<T> _lastOperation;
    private Action<CslaOperation<T>> m_nextOperationExecutor = null;

    /// <summary>
    /// </summary>
    protected CancelableViewModel()
    {
      IsConcurentRefreshesAllowed = true;
    }

    /// <summary>
    /// Allows more than one refresh operations in one time.
    /// Anyway, only the last refresh request will set the Model.
    /// </summary>
    public bool IsConcurentRefreshesAllowed { get; set; }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Static factory method action.</param>
    /// <example>BeginRefresh(BusinessList.BeginGetList)</example>
    /// <example>BeginRefresh(handler => BusinessList.BeginGetList(handler))</example>
    /// <example>BeginRefresh(handler => BusinessList.BeginGetList(id, handler))</example>
    protected new void BeginRefresh(Action<EventHandler<DataPortalResult<T>>> factoryMethod)
    {
      ExecuteOperation(operation => operation.Execute(factoryMethod));
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Name of the static factory method.</param>
    /// <param name="factoryParameters">Factory method parameters.</param>
    protected override void BeginRefresh(string factoryMethod, params object[] factoryParameters)
    {
      ExecuteOperation(operation => operation.Execute(factoryMethod, factoryParameters));
    }

    /// <summary>
    /// Clear operations "queue".
    /// </summary>
    public virtual void Clear()
    {
      m_nextOperationExecutor = null;
      if (_lastOperation != null)
      {
        _lastOperation.Cancel();
      }
      IsBusy = false;
      Model = default(T);
      Error = null;
    }

    private void ExecuteOperation(Action<CslaOperation<T>> operationExecutor)
    {
      try
      {
        if (_lastOperation != null)
          _lastOperation.Cancel();

        if (!IsConcurentRefreshesAllowed && _lastOperation != null)
        {
          m_nextOperationExecutor = operationExecutor;
          return;
        }

        Error = null;
        IsBusy = true;

        _lastOperation = new CslaOperation<T>(QueryCompleted);
        operationExecutor(_lastOperation);
      }
      catch (Exception ex)
      {
        Error = ex;
        IsBusy = false;
      }
    }

    private void QueryCompleted(DataPortalResult<T> result, bool isCanceled)
    {
      _lastOperation = null;

      if (!IsConcurentRefreshesAllowed && m_nextOperationExecutor != null)
      {
        var executor = m_nextOperationExecutor;
        m_nextOperationExecutor = null;
        ExecuteOperation(executor);
        return;
      }

      if (isCanceled) return;

      try
      {
        if (result.Error == null)
        {
          OnRefreshing(result.Object);
          Model = result.Object;
        }
        else
          Error = result.Error;
        OnRefreshed();
      }
      finally
      {
        IsBusy = false;
      }
    }
  }

  internal class CslaOperation<T>
  {
    private readonly Action<DataPortalResult<T>, bool> m_OnCompletedQuery;
    private bool m_IsCanceled;

    public CslaOperation(Action<DataPortalResult<T>, bool> onCompletedQuery)
    {
      if (onCompletedQuery == null) throw new ArgumentNullException("onCompletedQuery");

      m_OnCompletedQuery = onCompletedQuery;
    }

    public void Cancel()
    {
      m_IsCanceled = true;
    }

    public void Execute(Action<EventHandler<DataPortalResult<T>>> factoryMethod)
    {
      EventHandler<DataPortalResult<T>> handler = (sender, result) => m_OnCompletedQuery(result, m_IsCanceled);
      factoryMethod(handler);
    }

    public void Execute(string factoryMethod, params object[] factoryParameters)
    {
      var parameters = new List<object>(factoryParameters) { CreateHandler() };
      MethodCaller.CallFactoryMethod(typeof(T), factoryMethod, parameters.ToArray());
    }

    private Delegate CreateHandler()
    {
      var args = typeof(DataPortalResult<>).MakeGenericType(typeof(T));
      System.Reflection.MethodInfo method = MethodCaller.GetNonPublicMethod(GetType(), "QueryCompleted");
      Delegate handler = Delegate.CreateDelegate(typeof(EventHandler<>).MakeGenericType(args), this, method);
      return handler;
    }

    private void QueryCompleted(object sender, EventArgs e)
    {
      DataPortalResult<T> result = (DataPortalResult<T>)e;
      m_OnCompletedQuery(result, m_IsCanceled);
    }
  }
}