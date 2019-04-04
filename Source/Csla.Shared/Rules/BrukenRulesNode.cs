//-----------------------------------------------------------------------
// <copyright file="BusinessRulesNode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Holds Node properties for BrokenRulesTree.</summary>
//-----------------------------------------------------------------------using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Holds broken rules for an Node in the BrokenRulesTree.
  /// </summary>
  public class BrokenRulesNode
  {
    /// <summary>
    /// Gets the parent key. Root key is null.
    /// </summary>
    /// <value>The parent.</value>
    public object Parent { get; internal set; }

    /// <summary>
    /// Gets the node key.
    /// </summary>
    /// <value>The node.</value>
    public object Node { get; internal set; }

    /// <summary>
    /// Gets the broken rules for this node.
    /// </summary>
    /// <value>The node broken rules.</value>
    public BrokenRulesCollection BrokenRules { get; internal set; }

    /// <summary>
    /// Gets the Business Object.
    /// </summary>
    /// <value>The object.</value>
    public object Object { get; internal set; }
  }
}
