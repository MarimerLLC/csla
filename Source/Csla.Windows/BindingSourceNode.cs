//-----------------------------------------------------------------------
// <copyright file="BindingSourceNode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains a reference to a BindingSource object</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Windows
{
  /// <summary>
  /// Maintains a reference to a BindingSource object
  /// on the form.
  /// </summary>
  public class BindingSourceNode
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="source">
    /// BindingSource object to be mananaged.
    /// </param>
    public BindingSourceNode(BindingSource source)
    {
      Source = source;
      Source.CurrentChanged += BindingSource_CurrentChanged;
    }

    private List<BindingSourceNode> _children;

    private void BindingSource_CurrentChanged(object sender, EventArgs e)
    {
      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.Source.EndEdit();
    }

    internal BindingSource Source { get; }

    internal List<BindingSourceNode> Children
    {
      get
      {
        if (_children == null)
          _children = new List<BindingSourceNode>();

        return _children;
      }
    }

    internal BindingSourceNode Parent { get; set; }

    internal void Unbind(bool cancel)
    {
      if (Source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.Unbind(cancel);

      IEditableObject current = Source.Current as IEditableObject;

      if (!(Source.DataSource is BindingSource))
        Source.DataSource = null;

      if (current != null)
      {
        if (cancel)
          current.CancelEdit();
        else
          current.EndEdit();
      }

      if (Source.DataSource is BindingSource)
        Source.DataSource = Parent.Source;
    }

    internal void EndEdit()
    {
      if (Source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.EndEdit();

      Source.EndEdit();
    }

    internal void SetEvents(bool value)
    {
      if (Source == null)
        return;

      Source.RaiseListChangedEvents = value;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.SetEvents(value);
    }

    internal void ResetBindings(bool refreshMetadata)
    {
      if (Source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.ResetBindings(refreshMetadata);

      Source.ResetBindings(refreshMetadata);
    }

    /// <summary>
    /// Binds a business object to the BindingSource.
    /// </summary>
    /// <param name="objectToBind">
    /// Business object.
    /// </param>
    public void Bind(object objectToBind)
    {
      if (objectToBind is ISupportUndo root)
        root.BeginEdit();

      Source.DataSource = objectToBind;
      SetEvents(true);
      ResetBindings(false);
    }

    /// <summary>
    /// Applies changes to the business object.
    /// </summary>
    public void Apply()
    {
      SetEvents(false);

      ISupportUndo root = Source.DataSource as ISupportUndo;

      Unbind(false);
      EndEdit();

      root?.ApplyEdit();
    }

    /// <summary>
    /// Cancels changes to the business object.
    /// </summary>
    /// <param name="businessObject"></param>
    public void Cancel(object businessObject)
    {
      SetEvents(false);

      ISupportUndo root = Source.DataSource as ISupportUndo;

      Unbind(true);

      root?.CancelEdit();

      Bind(businessObject);
    }

    /// <summary>
    /// Disconnects from the BindingSource object.
    /// </summary>
    public void Close()
    {
      SetEvents(false);
      Unbind(true);
    }

  }
}
