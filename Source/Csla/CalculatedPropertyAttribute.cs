//-----------------------------------------------------------------------
// <copyright file="CalculatedPropertyAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a managed property holds a calculated value</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Indicates that a managed property of a business object holds
  /// a value calculated by business rules from other property values.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A calculated property:
  /// </para>
  /// <list type="bullet">
  /// <item><description>is not serialized (as if it were marked with <see cref="DoNotSerializeAttribute"/>);</description></item>
  /// <item><description>never affects the <c>IsSelfDirty</c> or <c>IsDirty</c> state of the
  /// business object, whether its value is changed by a rule, <c>SetProperty</c> or <c>LoadProperty</c>;
  /// property changed notifications and dependent rules still run as normal;</description></item>
  /// <item><description>is recalculated after the business object is deserialized by the
  /// <see cref="Serialization.Mobile.MobileFormatter"/>, by running the rules whose primary property
  /// is the calculated property. This happens once the whole object graph has been deserialized
  /// (after any <c>Deserialized()</c> overrides), and child objects are recalculated before the
  /// objects that contain them, so a parent's calculation can use its children's calculated values.</description></item>
  /// </list>
  /// <para>
  /// The value should be set by business rules (for example through <c>context.AddOutValue</c>),
  /// so the property should not have a public setter. The CSLA0026 analyzer reports a
  /// calculated property with a non-private setter.
  /// </para>
  /// <para>
  /// This attribute applies to managed properties of <see cref="BusinessBase{T}"/> types,
  /// and is intended for simple values, not child objects or collections.
  /// Rules for calculated properties that are asynchronous are started after
  /// deserialization but are not awaited.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class CalculatedPropertyAttribute : Attribute;
}
