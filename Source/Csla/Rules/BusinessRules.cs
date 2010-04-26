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
#if SILVERLIGHT
    , IUndoableObject
#endif
  {
    private object SyncRoot = new object();

    // list of broken rules for this business object.
    private BrokenRulesCollection _brokenRules;
    private BrokenRulesCollection BrokenRules
    {
      get
      {
        if (_brokenRules == null)
          _brokenRules = new BrokenRulesCollection(true);
        return _brokenRules;
      }
    }

    private int _processThroughPriority;
    /// <summary>
    /// Gets or sets the priority through which
    /// all rules will be processed.
    /// </summary>
    public int ProcessThroughPriority
    {
      get { return _processThroughPriority; }
      set { _processThroughPriority = value; }
    }

    private string _ruleSet = null;
    /// <summary>
    /// Gets or sets the rule set to use for this
    /// business object instance.
    /// </summary>
    public string RuleSet
    {
      get { return string.IsNullOrEmpty(_ruleSet) ? "default" : _ruleSet; }
      set 
      {
        _typeRules = null;
        _typeAuthRules = null;
        _ruleSet = value == "default" ? null : value; 
      }
    }

    [NonSerialized]
    private BusinessRuleManager _typeRules;
    private BusinessRuleManager TypeRules
    {
      get
      {
        if (_typeRules == null && _target != null)
          _typeRules = BusinessRuleManager.GetRulesForType(_target.GetType(), _ruleSet);
        return _typeRules;
      }
    }

    [NonSerialized]
    private AuthorizationRuleManager _typeAuthRules;
    private AuthorizationRuleManager TypeAuthRules
    {
      get
      {
        if (_typeAuthRules == null && _target != null)
          _typeAuthRules = AuthorizationRuleManager.GetRulesForType(_target.GetType(), _ruleSet);
        return _typeAuthRules;
      }
    }

    /// <summary>
    /// Gets a list of rule:// URI values for
    /// the rules defined in the object.
    /// </summary>
    /// <returns></returns>
    public string[] GetRuleDescriptions()
    {
      var result = new List<string>();
      foreach (var item in TypeRules.Rules)
        result.Add(item.RuleName);
      return result.ToArray();
    }

    // reference to current business object
    [NonSerialized]
    private IHostRules _target;

    internal void SetTarget(IHostRules target)
    {
      _target = target;
    }

    internal object Target
    {
      get { return _target; }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public BusinessRules()
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="target">Target business object.</param>
    internal BusinessRules(IHostRules target)
    {
      SetTarget(target);
    }

    /// <summary>
    /// Associates a business rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    public void AddRule(IBusinessRule rule)
    {
      TypeRules.Rules.Add(rule);
    }

    /// <summary>
    /// Associates an authorization rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    public void AddRule(IAuthorizationRule rule)
    {
      EnsureUniqueRule(TypeAuthRules, rule);
      TypeAuthRules.Rules.Add(rule);
    }

    /// <summary>
    /// Associates a per-type authorization rule with 
    /// the business type in the default rule set.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="rule">Rule object.</param>
    public static void AddRule(Type objectType, IAuthorizationRule rule)
    {
      AddRule(objectType, rule, null);
    }

    /// <summary>
    /// Associates a per-type authorization rule with 
    /// the business type.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="rule">Rule object.</param>
    /// <param name="ruleSet">Rule set name.</param>
    public static void AddRule(Type objectType, IAuthorizationRule rule, string ruleSet)
    {
      var typeRules = AuthorizationRuleManager.GetRulesForType(objectType, ruleSet);
      EnsureUniqueRule(typeRules, rule);
      typeRules.Rules.Add(rule);
    }

    private static void EnsureUniqueRule(AuthorizationRuleManager mgr, IAuthorizationRule rule)
    {
      IAuthorizationRule oldRule = null;
      if (rule.Element != null)
        oldRule = mgr.Rules.Where(c => c.Element.Name == rule.Element.Name && c.Action == rule.Action).FirstOrDefault();
      else
        oldRule = mgr.Rules.Where(c => c.Element == null && c.Action == rule.Action).FirstOrDefault();
      if (oldRule != null)
        throw new ArgumentException("rule");
    }

    /// <summary>
    /// Gets a value indicating whether there are
    /// any currently broken rules, which would
    /// mean the object is not valid.
    /// </summary>
    public bool IsValid
    {
      get { return BrokenRules.ErrorCount == 0; }
    }

    /// <summary>
    /// Gets the broken rules list.
    /// </summary>
    public BrokenRulesCollection GetBrokenRules()
    {
      return BrokenRules;
    }

    [NonSerialized]
    private bool _runningRules;
    /// <summary>
    /// Gets a value indicating whether a CheckRules
    /// operation is in progress.
    /// </summary>
    public bool RunningRules
    {
      get { return _runningRules; }
      private set { _runningRules = value; }
    }

    /// <summary>
    /// Gets a value indicating whether any async
    /// rules are currently executing.
    /// </summary>
    public bool RunningAsyncRules
    {
      get { return BusyProperties.Count > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether a specific
    /// property has any async rules running.
    /// </summary>
    /// <param name="property">Property to check.</param>
    public bool GetPropertyBusy(Csla.Core.IPropertyInfo property)
    {
      return BusyProperties.Contains(property);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="action">Authorization action.</param>
    public static bool HasPermission(Type objectType, AuthorizationActions action)
    {
      return false;
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="action">Authorization action.</param>
    /// <param name="objectType">Type of business object.</param>
    public static bool HasPermission(AuthorizationActions action, Type objectType)
    {
      if (action == AuthorizationActions.ReadProperty ||
          action == AuthorizationActions.WriteProperty ||
          action == AuthorizationActions.ExecuteMethod)
        throw new ArgumentOutOfRangeException("action");

      bool result = true;
      var rule =
        AuthorizationRuleManager.GetRulesForType(objectType).Rules.Where(c => c.Element == null && c.Action == action).FirstOrDefault();
      if (rule != null)
      {
        var context = new AuthorizationContext { Rule = rule };
        rule.Execute(context);
        result = context.HasPermission;
      }
      return result;
    }

    /// <summary>
    /// Checks per-instance authorization rules.
    /// </summary>
    /// <param name="action">Authorization action.</param>
    /// <param name="obj">Business object instance.</param>
    public static bool HasPermission(AuthorizationActions action, object obj)
    {
      if (action == AuthorizationActions.ReadProperty ||
          action == AuthorizationActions.WriteProperty ||
          action == AuthorizationActions.ExecuteMethod)
        throw new ArgumentOutOfRangeException("action");

      bool result = true;
      var rule =
        AuthorizationRuleManager.GetRulesForType(obj.GetType()).Rules.Where(c => c.Element == null && c.Action == action).FirstOrDefault();
      if (rule != null)
      {
        var context = new AuthorizationContext { Rule = rule, Target = obj };
        rule.Execute(context);
        result = context.HasPermission;
      }
      return result;
    }

    /// <summary>
    /// Checks per-property authorization rules.
    /// </summary>
    /// <param name="action">Authorization action.</param>
    /// <param name="element">Property or method to check.</param>
    public bool HasPermission(AuthorizationActions action, Csla.Core.IMemberInfo element)
    {
      if (action == AuthorizationActions.CreateObject ||
          action == AuthorizationActions.DeleteObject ||
          action == AuthorizationActions.GetObject ||
          action == AuthorizationActions.EditObject)
        throw new ArgumentOutOfRangeException("action");

      bool result = true;
      var rule =
        AuthorizationRuleManager.GetRulesForType(Target.GetType()).Rules.
        Where(c => c.Element != null && c.Element.Name == element.Name && c.Action == action).FirstOrDefault();
      if (rule != null)
      {
        var context = new AuthorizationContext { Rule = rule, Target = this.Target };
        rule.Execute(context);
        result = context.HasPermission;
      }
      return result;
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
      RunningRules = true;
      var affectedProperties = CheckObjectRules();
      var properties = ((IManageProperties)Target).GetManagedProperties();
      foreach (var property in properties)
        affectedProperties.AddRange(CheckRules(property));
      RunningRules = false;
      if (!RunningRules && !RunningAsyncRules)
        _target.AllRulesComplete();
      return affectedProperties.Distinct().ToList();
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
      var oldRR = RunningRules;
      RunningRules = true;
      var rules = from r in TypeRules.Rules
                  where r.PrimaryProperty == null
                  orderby r.Priority
                  select r;
      BrokenRules.ClearRules(null);
      var affectedProperties = RunRules(rules);
      RunningRules = oldRR;
      if (!RunningRules && !RunningAsyncRules)
        _target.AllRulesComplete();
      return affectedProperties.Distinct().ToList();
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
      var oldRR = RunningRules;
      RunningRules = true;

      var affectedProperties = new List<string>();
      affectedProperties.AddRange(CheckRulesForProperty(property, true));

      RunningRules = oldRR;
      if (!RunningRules && !RunningAsyncRules)
        _target.AllRulesComplete();
      return affectedProperties.Distinct().ToList();
    }

    /// <summary>
    /// Invokes all rules for a specific property.
    /// </summary>
    private List<string> CheckRulesForProperty(Csla.Core.IPropertyInfo property, bool cascade)
    {
      var rules = from r in TypeRules.Rules
                  where ReferenceEquals(r.PrimaryProperty, property)
                  orderby r.Priority
                  select r;
      var affectedProperties = new List<string> { property.Name };
      BrokenRules.ClearRules(property);
      affectedProperties.AddRange(RunRules(rules));

      if (cascade)
      {
        // get properties affected by all rules
        var propertiesToRun = new List<Csla.Core.IPropertyInfo>();
        foreach (var item in rules)
          foreach (var p in item.AffectedProperties)
            if (!ReferenceEquals(property, p))
              propertiesToRun.Add(p);
        // run rules for affected properties
        foreach (var item in propertiesToRun.Distinct())
          CheckRulesForProperty(item, false);
      }

      return affectedProperties.Distinct().ToList();
    }

    [NonSerialized]
    private List<Csla.Core.IPropertyInfo> _busyProperties;
    private List<Csla.Core.IPropertyInfo> BusyProperties
    {
      get
      {
        if (_busyProperties == null)
          _busyProperties = new List<Csla.Core.IPropertyInfo>();
        return _busyProperties;
      }
    }

    private List<string> RunRules(IEnumerable<IBusinessRule> rules)
    {
      var affectedProperties = new List<string>();
      bool anyRuleBroken = false;
      foreach (var rule in rules)
      {
        // implicit short-circuiting
        if (anyRuleBroken && rule.Priority > ProcessThroughPriority)
          break;
        bool complete = false;
        // set up context
        var context = new RuleContext((r) =>
          {
            if (rule.IsAsync)
            {
              lock (SyncRoot)
              {
                // update output values
                if (r.OutputPropertyValues != null)
                  foreach (var item in r.OutputPropertyValues)
                    ((IManageProperties)_target).LoadProperty(item.Key, item.Value);
                // update broken rules list
                if (r.Results != null)
                  foreach (var result in r.Results)
                  {
                    BrokenRules.SetBrokenRule(result);
                    if (!rule.IsAsync && !result.Success && result.Severity == RuleSeverity.Error)
                      anyRuleBroken = true;
                  }
                // mark each property as not busy
                foreach (var item in r.Rule.AffectedProperties)
                {
                  BusyProperties.Remove(item);
                  if (!BusyProperties.Contains(item))
                    _target.RuleComplete(item);
                }
                if (!RunningRules && !RunningAsyncRules)
                  _target.AllRulesComplete();
              }
            }
            else
            {
              // update output values
              if (r.OutputPropertyValues != null)
                foreach (var item in r.OutputPropertyValues)
                  ((IManageProperties)_target).LoadProperty(item.Key, item.Value);
              // update broken rules list
              if (r.Results != null)
                foreach (var result in r.Results)
                {
                  BrokenRules.SetBrokenRule(result);
                  if (!rule.IsAsync && !result.Success && result.Severity == RuleSeverity.Error)
                    anyRuleBroken = true;
                }
              complete = true;
            }
          });
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

        // mark properties busy
        if (rule.IsAsync)
        {
          lock (SyncRoot)
          {
            // mark each property as busy
            foreach (var item in rule.AffectedProperties)
            {
              if (!BusyProperties.Contains(item))
                _target.RuleStart(item);
              BusyProperties.Add(item);
            }
          }
        }

        // execute (or start executing) rule
        try
        {
          rule.Execute(context);
        }
        catch (Exception ex)
        {
          context.AddErrorResult(ex.Message);
          if (rule.IsAsync)
            context.Complete();
        }

        if (!rule.IsAsync)
        {
          // process results
          if (!complete)
            context.Complete();
          // copy affected property names
          affectedProperties.AddRange(rule.AffectedProperties.Select(c => c.Name));
          // copy output property names
          if (context.OutputPropertyValues != null)
            foreach (var item in context.OutputPropertyValues)
              affectedProperties.Add(item.Key.Name);
          if (context.Results != null)
          {
            // explicit short-circuiting
            if ((from r in context.Results
                        where r.StopProcessing == true
                        select r).Count() > 0)
              break;
          }
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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

#if SILVERLIGHT
    #region IUndoableObject Members

    private Stack<SerializationInfo> _stateStack = new Stack<SerializationInfo>();

    int IUndoableObject.EditLevel
    {
      get { return _stateStack.Count; }
    }

    void IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (((IUndoableObject)this).EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "CopyState"));

      SerializationInfo state = new SerializationInfo(0);
      OnGetState(state, StateMode.Undo);

      if (_brokenRules != null && _brokenRules.Count > 0)
      {
        byte[] rules = MobileFormatter.Serialize(_brokenRules);
        state.AddValue("_rules", Convert.ToBase64String(rules));
      }

      _stateStack.Push(state);
    }

    void IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (((IUndoableObject)this).EditLevel > 0)
      {
        if (((IUndoableObject)this).EditLevel - 1 < parentEditLevel)
          throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "UndoChanges"));

        SerializationInfo state = _stateStack.Pop();
        OnSetState(state, StateMode.Undo);

        lock (SyncRoot)
          _brokenRules = null;

        if (state.Values.ContainsKey("_rules"))
        {
          byte[] rules = Convert.FromBase64String(state.GetValue<string>("_rules"));

          lock (SyncRoot)
            _brokenRules = (BrokenRulesCollection)MobileFormatter.Deserialize(rules);
        }
      }
    }

    void IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (((IUndoableObject)this).EditLevel - 1 < parentEditLevel)
        throw new UndoException(string.Format(Properties.Resources.EditLevelMismatchException, "AcceptChanges"));

      if (((IUndoableObject)this).EditLevel > 0)
      {
        // discard latest recorded state
        _stateStack.Pop();
      }
    }

    #endregion
#endif

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
      info.AddValue("_ruleSet", _ruleSet);
#if SILVERLIGHT
      if (mode == StateMode.Serialization)
      {
        if (_stateStack.Count > 0)
        {
          MobileList<SerializationInfo> list = new MobileList<SerializationInfo>(_stateStack.ToArray());
          byte[] xml = MobileFormatter.Serialize(list);
          info.AddValue("_stateStack", xml);
        }
      }
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
      _ruleSet = info.GetValue<string>("_ruleSet");
#if SILVERLIGHT
      if (mode == StateMode.Serialization)
      {
        _stateStack.Clear();

        if (info.Values.ContainsKey("_stateStack"))
        {
          byte[] xml = info.GetValue<byte[]>("_stateStack");
          MobileList<SerializationInfo> list = (MobileList<SerializationInfo>)MobileFormatter.Deserialize(xml);
          SerializationInfo[] layers = list.ToArray();
          Array.Reverse(layers);
          foreach (SerializationInfo layer in layers)
            _stateStack.Push(layer);
        }
      }
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
    #endregion
  }
}
