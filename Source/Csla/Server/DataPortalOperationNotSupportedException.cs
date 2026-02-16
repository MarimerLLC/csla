//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationNotSupportedException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception for unhandled data portal operation dispatch</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Exception thrown by generated IDataPortalOperationMapping code
  /// when the operation/criteria combination is not matched.
  /// This signals DataPortalTarget to fall back to reflection-based invocation.
  /// </summary>
  public class DataPortalOperationNotSupportedException : Exception
  {
    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="operationType">The operation attribute type that was not matched</param>
    /// <param name="criteria">The criteria that was not matched</param>
    public DataPortalOperationNotSupportedException(Type operationType, object?[]? criteria)
      : base($"No generated dispatch found for operation {operationType?.Name} with {criteria?.Length ?? 0} criteria parameters.")
    {
      OperationType = operationType;
      Criteria = criteria;
    }

    /// <summary>
    /// Creates a new instance of the exception for name-based dispatch.
    /// </summary>
    /// <param name="operationName">The operation name that was not matched</param>
    /// <param name="criteria">The criteria that was not matched</param>
    public DataPortalOperationNotSupportedException(string operationName, object?[]? criteria)
      : base($"No generated dispatch found for operation '{operationName}' with {criteria?.Length ?? 0} criteria parameters.")
    {
      OperationName = operationName;
      Criteria = criteria;
    }

    /// <summary>
    /// Gets the operation attribute type that was not matched.
    /// </summary>
    public Type? OperationType { get; }

    /// <summary>
    /// Gets the operation name that was not matched.
    /// </summary>
    public string? OperationName { get; }

    /// <summary>
    /// Gets the criteria that was not matched.
    /// </summary>
    public object?[]? Criteria { get; }
  }
}
