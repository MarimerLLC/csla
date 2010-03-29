using System;
using System.Linq;
using Csla.Serialization;
using System.Collections.Generic;
using Csla.Serialization.Mobile;
using Csla.Core;
using System.Collections.ObjectModel;

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
    // threshold for short-circuiting to kick in
    private int _processThroughPriority;

    // reference to current business object
    [NonSerialized]
    private object _target;
    // reference to per-type rules manager for this object
    [NonSerialized]
    private BusinessRuleManager _typeRules;
    // async rules currently executing
    [NonSerialized]
    private ObservableCollection<RuleContext> _validatingRules;

    private BusinessRuleManager TypeRules
    {
      get
      {
        if (_typeRules == null && _target != null)
          _typeRules = BusinessRuleManager.GetRulesForType(_target.GetType());
        return _typeRules;
      }
    }

    internal ObservableCollection<RuleContext> ValidatingRules
    {
      get
      {
        if (_validatingRules == null)
          _validatingRules = new ObservableCollection<RuleContext>();

        return _validatingRules;
      }
    }

    internal void SetTarget(object target)
    {
      _target = target;
    }

    internal object Target
    {
      get { return _target; }
    }

    public BusinessRules()
    { }

    public BusinessRules(object target)
    {
      SetTarget(target);
    }

    /// <summary>
    /// Associates a business rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    public void AddRule(IBusinessRule rule)
    {
      TypeRules.RuleMethods.Add(rule);
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
      //var affectedProperties = new List<string>();
      //affectedProperties.AddRange(CheckObjectRules());

      //return affectedProperties;
      return RunRules(TypeRules.RuleMethods);
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
      var rules = TypeRules.RuleMethods.Where(c => c.PrimaryProperty == null);
      return RunRules(rules);
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
      var affectedProperties = new List<string>();
      var rules = from r in TypeRules.RuleMethods
                  where r.AffectedProperties.Count > 0 &&  ReferenceEquals(r.AffectedProperties[0], property)
                  orderby r.Priority
                  select r;
      return affectedProperties;
    }

    private List<string> RunRules(IEnumerable<IBusinessRule> rules)
    {
      var affectedProperties = new List<string>();
      foreach (var rule in rules)
      {
        // setup callback handler
        var context = new RuleContext((r) =>
        {
          foreach (var result in r.Results)
            _brokenRules.SetBrokenRule(result);
          foreach (var item in rule.AffectedProperties)
            affectedProperties.Add(item.Name);
        });

        // set up rest of context
        context.Rule = rule;
        if (!rule.IsAsync)
          context.Target = _target;

        // get input properties
        if (rule.InputProperties != null)
        {
          var target = _target as Core.IManageProperties;
          context.InputPropertyValues = new Dictionary<IPropertyInfo, object>();
          foreach (var item in rule.InputProperties)
            context.InputPropertyValues.Add(item, target.ReadProperty(item));
        }

        // execute (or start executing) rule
        rule.Execute(context);

        // process any synchronous results
        if (!rule.IsAsync && context.Results != null)
        {
          var stop = (from r in context.Results
                     where r.StopProcessing == true
                     select r).Count() > 0;
          if (stop)
            break;
        }
      }
      // return any synchronous results
      return affectedProperties;
    }

    #region DataAnnotations

    /// <summary>
    /// Adds validation rules corresponding to property
    /// data annotation attributes.
    /// </summary>
    public void AddDataAnnotations()
    {
      Type metadataType;
#if SILVERLIGHT
      metadataType = _target.GetType();
#else
      var classAttList = _target.GetType().GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MetadataTypeAttribute), true);
      if (classAttList.Length > 0)
        metadataType = ((System.ComponentModel.DataAnnotations.MetadataTypeAttribute)classAttList[0]).MetadataClassType;
      else
        metadataType = _target.GetType();
#endif

      var propList = metadataType.GetProperties();
      foreach (var prop in propList)
      {
        var attList = prop.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute), true);
        foreach (var att in attList)
        {
          var target = (IManageProperties)_target;
          var pi = target.GetManagedProperties().Where(c => c.Name == prop.Name).First();
          AddRule(new CommonRules.DataAnnotation(pi, (System.ComponentModel.DataAnnotations.ValidationAttribute)att));
        }
      }
    }

    #endregion

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
