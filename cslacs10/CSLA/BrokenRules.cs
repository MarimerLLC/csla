using System;

namespace CSLA
{
  /// <summary>
  /// Tracks the business rules broken within a business object.
  /// </summary>
  [Serializable()]
  public class BrokenRules
	{
    #region Rule structure

    /// <summary>
    /// Stores details about a specific broken business rule.
    /// </summary>
    [Serializable()]
      public struct Rule
    {
      string _name;
      string _description;

      internal Rule(string name, string description)
      {
        _name = name;
        _description = description;
      }

      /// <summary>
      /// Provides access to the name of the broken rule.
      /// </summary>
      /// <remarks>
      /// This value is actually readonly, not readwrite. Any new
      /// value set into this property is ignored. The property is only
      /// readwrite because that is required to support data binding
      /// within Web Forms.
      /// </remarks>
      /// <value>The name of the rule.</value>
      public string Name
      {
        get
        {
          return _name;
        }
        set
        {
          // the property must be read-write for Web Forms data binding
          // to work, but we really don't want to allow the value to be
          // changed dynamically so we ignore any attempt to set it
        }
      }

      /// <summary>
      /// Provides access to the description of the broken rule.
      /// </summary>
      /// <remarks>
      /// This value is actually readonly, not readwrite. Any new
      /// value set into this property is ignored. The property is only
      /// readwrite because that is required to support data binding
      /// within Web Forms.
      /// </remarks>
      /// <value>The description of the rule.</value>
      public string Description
      {
        get
        {
          return _description;
        }
        set
        {
          // the property must be read-write for Web Forms data binding
          // to work, but we really don't want to allow the value to be
          // changed dynamically so we ignore any attempt to set it
        }
      }
    }

    #endregion

    #region RulesCollection

    /// <summary>
    /// A collection of currently broken rules.
    /// </summary>
    /// <remarks>
    /// This collection is readonly and can be safely made available
    /// to code outside the business object such as the UI. This allows
    /// external code, such as a UI, to display the list of broken rules
    /// to the user.
    /// </remarks>
    [Serializable()]
    public class RulesCollection : CSLA.Core.BindableCollectionBase
    {
      bool _legal = false;

      /// <summary>
      /// Returns a <see cref="T:CSLA.BrokenRules.Rule" /> object
      /// containing details about a specific broken business rule.
      /// </summary>
      /// <param name="Index"></param>
      /// <returns></returns>
      public Rule this [int index]
      {
        get 
        { 
          return (Rule)List[index]; 
        }
      }

      internal RulesCollection()
      {
        AllowEdit = false;
        AllowRemove = false;
        AllowNew = false;
      }

      internal void Add(string name, string description)
      {
        Remove(name);
        _legal = true;
        List.Add(new Rule(name, description));
        _legal = false;
      }

      internal void Remove(string name)
      {
        int index;

        // we loop through using a numeric counter because
        // the base class Remove requires a numberic index
        _legal = true;
        for(index = 0; index < List.Count; index++)
          if(((Rule)List[index]).Name == name)
          {
            List.Remove(List[index]);
            break;
          }
        _legal = false;
      }

      internal bool Contains(string name)
      {
        int index;

        for(index = 0; index < List.Count; index++)
          if(((Rule)List[index]).Name == name)
            return true;
        return false;
      }

      protected override void OnClear()
      {
        if(!_legal)
          throw new NotSupportedException("Clear is an invalid operation");
      }

      protected override void OnInsert(int index, object val)
      {
        if(!_legal)
          throw new NotSupportedException("Insert is an invalid operation");
      }

      protected override void OnRemove(int index, object val)
      {
        if(!_legal)
          throw new NotSupportedException("Remove is an invalid operation");
      }

      protected override void OnSet(int index, object oldValue, object newValue)
      {
        if(!_legal)
          throw new NotSupportedException(
                                  "Changing an element is an invalid operation");
      }
    }

    #endregion
    
    RulesCollection _rules = new RulesCollection();

    /// <summary>
    /// This method is called by business logic within a business class to
    /// indicate whether a business rule is broken.
    /// </summary>
    /// <remarks>
    /// Rules are identified by their names. The description field is merely a 
    /// comment that is used for display to the end user. When a rule is marked as
    /// broken, it is recorded under the rule name value. To mark the rule as not
    /// broken, the same rule name must be used.
    /// </remarks>
    /// <param name="Rule">The name of the business rule.</param>
    /// <param name="Description">The description of the business rule.</param>
    /// <param name="IsBroken">True if the value is broken, False if it is not broken.</param>
    public void Assert(string name, string description, bool isBroken)
    {
      if(isBroken)
        _rules.Add(name, description);
      else
        _rules.Remove(name);
    }

    /// <summary>
    /// Returns a value indicating whether there are any broken rules
    /// at this time. If there are broken rules, the business object
    /// is assumed to be invalid and False is returned. If there are no
    /// broken business rules True is returned.
    /// </summary>
    /// <returns>A value indicating whether any rules are broken.</returns>
    public bool IsValid
    {
      get
      {
        return (_rules.Count == 0);
      }
    }

    /// <summary>
    /// Returns a value indicating whether a particular business rule
    /// is currently broken.
    /// </summary>
    /// <param name="Rule">The name of the rule to check.</param>
    /// <returns>A value indicating whether the rule is currently broken.</returns>
    public bool IsBroken(string name)
    {
      return _rules.Contains(name);
    }

    /// <summary>
    /// Returns a reference to the readonly collection of broken
    /// business rules.
    /// </summary>
    /// <remarks>
    /// The reference returned points to the actual collection object.
    /// This means that as rules are marked broken or unbroken over time,
    /// the underlying data will change. Because of this, the UI developer
    /// can bind a display directly to this collection to get a dynamic
    /// display of the broken rules at all times.
    /// </remarks>
    /// <returns>A reference to the collection of broken rules.</returns>
    public RulesCollection GetBrokenRules() 
    {
      return _rules;
    }

    /// <summary>
    /// Returns the text of all broken rule descriptions, each
    /// separated by cr/lf.
    /// </summary>
    /// <returns>The text of all broken rule descriptions.</returns>
    public override string ToString()
    {
      System.Text.StringBuilder obj = new System.Text.StringBuilder();
      bool first = true;

      foreach(Rule item in _rules)
      {
        if(first)
          first = false;
        else
          obj.AppendFormat("/n");
        obj.Append(item.Description);
      }
      return obj.ToString();
    }
	}
}
