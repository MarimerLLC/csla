//-----------------------------------------------------------------------
// <copyright file="CriteriaBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type from which Criteria classes can be</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using System.Linq.Expressions;
using Csla.Reflection;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Csla
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable]
  [Obsolete("Use types that can be serialized by CSLA. See the `/docs/Upgrade to CSLA 9.md` document for details.", false)]
  public abstract class CriteriaBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : ManagedObjectBase
    where T : CriteriaBase<T>
  {
    /// <summary>
    /// Initializes empty CriteriaBase. The type of
    /// business object to be created by the DataPortal
    /// MUST be supplied by the subclass.
    /// </summary>
    protected CriteriaBase()
    {
      ((IUseApplicationContext)this).ApplicationContext = ApplicationContext.DummyApplicationContext.Value;
    }

    #region  Register Properties

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of property.
    /// </typeparam>
    /// <param name="info">
    /// PropertyInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IPropertyInfo object.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> info)
    {
      Guard.NotNull(info);

      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName)
    {
      Guard.NotNull(propertyName);

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      Guard.NotNull(propertyLambdaExpression);

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, RelationshipTypes relationship)
    {
      Guard.NotNull(propertyName);

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      Guard.NotNull(propertyLambdaExpression);

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, relationship);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName)
    {
      Guard.NotNull(propertyName);

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName)
    {
      Guard.NotNull(propertyLambdaExpression);

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P defaultValue)
    {
      Guard.NotNull(propertyName);

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P defaultValue)
    {
      Guard.NotNull(propertyLambdaExpression);

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty(reflectedPropertyInfo.Name, friendlyName, defaultValue);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      Guard.NotNull(propertyName);

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      Guard.NotNull(propertyLambdaExpression);

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship);
    }

    #endregion
  }
}