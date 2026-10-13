//-----------------------------------------------------------------------
// <copyright file="RequiredPropertyNotSetException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception indicating that a required property is null</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Exception indicating that one or more properties
  /// registered with <see cref="RelationshipTypes.Required"/>
  /// are null after a data portal operation completed.
  /// </summary>
  [Serializable]
  public class RequiredPropertyNotSetException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public RequiredPropertyNotSetException(string message)
      : base(message)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
    public RequiredPropertyNotSetException(string message, Exception ex)
      : base(message, ex)
    {
      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(message)), nameof(message));
      if (ex is null)
        throw new ArgumentNullException(nameof(ex));
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="objectType">Type of the object containing the properties.</param>
    /// <param name="propertyNames">Names of the required properties that are null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="propertyNames"/> is <see langword="null"/>.</exception>
    public RequiredPropertyNotSetException(Type objectType, IEnumerable<string> propertyNames)
      : this(objectType ?? throw new ArgumentNullException(nameof(objectType)), (propertyNames ?? throw new ArgumentNullException(nameof(propertyNames))).ToList())
    { }

    private RequiredPropertyNotSetException(Type objectType, List<string> propertyNames)
      : base(string.Format(Properties.Resources.RequiredPropertyNotSetException, objectType.FullName, string.Join(", ", propertyNames)))
    {
      ObjectType = objectType;
      PropertyNames = propertyNames.AsReadOnly();
    }

    /// <summary>
    /// Gets the type of the object containing the
    /// required properties that are null.
    /// </summary>
    public Type? ObjectType { get; }

    /// <summary>
    /// Gets the names of the required properties
    /// that are null.
    /// </summary>
    public IReadOnlyList<string> PropertyNames { get; } = [];
  }
}
