#if !IOS
//-----------------------------------------------------------------------
// <copyright file="DynamicMemberHandle.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Csla.Reflection
{
  internal class DynamicMemberHandle
  {
    public string MemberName { get; private set; }
    public Type MemberType { get; private set; }
    public DynamicMemberGetDelegate DynamicMemberGet { get; private set; }
    public DynamicMemberSetDelegate DynamicMemberSet { get; private set; }

    //public string MemberFullName
    //{
    //  get { return MemberType + "." + MemberName; }
    //}

    public DynamicMemberHandle(string memberName, Type memberType, DynamicMemberGetDelegate dynamicMemberGet, DynamicMemberSetDelegate dynamicMemberSet)
    {
      MemberName = memberName;
      MemberType = memberType;
      DynamicMemberGet = dynamicMemberGet;
      DynamicMemberSet = dynamicMemberSet;
    }

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
  }
}
#endif