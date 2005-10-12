using System;

namespace Csla.Validation
{

    /// <summary>
    /// Object providing extra information to methods that
    /// implement business rules.
    /// </summary>
    public class RuleArgs
    {
        private string _propertyName;
        private string _description;

        /// <summary>
        /// The name of the property to be validated.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Set by the rule handler method to describe the broken
        /// rule.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Creates an instance of RuleArgs.
        /// </summary>
        /// <param name="propertyName">The name of the property to be validated.</param>
        public RuleArgs(string propertyName)
        {
            _propertyName = propertyName;
        }
    }
}
