//-----------------------------------------------------------------------
// <copyright file="CriteriaBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base type from which Criteria classes can be</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;
using Csla.Core;
using System.ComponentModel;
using System.Linq.Expressions;
using Csla.Reflection;
using System.Reflection;
using Csla.Serialization;

namespace Csla
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable]
  public abstract class CriteriaBase<T> : ManagedObjectBase
    where T : CriteriaBase<T>
  {
#if SILVERLIGHT || NETFX_CORE
    private static bool _forceInit = false;

    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    public CriteriaBase()
    {
      _forceInit = _forceInit && false;
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="mode">Serialization mode.</param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _forceInit = _forceInit && false;
      base.OnSetState(info, mode);
    }
#else
    /// <summary>
    /// Initializes empty CriteriaBase. The type of
    /// business object to be created by the DataPortal
    /// MUST be supplied by the subclass.
    /// </summary>
    protected CriteriaBase()
    { }
#endif

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
    protected static PropertyInfo<P> RegisterProperty<P>(PropertyInfo<P> info)
    {
      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, reflectedPropertyInfo.Name, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    /// <returns></returns>
    protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

      return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship));
    }

    #endregion
  }
}