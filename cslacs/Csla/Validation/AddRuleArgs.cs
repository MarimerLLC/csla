using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  public class AddRuleArgs : EventArgs
  {
    public object BusinessObject { get; set; }
    public object Attribute { get; set; }
    public System.Reflection.PropertyInfo PropertyInfo { get; set; }
    public bool RuleAdded { get; set; }
  }
}
