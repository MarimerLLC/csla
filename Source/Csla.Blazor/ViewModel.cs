//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type for creating your own view model</summary>
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Csla.Core;
using Csla.Reflection;

namespace Csla.Blazor
{
  /// <summary>
  /// Base type for creating your own view model.
  /// </summary>
  public class ViewModel<T> : Csla.Mvvm.ViewModel<T>
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ViewModel(ApplicationContext applicationContext)
      : base(applicationContext)
    { }

    /// <inheritdoc />
    protected override void OnModelChanging(T? oldValue, T? newValue)
    {
      if (!ReferenceEquals(oldValue, newValue))
        _propertyInfoCache.Clear();
      base.OnModelChanging(oldValue, newValue);
    }

    #region GetPropertyInfo

    private const string TextSeparator = " ";

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property)
    {
      ArgumentNullException.ThrowIfNull(property);

      var keyName = property.GetKey();
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName, TextSeparator);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <param name="textSeparator">text seprator for concatenating errors </param>
    /// <exception cref="ArgumentNullException"><paramref name="textSeparator"/> or <paramref name="property"/> is <see langword="null"/>.</exception>
    public IPropertyInfo GetPropertyInfo<P>(string textSeparator, Expression<Func<P>> property)
    {
      ArgumentNullException.ThrowIfNull(textSeparator);
      ArgumentNullException.ThrowIfNull(property);

      var keyName = property.GetKey();
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName, textSeparator);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property, string id)
    {
      ArgumentNullException.ThrowIfNull(property);

      var keyName = property.GetKey() + $"[{id}]";
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName, TextSeparator);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    /// <exception cref="InvalidOperationException"><see cref="Csla.Mvvm.ViewModel{T}.Model"/> is <see langword="null"/>.</exception>
    public IPropertyInfo GetPropertyInfo(string propertyName)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
      _ = Model ?? throw new InvalidOperationException($"{nameof(Model)} == null");

      var keyName = Model.GetType().FullName + "." + propertyName;
      return GetPropertyInfo(keyName, Model, propertyName, TextSeparator);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    /// <exception cref="InvalidOperationException"><see cref="Csla.Mvvm.ViewModel{T}.Model"/> is <see langword="null"/>.</exception>
    public IPropertyInfo GetPropertyInfo(string propertyName, string id)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
      _ = Model ?? throw new InvalidOperationException($"{nameof(Model)} == null");

      var keyName = Model.GetType().FullName + "." + propertyName + $"[{id}]";
      return GetPropertyInfo(keyName, Model, propertyName, TextSeparator);
    }

    private readonly Dictionary<string, IPropertyInfo> _propertyInfoCache = [];

    private IPropertyInfo GetPropertyInfo(string keyName, object model, string propertyName, string textSeparator)
    {
      if (_propertyInfoCache.TryGetValue(keyName, out var result))
      {
        return result;
      }

      result = new PropertyInfo(model, propertyName, textSeparator);
      _propertyInfoCache.Add(keyName, result);
      return result;
    }

    #endregion
  }
}
