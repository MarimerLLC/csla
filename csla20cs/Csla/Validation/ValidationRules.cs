using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Validation
{
    public class ValidationRules
    {
        public ValidationRules(object businessObject) { }
        public void CheckRules(string propertyName) { }
        internal bool IsValid
        {
            get { return true; }
        }
        public BrokenRulesCollection GetBrokenRules()
        {
            return null;
        }
    }
}
