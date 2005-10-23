using System;

namespace Csla.Validation
{
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
    public class BrokenRulesCollection : Core.ReadOnlyBindingList<BrokenRule>
    {

        /// <summary>
        /// Returns the first <see cref="T:Csla.BrokenRules.BrokenRule" /> object
        /// corresponding to the specified property.
        /// </summary>
        /// <remarks>
        /// Code in a business object or UI can also use this value to retrieve
        /// the first broken rule in <see cref="T:Csla.BrokenRules" /> that corresponds
        /// to a specfic Property on the object.
        /// </remarks>
        /// <param name="property">The name of the property affected by the rule.</param>
        /// <returns>
        /// The first BrokenRule object corresponding to the specified property, or Nothing if 
        /// there are no rules defined for the property.
        /// </returns>
        public BrokenRule GetFirstBrokenRule(string property)
        {
            foreach (BrokenRule item in this)
                if (item.Property == property)
                    return item;
            return null;
        }

        internal BrokenRulesCollection()
        {
            // limit creation to this assembly
        }

        internal void Add(string ruleName, string description, string property)
        {
            Remove(ruleName);
            IsReadOnly = false;
            Add(new BrokenRule(ruleName, description, property));
            IsReadOnly = true;
        }

        internal void Remove(string ruleName)
        {
            // we loop through using a numeric counter because
            // the base class Remove requires a numeric index
            IsReadOnly = false;
            for (int index = 0; index < Count; index++)
            {
                if (this[index].RuleName == ruleName)
                {
                    RemoveAt(index);
                    break;
                }
            }
            IsReadOnly = true;
        }

        /// <summary>
        /// Returns the text of all broken rule descriptions, each
        /// separated by a NewLine.
        /// </summary>
        /// <returns>The text of all broken rule descriptions.</returns>
        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            bool first = true;
            foreach (BrokenRule item in this)
            {
                if (first)
                    first = false;
                else
                    result.Append(Environment.NewLine);
                result.Append(item.Description);
            }
            return result.ToString();
        }
    }
}