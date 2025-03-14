namespace Csla.Server
{
  /// <summary>
  /// Implement this interface to create a data
  /// portal interceptor that is notified each
  /// time the data portal is invoked and
  /// completes processing.
  /// </summary>
  public interface IInterceptDataPortal
  {
    /// <summary>
    /// Invoked at the start of each server-side
    /// data portal invocation, immediately after
    /// the context has been set, and before
    /// authorization.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    Task InitializeAsync(InterceptArgs e);
    /// <summary>
    /// Invoked at the end of each server-side
    /// data portal invocation for success
    /// and exception scenarios.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    void Complete(InterceptArgs e);
  }

  /// <summary>
  /// Arguments parameter passed to the
  /// interceptor methods.
  /// </summary>
  public class InterceptArgs
  {
    /// <summary>
    /// Gets or sets the business object type.
    /// </summary>
    public Type ObjectType { get; }
    /// <summary>
    /// Gets or sets the criteria or business
    /// object parameter provided to the
    /// data portal from the client.
    /// </summary>
    public object? Parameter { get; }
    /// <summary>
    /// Gets or sets the business object
    /// resulting from the data portal
    /// operation.
    /// </summary>
    public DataPortalResult? Result { get; }
    /// <summary>
    /// Gets or sets the exception that occurred
    /// during data portal processing.
    /// </summary>
    public Exception? Exception { get; }
    /// <summary>
    /// Gets or sets the data portal operation
    /// being performed.
    /// </summary>
    public DataPortalOperations Operation { get; }
    /// <summary>
    /// Gets or sets a value indicating whether
    /// the data portal was invoked synchronously.
    /// </summary>
    public bool IsSync { get; }

    /// <summary>
    /// Gets or sets a value containing the elapsed
    /// runtime for this operation (only valid at end
    /// of operation).
    /// </summary>
    public TimeSpan Runtime { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="InterceptArgs"/>-object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="result"/> is <see langword="null"/>.</exception>
    public InterceptArgs(Type objectType, object? parameter, DataPortalResult result, DataPortalOperations operation, bool isSync) : this(objectType, parameter, operation, isSync)
    {
      Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="InterceptArgs"/>-object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="exception"/> is <see langword="null"/>.</exception>
    public InterceptArgs(Type objectType, object? parameter, Exception exception, DataPortalOperations operation, bool isSync) : this(objectType, parameter, operation, isSync)
    {
      Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="InterceptArgs"/>-object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> is <see langword="null"/>.</exception>
    public InterceptArgs(Type objectType, object? parameter, DataPortalOperations operation, bool isSync)
    {
      ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
      Parameter = parameter;
      Operation = operation;
      IsSync = isSync;
    }
  }
}
