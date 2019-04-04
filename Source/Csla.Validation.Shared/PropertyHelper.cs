﻿//-----------------------------------------------------------------------
// <copyright file="PropertyHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>internal implemetation to get a registered property</summary>
//----------------------------------------------------------------------
using System;
using System.Linq;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Rules;

namespace Csla.Validation
{
  internal static class PropertyHelper
  {
    public static IPropertyInfo GetRegisteredProperty(BusinessRules businessRules, string propertyName)
    {
      var rules = (IBusinessRules) businessRules;
      var target = rules.Target;
      var primaryProperty = PropertyInfoManager.GetRegisteredProperty(target.GetType(), propertyName);

      if (primaryProperty == null)
        throw new ArgumentException(string.Format("Unkonwn property. {0} is not a registered field", propertyName));

      return primaryProperty;
    }
  }
}