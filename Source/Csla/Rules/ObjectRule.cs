﻿//-----------------------------------------------------------------------
// <copyright file="ObjectRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create object level rule</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using Csla.Properties;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for object level rules.
  /// </summary>
  public abstract class ObjectRule : BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectRule"/> class.
    /// </summary>
    protected ObjectRule()
    { }

    /// <summary>
    /// Gets or sets the primary property affected by this rule.
    /// Will always return null.
    /// Will throw ArgumentException if set to anything but null.
    /// </summary>
    public override IPropertyInfo PrimaryProperty
    {
      get
      {
        return null;
      }
      set
      {
        if (value != null)
        {
          throw new ArgumentException(Resources.ObjectRulesCannotSetPrimaryProperty, "PrimaryProperty");
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can run when CheckRules is called on BO.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can run when CheckRules is called; otherwise, <c>false</c>.
    /// </value>
    public bool CanRunInCheckRules
    {
      get { return (RunMode & RunModes.DenyCheckRules) == 0; }
      set
      {
        if (value && !CanRunInCheckRules)
        {
          RunMode = RunMode ^ RunModes.DenyCheckRules;
        }
        else if (!value && CanRunInCheckRules)
        {
          RunMode = RunMode | RunModes.DenyCheckRules;
        }
      }
    }
  }
}
