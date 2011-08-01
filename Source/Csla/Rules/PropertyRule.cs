using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for a property rule 
  /// These rules will run without any restrictions.
  /// </summary>
  public abstract class PropertyRule : BusinessRule
  {


    /// <summary>
    /// Gets the error message.
    /// </summary>
    protected virtual string Message
    {
      get { return MessageDelegate.Invoke(); }
    }

    /// <summary>
    /// Gets or sets the error message function for this rule.
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
    /// Initializes a new instance of the <see cref="PropertyRule"/> class.
    /// </summary>
    protected PropertyRule()
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
    protected PropertyRule(IPropertyInfo propertyInfo) : base(propertyInfo)
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
    /// 	<c>true</c> if this instance can run in  in logical serverside data portal; otherwise, <c>false</c>.
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
  }
}
