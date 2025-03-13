//-----------------------------------------------------------------------
// <copyright file="DataPortalResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Returns data from the server-side DataPortal to the </summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.Server
{
  /// <summary>
  /// Returns data from the server-side DataPortal to the 
  /// client-side DataPortal. Intended for internal CSLA .NET
  /// use only.
  /// </summary>
  [Serializable]
  public class DataPortalResult : EventArgs, Core.IUseApplicationContext
  {
    internal ApplicationContext ApplicationContext { get; set; }

    /// <inheritdoc />
    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext)); }

    /// <summary>
    /// The business object being returned from
    /// the server.
    /// </summary>
    public object? ReturnObject { get; private set; }

    /// <summary>
    /// Error that occurred during the DataPortal call.
    /// This will be null if no errors occurred.
    /// </summary>
    public Exception? Error { get; private set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. Not usable by user code
    public DataPortalResult()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public DataPortalResult(ApplicationContext applicationContext) : this(applicationContext, null, null)
    {
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="returnObject"/> is <see langword="null"/>.</exception>
    public DataPortalResult(ApplicationContext applicationContext, object returnObject) : this(applicationContext, returnObject, null)
    {
      if (returnObject is null)
        throw new ArgumentNullException(nameof(returnObject));
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="returnObject">Object to return as part
    /// of the result.</param>
    /// <param name="ex">
    /// Error that occurred during the DataPortal call.
    /// This will be null if no errors occurred.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public DataPortalResult(ApplicationContext applicationContext, object? returnObject, Exception? ex)
    {
      ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      ReturnObject = returnObject;
      Error = ex;
    }
  }
}