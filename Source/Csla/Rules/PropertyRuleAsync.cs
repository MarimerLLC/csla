//-----------------------------------------------------------------------
// <copyright file="PropertyRuleAsync.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create property level rule</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for a property rule 
  /// </summary>
  public abstract class PropertyRuleAsync : BusinessRuleAsync
  {
    /// <summary>
    /// Gets or sets the error message (constant).
    /// </summary>
    public string MessageText
    {
      get { return MessageDelegate.Invoke(); }
      set { MessageDelegate = () => value; }
    }

    /// <summary>
    /// Gets or sets the error message function for this rule.
    /// Use this for localizable messages from a resource file. 
    /// </summary>    
    public Func<string> MessageDelegate { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance has message delegate.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has message delegate; otherwise, <c>false</c>.
    /// </value>
    protected bool HasMessageDelegate
    {
      get { return MessageDelegate != null; }
    }

    /// <summary>
    /// Gets the error message text.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetMessage()
    {
      return MessageText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRule"/> class.
    /// </summary>
    protected PropertyRuleAsync()
      : base()
    {
      CanRunAsAffectedProperty = true;
      CanRunOnServer = true;
      CanRunInCheckRules = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRule"/> class.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    protected PropertyRuleAsync(IPropertyInfo propertyInfo) : base(propertyInfo)
    {
      CanRunAsAffectedProperty = true;
      CanRunOnServer = true;
      CanRunInCheckRules = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can run as affected property.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can run as affected property; otherwise, <c>false</c>.
    /// </value>
    public bool CanRunAsAffectedProperty
    {
      get { return (RunMode & RunModes.DenyAsAffectedProperty) == 0; }
      set
      {
        if (value && !CanRunAsAffectedProperty)
        {
          RunMode = RunMode ^ RunModes.DenyAsAffectedProperty;
        }
        else if (!value && CanRunAsAffectedProperty)
        {
          RunMode = RunMode | RunModes.DenyAsAffectedProperty;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can run in logical serverside data portal.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can run logical serverside data portal; otherwise, <c>false</c>.
    /// </value>
    public bool CanRunOnServer
    {
      get { return (RunMode & RunModes.DenyOnServerSidePortal) == 0; }
      set
      {
        if (value && !CanRunOnServer)
        {
          RunMode = RunMode ^ RunModes.DenyOnServerSidePortal;
        }
        else if (!value && CanRunOnServer)
        {
          RunMode = RunMode | RunModes.DenyOnServerSidePortal;
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
