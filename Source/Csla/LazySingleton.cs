using System.Diagnostics.CodeAnalysis;

namespace Csla
{
  /// <summary>
  /// An alternative to Lazy&lt;T&gt; 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class LazySingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : Core.IUseApplicationContext
    where T : class
  {
#if NET9_0_OR_GREATER
    private readonly Lock _syncRoot = new();
#else
    private readonly object _syncRoot = new();
#endif
    private T? _value;
    private readonly Func<T> _delegate;

    private ApplicationContext _applicationContext = default!;

    /// <inheritdoc />
    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext)); }

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
    /// <exception cref="ArgumentNullException"><paramref name="delegate"/> is <see langword="null"/>.</exception>
    public LazySingleton(Func<T> @delegate)
    {
      _delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
    }

    /// <summary>
    /// Gets a value indicating whether this instance is initialized and contains a value.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is value created; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(_value))]
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

        return _value!;
      }
    }
  }
}