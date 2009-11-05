using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
    public class MaxLengthArgs : Csla.Validation.RuleArgs
    {
        private int _max;

        public int MaxLength
        {
            get { return _max; }
        }

        public MaxLengthArgs(string propertyName, int maxLength) : base(propertyName)
        {
            _max = maxLength;
        }
    }
}
