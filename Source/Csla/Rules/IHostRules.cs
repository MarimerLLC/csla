using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  public interface IHostRules
  {
    void RuleStart(Csla.Core.IPropertyInfo property);
    void RuleComplete(Csla.Core.IPropertyInfo property);
    void AllRulesComplete();
  }
}
