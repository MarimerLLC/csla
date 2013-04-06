using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Internal;

namespace System
{
  /// <summary>
  /// Reimplemented System.Lazy for Windows Phone
  /// </summary>
  /// <typeparam name="T"></typeparam>
  internal class Lazy<T>
  {
    private T _value = default(T);
    private volatile bool _isValueCreated = false;
    private Func<T> _valueFactory = null;
    private object _lock;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
    /// </summary>
    public Lazy()
      : this(Activator.CreateInstance<T>)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
    public Lazy(bool isThreadSafe)
      : this(Activator.CreateInstance<T>, isThreadSafe)
    {
    }

    public Lazy(Func<T> valueFactory) :
      this(valueFactory, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lazy&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="isThreadSafe">if set to <c>true</c> [is thread safe].</param>
    public Lazy(Func<T> valueFactory, bool isThreadSafe)
    {
      if (valueFactory == null) 
        throw new ArgumentNullException("valueFactory");
      if (isThreadSafe)
      {
        this._lock = new object();
      }

      this._valueFactory = valueFactory;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is initilized and contains a value.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is value created; otherwise, <c>false</c>.
    /// </value>
    public bool IsValueCreated
    {
      get { return _isValueCreated; }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public T Value
    {
      get
      {
        if (!this._isValueCreated)
        {
          if (this._lock != null)
          {
            Monitor.Enter(this._lock);
          }

          try
          {
            T value = this._valueFactory.Invoke();
            this._valueFactory = null;
            Thread.MemoryBarrier();
            this._value = value;
            this._isValueCreated = true;
          }
          finally
          {
            if (this._lock != null)
            {
              Monitor.Exit(this._lock);
            }
          }
        }
        return this._value;
      }
    }
  }
}
