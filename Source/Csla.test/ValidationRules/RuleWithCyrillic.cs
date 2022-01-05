using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  public class ИзилдрRule : Csla.Rules.BusinessRule
  {
    public ИзилдрRule(IPropertyInfo property) : base(property)
    {}
  }
}
