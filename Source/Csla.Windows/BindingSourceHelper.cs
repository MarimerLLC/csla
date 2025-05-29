//-----------------------------------------------------------------------
// <copyright file="BindingSourceHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper methods for dealing with BindingSource</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Properties;

namespace Csla.Windows;

/// <summary>
/// Helper methods for dealing with BindingSource
/// objects and data binding.
/// </summary>
public static class BindingSourceHelper
{
  private static BindingSourceNode _rootSourceNode = default!;

  /// <summary>
  /// Sets up BindingSourceNode objects for all
  /// BindingSource objects related to the provided
  /// root source.
  /// </summary>
  /// <param name="container">
  /// Container for the components.
  /// </param>
  /// <param name="rootSource">
  /// Root BindingSource object.
  /// </param>
  /// <exception cref="ArgumentNullException"><paramref name="rootSource"/> or <paramref name="container"/> is <see langword="null"/>.</exception>
  public static BindingSourceNode InitializeBindingSourceTree(IContainer container, BindingSource rootSource)
  {
    if (container is null)
      throw new ArgumentNullException(nameof(container));
    if (rootSource == null)
      throw new ApplicationException(Resources.BindingSourceNotProvided);

    _rootSourceNode = new BindingSourceNode(rootSource);
    _rootSourceNode.Children.AddRange(GetChildBindingSources(container, rootSource, _rootSourceNode));

    return _rootSourceNode;
  }

  private static IEnumerable<BindingSourceNode> GetChildBindingSources(IContainer container, BindingSource parent, BindingSourceNode parentNode)
  {
    foreach (Component component in container.Components)
    {
      if (component is BindingSource { DataSource: not null } source && source.DataSource.Equals(parent))
      {
        var childNode = new BindingSourceNode(source)
        {
          Parent = parentNode
        };
        childNode.Children.AddRange(GetChildBindingSources(container, source, childNode));
        yield return childNode;
      }
    }
  }
}