using System;
using System.Linq;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Tracks the business rules for a business object.
  /// </summary>
  [Serializable]
  public class BusinessRules : Csla.Core.MobileObject
  {
    // list of broken rules for this business object.
    private BrokenRulesCollection _brokenRules;
    // reference to current business object
    [NonSerialized()]
    private object _target;
    // threshold for short-circuiting to kick in
    private int _processThroughPriority;
    // reference to per-type rules manager for this object
    [NonSerialized]
    private BusinessRuleManager _typeRules;

    internal void SetTarget(object target)
    {
      _target = target;
    }

    public void AddObjectRule(IBusinessRule rule)
    {
      _typeRules.RuleMethods.Add(new RuleMethod(rule, null, 0));
    }

    public void AddObjectRule(IBusinessRule rule, int priority)
    {
      _typeRules.RuleMethods.Add(new RuleMethod(rule, null, priority));
    }

    public void AddRule(IBusinessRule rule, Csla.Core.IPropertyInfo property)
    {
      _typeRules.RuleMethods.Add(new RuleMethod(rule, property, 0));
    }

    public void AddRule(IBusinessRule rule, Csla.Core.IPropertyInfo property, int priority)
    {
      _typeRules.RuleMethods.Add(new RuleMethod(rule, property, priority));
    }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckRules()
    {
      var result = new List<string>();

      return result;
    }

    /// <summary>
    /// Invokes all rules attached at the class level
    /// of the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckObjectRules()
    {
      var result = new List<string>();
      var rules = _typeRules.RuleMethods.Where(c => c.PrimaryProperty == null).ToList();
      return result;
    }

    /// <summary>
    /// Invokes all rules for a specific property of the business type.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckRules(Csla.Core.IPropertyInfo property)
    {
      var result = new List<string>();
      var rules = _typeRules.RuleMethods.Where(c => ReferenceEquals(c.PrimaryProperty, property)).ToList();
      return result;
    }

    #region MobileObject overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("_processThroughPriority", _processThroughPriority);
#if SILVERLIGHT
      OnGetStatePartial(info, mode);
#endif
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _processThroughPriority = info.GetValue<int>("_processThroughPriority");
#if SILVERLIGHT
      OnSetStatePartial(info, mode);
#endif
      base.OnSetState(info, mode);
    }

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_brokenRules != null && _brokenRules.Count > 0)
      {
        SerializationInfo brInfo = formatter.SerializeObject(_brokenRules);
        info.AddChild("_brokenRules", brInfo.ReferenceId);
      }

      base.OnGetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.ContainsKey("_brokenRules"))
      {
        int referenceId = info.Children["_brokenRules"].ReferenceId;
        _brokenRules = (BrokenRulesCollection)formatter.GetObject(referenceId);
      }

      base.OnSetChildren(info, formatter);
    }

#if SILVERLIGHT
    /// <summary>
    /// Gets the object state.
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="mode">Serialization mode</param>
    protected virtual void OnGetStatePartial(SerializationInfo info, StateMode mode)
    {
      if (mode == StateMode.Serialization)
      {
        if (_stateStack.Count > 0)
        {
          byte[] xml = Utilities.XmlSerialize(_stateStack.ToArray());
          info.AddValue("_stateStack", xml);
        }
      }

      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Sets the object state.
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="mode">Serialization mode</param>
    protected virtual void OnSetStatePartial(SerializationInfo info, StateMode mode)
    {
      if (mode == StateMode.Serialization)
      {
        _stateStack.Clear();

        if (info.Values.ContainsKey("_stateStack"))
        {
          //string xml = info.GetValue<string>("_stateStack");
          byte[] xml = info.GetValue<byte[]>("_stateStack");
          SerializationInfo[] layers = Utilities.XmlDeserialize<SerializationInfo[]>(xml);
          Array.Reverse(layers);
          foreach (SerializationInfo layer in layers)
            _stateStack.Push(layer);
        }
      }

      base.OnSetState(info, mode);
    }
#endif
    #endregion
  }
}
