using System;

/// <summary>
/// Exception class indicating that there was a validation
/// problem with a business object.
/// </summary>
[Serializable()]
public class ValidationException : ApplicationException
{
  /// <summary>
  /// Initializes a new instance of the 
  /// <see cref="T:CSLA.ValidationException" /> class.
  /// </summary>
  public ValidationException()
  {
  }

  /// <summary>
  /// Initializes a new instance of the 
  /// <see cref="T:CSLA.ValidationException" /> class
  /// with a specified error message.
  /// </summary>
  public ValidationException(string message) : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the 
  /// <see cref="T:CSLA.ValidationException" /> class
  /// with a specified error message and a reference to the 
  /// inner exception that is the cause of this exception.
  /// </summary>
  public ValidationException(string message, Exception innerException) : base(message, innerException)
  {
  }

  /// <summary>
  /// Initializes a new instance of the 
  /// <see cref="T:CSLA.ValidationException" /> class
  /// with serialized data.
  /// </summary>
  protected ValidationException(System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context)
  {
  }
}
