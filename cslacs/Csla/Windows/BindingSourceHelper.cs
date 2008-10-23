using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Csla.Windows
{
  public static class BindingSourceHelper
  {
    private const string STR_BindingSourceNotProvided = "A root binding source has not been provided.";
    
    private static BindingSourceNode _rootSourceNode;

    public static BindingSourceNode InitializeBindingSourceTree(
      IContainer container, BindingSource rootSource)
    {
      if (rootSource == null)
        throw new ApplicationException(STR_BindingSourceNotProvided);

      _rootSourceNode = new BindingSourceNode(rootSource);
      _rootSourceNode.Children.AddRange(GetChildBindingSources(container, rootSource, _rootSourceNode));

      return _rootSourceNode;
    }

    private static List<BindingSourceNode> GetChildBindingSources(
      IContainer container, BindingSource parent, BindingSourceNode parentNode)
    {
      List<BindingSourceNode> children = new List<BindingSourceNode>();

      foreach (Component component in container.Components)
      {
        if (component is BindingSource)
        {
          BindingSource temp = component as BindingSource;
          if (temp.DataSource != null && temp.DataSource.Equals(parent))
          {
            BindingSourceNode childNode = new BindingSourceNode(temp);
            children.Add(childNode);
            childNode.Children.AddRange(GetChildBindingSources(container, temp, childNode));
            childNode.Parent = parentNode;
          }
        }
      }

      return children;
    }

  }
}
