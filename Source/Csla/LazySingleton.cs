namespace Csla
{
  /// <summary>
  /// An alternative to Lazy&lt;T&gt; 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class LazySingleton<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    T> : Core.IUseApplicationContext
    where T : class
  {
    private readonly Lock _syncRoot = LockFactory.Create();
    private T _value;
    private readonly Func<T> _delegate;

    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value; }
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazySingleton&lt;T&gt;"/> class.
    /// Will use the default public constructor to create an instance of T (the value)
    /// </summary>
    public LazySingleton()
    {
      _delegate = () => (T)_applicationContext.CreateInstanceDI(typeof(T));
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
    public bool IsValueCreated { get; private set; }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public T Value
    {
      get
      {
        if (!IsValueCreated) // not initialized 
        {
          lock (_syncRoot)       // lock syncobject
          {
            if (!IsValueCreated) // recheck for initialized after lock is taken
            {
               _value = _delegate.Invoke();
              IsValueCreated = true;   // mark as initialized
            }
          }
        }

        return _value;
      }
    }
  }
}