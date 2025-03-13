//-----------------------------------------------------------------------
// <copyright file="PropertyRule.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create property level rule</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for a property rule 
  /// </summary>
  public abstract class PropertyRule : BusinessRule
  {
    /// <summary>
    /// Gets or sets the error message (constant).
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="MessageDelegate"/> is <see langword="null"/>.</exception>
    public string MessageText
    {
      get 
      {
        if (MessageDelegate == null)
          throw new InvalidOperationException($"{nameof(MessageDelegate)} == null");
        
        return MessageDelegate.Invoke(); 
      }
      set { MessageDelegate = () => value; }
    }

    /// <summary>
    /// Gets or sets the error message function for this rule.
    /// Use this for localizable messages from a resource file. 
    /// </summary>    
    public Func<string>? MessageDelegate { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance has message delegate.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has message delegate; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(MessageDelegate))]
    protected bool HasMessageDelegate => MessageDelegate != null;

    /// <summary>
    /// Gets the error message text.
    /// </summary>
    protected virtual string GetMessage()
    {
      return MessageText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRule"/> class.
    /// </summary>
    protected PropertyRule()
    {
      CanRunAsAffectedProperty = true;
      CanRunOnServer = true;
      CanRunInCheckRules = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRule"/> class.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    protected PropertyRule(IPropertyInfo? propertyInfo) : base(propertyInfo)
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
      get => (RunMode & RunModes.DenyAsAffectedProperty) == 0;
      set
      {
        if (value && !CanRunAsAffectedProperty)
        {
          RunMode ^= RunModes.DenyAsAffectedProperty;
        }
        else if (!value && CanRunAsAffectedProperty)
        {
          RunMode |= RunModes.DenyAsAffectedProperty;
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
      get => (RunMode & RunModes.DenyOnServerSidePortal) == 0;
      set
      {
        if (value && !CanRunOnServer)
        {
          RunMode ^= RunModes.DenyOnServerSidePortal;
        }
        else if (!value && CanRunOnServer)
        {
          RunMode |= RunModes.DenyOnServerSidePortal;
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
      get => (RunMode & RunModes.DenyCheckRules) == 0;
      set
      {
        if (value && !CanRunInCheckRules)
        {
          RunMode ^= RunModes.DenyCheckRules;
        }
        else if (!value && CanRunInCheckRules)
        {
          RunMode |= RunModes.DenyCheckRules;
        }
      }
    }
  }
}
