using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// <param name="e"></param>
    void Initialize(InterceptArgs e);
    /// <summary>
    /// Invoked at the end of each server-side
    /// data portal invocation for success
    /// and exception scenarios.
    /// </summary>
    /// <param name="e"></param>
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
    public Type ObjectType { get; set; }
    /// <summary>
    /// Gets or sets the criteria or business
    /// object paramter provided to the
    /// data portal from the client.
    /// </summary>
    public object Parameter { get; set; }
    /// <summary>
    /// Gets or sets the business object
    /// resulting from the data portal
    /// operation.
    /// </summary>
    public DataPortalResult Result { get; set; }
    /// <summary>
    /// Gets or sets the exception that occurred
    /// during data portal processing.
    /// </summary>
    public Exception Exception { get; set; }
    /// <summary>
    /// Gets or sets the data portal operation
    /// being performed.
    /// </summary>
    public DataPortalOperations Operation { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether
    /// the data portal was invoked synchronously.
    /// </summary>
    public bool IsSync { get; set; }

    /// <summary>
    /// Gets or sets a value containing the elapsed
    /// runtime for this operation (only valid at end
    /// of operation).
    /// </summary>
    public TimeSpan Runtime { get; set; }
  }
}
