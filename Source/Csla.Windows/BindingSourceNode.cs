//-----------------------------------------------------------------------
// <copyright file="BindingSourceNode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains a reference to a BindingSource object</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
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
      _source = source;
      _source.CurrentChanged += BindingSource_CurrentChanged;
    }

    private BindingSource _source;
    private List<BindingSourceNode> _children;
    private BindingSourceNode _parent;

    private void BindingSource_CurrentChanged(object sender, EventArgs e)
    {
      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.Source.EndEdit();
    }

    internal BindingSource Source
    {
      get { return _source; }
    }

    internal List<BindingSourceNode> Children
    {
      get
      {
        if (_children == null)
          _children = new List<BindingSourceNode>();

        return _children;
      }
    }

    internal BindingSourceNode Parent
    {
      get { return _parent; }
      set { _parent = value; }
    }

    internal void Unbind(bool cancel)
    {
      if (_source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.Unbind(cancel);

      IEditableObject current = _source.Current as IEditableObject;

      if (!(_source.DataSource is BindingSource))
        _source.DataSource = null;

      if (current != null)
      {
        if (cancel)
          current.CancelEdit();
        else
          current.EndEdit();
      }

      if (_source.DataSource is BindingSource)
        _source.DataSource = _parent.Source;
    }

    internal void EndEdit()
    {
      if (Source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.EndEdit();

      _source.EndEdit();
    }

    internal void SetEvents(bool value)
    {
      if (_source == null)
        return;

      _source.RaiseListChangedEvents = value;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.SetEvents(value);
    }

    internal void ResetBindings(bool refreshMetadata)
    {
      if (_source == null)
        return;

      if (_children.Count > 0)
        foreach (BindingSourceNode child in _children)
          child.ResetBindings(refreshMetadata);

      _source.ResetBindings(refreshMetadata);
    }

    /// <summary>
    /// Binds a business object to the BindingSource.
    /// </summary>
    /// <param name="objectToBind">
    /// Business object.
    /// </param>
    public void Bind(object objectToBind)
    {
      ISupportUndo root = objectToBind as ISupportUndo;

      if (root != null)
        root.BeginEdit();

      _source.DataSource = objectToBind;
      SetEvents(true);
      ResetBindings(false);
    }

    /// <summary>
    /// Applies changes to the business object.
    /// </summary>
    public void Apply()
    {
      SetEvents(false);

      ISupportUndo root = _source.DataSource as ISupportUndo;

      Unbind(false);
      EndEdit();

      if (root != null)
        root.ApplyEdit();
    }

    /// <summary>
    /// Cancels changes to the business object.
    /// </summary>
    /// <param name="businessObject"></param>
    public void Cancel(object businessObject)
    {
      SetEvents(false);

      ISupportUndo root = _source.DataSource as ISupportUndo;

      Unbind(true);

      if (root != null)
        root.CancelEdit();

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
