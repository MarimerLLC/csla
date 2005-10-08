using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Validation
{
    /// <summary>
    /// Stores details about a specific broken business rule.
    /// </summary>
    [Serializable()]
    public class BrokenRule
    {
        private string _ruleName;
        private string _description;
        private string _property;

        internal BrokenRule(string ruleName, string description, string property)
        {
            _ruleName = ruleName;
            _description = description;
            _property = property;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string RuleName
        {
            get { return _ruleName; }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string Description
        {
            get { return _description; }
            set
            {
                // the property must be read-write for Web Forms data binding
                // to work, but we really don't want to allow the value to be
                // changed dynamically so we ignore any attempt to set it
            }
        }

        /// <summary>
        /// Provides access to the property affected by the broken rule.
        /// </summary>
        /// <remarks>
        /// This value is actually readonly, not readwrite. Any new
        /// value set into this property is ignored. The property is only
        /// readwrite because that is required to support data binding
        /// within Web Forms.
        /// </remarks>
        /// <value>The property affected by the rule.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        public string Property
        {
            get { return _property; }
            set
            {
                // the property must be read-write for Web Forms data binding
                // to work, but we really don't want to allow the value to be
                // changed dynamically so we ignore any attempt to set it
            }
        }
    }
}