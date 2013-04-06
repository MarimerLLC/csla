using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

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

    /// <summary>
    /// Binds a business object to the BindingSource.
    /// </summary>
    /// <param name="objectToBind">
    /// Business object.
    /// </param>
    public void Bind(object objectToBind)
    {
      Csla.Core.ISupportUndo root = objectToBind as Csla.Core.ISupportUndo;

      if (root != null)
        root.BeginEdit();

      _Source.DataSource = objectToBind;
      SetEvents(true);
      ResetBindings(false);
    }

    /// <summary>
    /// Applies changes to the business object.
    /// </summary>
    public void Apply()
    {
      SetEvents(false);

      Csla.Core.ISupportUndo root = _Source.DataSource as Csla.Core.ISupportUndo;

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

      Csla.Core.ISupportUndo root = _Source.DataSource as Csla.Core.ISupportUndo;

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
