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

    // reference to current business object
    [NonSerialized]
    private IHostRules _target;
    // reference to per-type rules manager for this object
    [NonSerialized]
    private BusinessRuleManager _typeRules;
    // async rules currently executing
    [NonSerialized]
    private ObservableCollection<RuleContext> _validatingRules;

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

    private BusinessRuleManager TypeRules
    {
      get
      {
        if (_typeRules == null && _target != null)
          _typeRules = BusinessRuleManager.GetRulesForType(_target.GetType());
        return _typeRules;
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
      foreach (var item in TypeRules.RuleMethods)
        result.Add(item.RuleName);
      return result.ToArray();
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
    {
      _brokenRules = new BrokenRulesCollection(true);
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="target">Target business object.</param>
    public BusinessRules(IHostRules target)
      : this()
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
    /// Gets a value indicating whether there are
    /// any currently broken rules, which would
    /// mean the object is not valid.
    /// </summary>
    public bool IsValid
    {
      get { return _brokenRules.ErrorCount == 0; }
    }

    /// <summary>
    /// Gets the broken rules list.
    /// </summary>
    public BrokenRulesCollection GetBrokenRules()
    {
      return _brokenRules;
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
      if (BusyProperties.Count == 0)
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
      RunningRules = true;
      var rules = from r in TypeRules.RuleMethods
                  where r.PrimaryProperty == null
                  orderby r.Priority
                  select r;
      _brokenRules.ClearRules(null);
      var affectedProperties = RunRules(rules);
      RunningRules = false;
      if (BusyProperties.Count == 0)
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
      RunningRules = true;
      var rules = from r in TypeRules.RuleMethods
                  where ReferenceEquals(r.PrimaryProperty, property)
                  orderby r.Priority
                  select r;
      var affectedProperties = new List<string> { property.Name };
      _brokenRules.ClearRules(property);
      affectedProperties.AddRange(RunRules(rules));
      RunningRules = false;
      if (BusyProperties.Count == 0)
        _target.AllRulesComplete();
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
                // update broken rules list
                if (r.Results != null)
                  foreach (var result in r.Results)
                  {
                    _brokenRules.SetBrokenRule(result);
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
                if (!RunningRules && BusyProperties.Count == 0)
                  _target.AllRulesComplete();
              }
            }
            else
            {
              // update broken rules list
              if (r.Results != null)
                foreach (var result in r.Results)
                {
                  _brokenRules.SetBrokenRule(result);
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

        // execute (or start executing) rule
        try
        {
          rule.Execute(context);
          if (rule.IsAsync)
          {
            // mark each property as busy
            foreach (var item in rule.AffectedProperties)
            {
              lock (SyncRoot)
              {
                if (!BusyProperties.Contains(item))
                  _target.RuleStart(item);
                BusyProperties.Add(rule.PrimaryProperty);
              }
            }
          }
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
          foreach (var item in rule.AffectedProperties)
            affectedProperties.Add(item.Name);
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
          MobileList<SerializationInfo> list = new MobileList<SerializationInfo>(_stateStack.ToArray());
          byte[] xml = MobileFormatter.Serialize(list);
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
          MobileList<SerializationInfo> list = (MobileList<SerializationInfo>)MobileFormatter.Deserialize(xml);
          SerializationInfo[] layers = list.ToArray();
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
