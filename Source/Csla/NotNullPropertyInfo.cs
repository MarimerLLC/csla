//-----------------------------------------------------------------------
// <copyright file="NotNullPropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a property whose value must not be null.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a property whose value must not be null.
  /// </summary>
  /// <typeparam name="T">
  /// Data type of the property.
  /// </typeparam>
  /// <remarks>
  /// <para>
  /// Register it with <c>RegisterNotNullProperty</c>. Its typed read methods
  /// (<c>GetProperty</c>, <c>ReadProperty</c>) return <typeparamref name="T"/>
  /// instead of <c>T?</c>.
  /// </para>
  /// <para>
  /// The framework does not invent a value at read time. It rejects null writes, and
  /// the data portal verifies at the end of the operation that completes an object
  /// (Create/Fetch, or Execute for a command) that the property holds a value.
  /// A property created with a default value is satisfied by that default; a
  /// property created without one must be loaded on every Create/Fetch path.
  /// </para>
  /// <para>
  /// The relationship is always <see cref="RelationshipTypes.None"/>: lazy loading,
  /// private backing fields and non-serialized fields are not supported.
  /// </para>
  /// </remarks>
  public class NotNullPropertyInfo<T> : PropertyInfo<T>, Core.INotNullPropertyInfo
    where T : notnull
  {
    /// <summary>
    /// Creates a new instance of this class without a default value.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public NotNullPropertyInfo(string name)
      : this(name, null, null)
    { }

    /// <summary>
    /// Creates a new instance of this class without a default value.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Type that declares the property, used to provide the display name from attributes.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public NotNullPropertyInfo(string name, string? friendlyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type? containingType = null)
      : base(name, friendlyName, containingType, default, RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class with a default value.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Type that declares the property, used to provide the display name from attributes.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property. An unloaded property reads as this value.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="defaultValue"/> is <see langword="null"/>.</exception>
    public NotNullPropertyInfo(string name, string? friendlyName, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type? containingType, T defaultValue)
      : base(name, friendlyName, containingType, defaultValue ?? throw new ArgumentNullException(nameof(defaultValue)), RelationshipTypes.None)
    {
      HasDefaultValue = true;
    }

    /// <summary>
    /// Gets a value indicating whether the property was created with a default value.
    /// </summary>
    /// <remarks>
    /// A property without a default value has a <see cref="PropertyInfo{T}.DefaultValue"/>
    /// of <c>default(T)</c>, which is null for a reference type.
    /// </remarks>
    public bool HasDefaultValue { get; }
  }
}
