using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Csla.Windows
{
  public class BindingSourceNode
  {
    public BindingSourceNode(BindingSource source)
    {
      _Source = source;
      _Source.CurrentChanged += BindingSource_CurrentChanged;
    }

    private BindingSource _Source;
    private List<BindingSourceNode> _Children;
    private BindingSourceNode _Parent;

    void BindingSource_CurrentChanged(object sender, EventArgs e)
    {
      if (_Children.Count > 0)
        foreach (BindingSourceNode child in _Children)
          child.Source.EndEdit();
    }

    internal BindingSource Source
    {
      get { return _Source; }
    }

    internal List<BindingSourceNode> Children
    {
      get
      {
        if (_Children == null)
          _Children = new List<BindingSourceNode>();

        return _Children;
      }
    }

    internal BindingSourceNode Parent
    {
      get { return _Parent; }
      set { _Parent = value; }
    }

    internal void Unbind(bool cancel)
    {
      if (_Source == null)
        return;

      if (_Children.Count > 0)
        foreach (BindingSourceNode child in _Children)
          child.Unbind(cancel);

      IEditableObject current = _Source.Current as IEditableObject;

      if (!(_Source.DataSource is BindingSource))
        _Source.DataSource = null;

      if (current != null)
      {
        if (cancel)
          current.CancelEdit();
        else
          current.EndEdit();
      }

      if (_Source.DataSource is BindingSource)
        _Source.DataSource = _Parent.Source;
    }

    internal void EndEdit()
    {
      if (Source == null)
        return;

      if (_Children.Count > 0)
        foreach (BindingSourceNode child in _Children)
          child.EndEdit();

      _Source.EndEdit();
    }

    internal void SetEvents(bool value)
    {
      if (_Source == null)
        return;

      _Source.RaiseListChangedEvents = value;

      if (_Children.Count > 0)
        foreach (BindingSourceNode child in _Children)
          child.SetEvents(value);
    }

    internal void ResetBindings(bool refreshMetadata)
    {
      if (_Source == null)
        return;

      if (_Children.Count > 0)
        foreach (BindingSourceNode child in _Children)
          child.ResetBindings(refreshMetadata);

      _Source.ResetBindings(refreshMetadata);
    }

    public void Bind(object objectToBind)
    {
      Csla.Core.ISupportUndo root = objectToBind as Csla.Core.ISupportUndo;

      if (root != null)
        root.BeginEdit();

      _Source.DataSource = objectToBind;
      SetEvents(true);
      ResetBindings(false);
    }

    public void Apply()
    {
      SetEvents(false);

      Csla.Core.ISupportUndo root = _Source.DataSource as Csla.Core.ISupportUndo;

      Unbind(false);
      EndEdit();

      if (root != null)
        root.ApplyEdit();
    }

    public void Cancel(object businessObject)
    {
      SetEvents(false);

      Csla.Core.ISupportUndo root = _Source.DataSource as Csla.Core.ISupportUndo;

      Unbind(true);

      if (root != null)
        root.CancelEdit();

      Bind(businessObject);
    }

    public void Close()
    {
      SetEvents(false);
      Unbind(true);
    }

  }
}
