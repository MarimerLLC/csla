using System;
using System.Threading;

namespace Csla
{
  /// <summary>
  /// An alternative to Lazy&lt;T&gt; 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class LazySingleton<T> : Core.IUseApplicationContext
    where T : class
  {
    private readonly object _syncRoot = new object();
    private T _value;
    private readonly Func<T> _delegate;
    private bool _isValueCreated;

    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazySingleton&lt;T&gt;"/> class.
    /// Will use the default public constructor to create an instance of T (the value)
    /// </summary>
    public LazySingleton()
    {
      _delegate = () => (T)ApplicationContext.CreateInstanceDI(typeof(T));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazySingleton&lt;T&gt;"/> class.
    /// Will call the supplied delegate to create an instance of T (the value)
    /// </summary>
    /// <param name="delegate">The @delegate.</param>
    public LazySingleton(Func<T> @delegate)
    {
      _delegate = @delegate;
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
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public T Value
    {
      get
      {
        if (!_isValueCreated) // not initialized 
        {
          lock (_syncRoot)       // lock syncobject
          {
            if (!_isValueCreated) // recheck for initialized after lock is taken
            {
               _value = _delegate.Invoke();
              _isValueCreated = true;   // mark as initialized
            }
          }
        }

        return _value;
      }
    }
  }
}