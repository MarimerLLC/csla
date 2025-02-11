#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMemberHandle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;

namespace Csla.Reflection
{
  internal class DynamicMemberHandle
  {
    public string MemberName { get; }
    public Type MemberType { get; }
    public DynamicMemberGetDelegate? DynamicMemberGet { get; }
    public DynamicMemberSetDelegate? DynamicMemberSet { get; }

    public DynamicMemberHandle(PropertyInfo info) :
      this(
            info.Name,
            info.PropertyType,
            DynamicMethodHandlerFactory.CreatePropertyGetter(info),
            DynamicMethodHandlerFactory.CreatePropertySetter(info))
    { }
    public DynamicMemberHandle(FieldInfo info) :
      this(
            info.Name,
            info.FieldType,
            DynamicMethodHandlerFactory.CreateFieldGetter(info),
            DynamicMethodHandlerFactory.CreateFieldSetter(info))
    { }
    /// <summary>
    /// Initializes a new instance of <see cref="DynamicMemberHandle"/>-object.
    /// </summary>
    /// <param name="memberName">The member name.</param>
    /// <param name="memberType">Type of the member</param>
    /// <param name="dynamicMemberGet">Delegate to invoke the get of the member dynamically.</param>
    /// <param name="dynamicMemberSet">Delegate to invoke the set of the member dynamically.</param>
    /// <exception cref="ArgumentNullException"><paramref name="memberType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="memberName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public DynamicMemberHandle(string memberName, Type memberType, DynamicMemberGetDelegate? dynamicMemberGet, DynamicMemberSetDelegate? dynamicMemberSet)
    {
      if (string.IsNullOrWhiteSpace(memberName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(memberName)), nameof(memberName));

      MemberName = memberName;
      DynamicMemberSet = dynamicMemberSet;
      DynamicMemberGet = dynamicMemberGet;
      MemberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
    }

    public object? MemberGetOrNotSupportedException(object target)
    {
      if (DynamicMemberGet is null)
        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' on Type '{1}' does not have a public getter.", MemberName, target.GetType()));

      return DynamicMemberGet(target);
    }

    public void MemberSetOrNotSupportedException(object target, object? value)
    {
      if (DynamicMemberSet is null)
        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' on Type '{1}' does not have a public setter.", MemberName, target.GetType()));

      DynamicMemberSet(target, value);
    }
  }
}
#endif