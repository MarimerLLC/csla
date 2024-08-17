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
    public DynamicMemberGetDelegate DynamicMemberGet { get; }
    public DynamicMemberSetDelegate DynamicMemberSet { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DynamicMemberHandle"/>-object.
    /// </summary>
    /// <param name="memberName">The member name.</param>
    /// <param name="memberType">Type of the member</param>
    /// <param name="dynamicMemberGet">Delegate to invoke the get of the member dynamically.</param>
    /// <param name="dynamicMemberSet">Delegate to invoke the set of the member dynamically.</param>
    /// <exception cref="ArgumentNullException"><paramref name="memberType"/>, <paramref name="dynamicMemberGet"/> or <paramref name="dynamicMemberSet"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="memberName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public DynamicMemberHandle(string memberName, Type memberType, DynamicMemberGetDelegate dynamicMemberGet, DynamicMemberSetDelegate dynamicMemberSet)
    {
      if (string.IsNullOrWhiteSpace(memberName))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(memberName)), nameof(memberName));

      MemberName = memberName;
      MemberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
      DynamicMemberGet = dynamicMemberGet ?? throw new ArgumentNullException(nameof(dynamicMemberGet));
      DynamicMemberSet = dynamicMemberSet ?? throw new ArgumentNullException(nameof(dynamicMemberSet));
    }

    public DynamicMemberHandle(PropertyInfo info) :
      this(
            info.Name,
            info.PropertyType,
            DynamicMethodHandlerFactory.CreatePropertyGetter(info) ?? throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' on Type '{1}' does not have a getter.", info.Name, info.DeclaringType)),
            DynamicMethodHandlerFactory.CreatePropertySetter(info) ?? throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' on Type '{1}' does not have a setter.", info.Name, info.DeclaringType)))
    { }
    public DynamicMemberHandle(FieldInfo info) :
      this(
            info.Name,
            info.FieldType,
            DynamicMethodHandlerFactory.CreateFieldGetter(info),
            DynamicMethodHandlerFactory.CreateFieldSetter(info))
    { }
  }
}
#endif