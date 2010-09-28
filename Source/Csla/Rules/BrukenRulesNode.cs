using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  // Holds roken rules for an Niode in the tree.
  /// </summary>
  public class BrukenRulesNode 
  {
    public object Parent { get; internal set; }
    public object Node { get; internal set; }
    public BrokenRulesCollection NodeBrukenRules { get; internal set; }
  }
}
