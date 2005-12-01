using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

// code from Bill McCarthy
// http://msmvps.com/bill/archive/2005/10/05/69012.aspx
// used with permission

namespace Csla.Windows
{
  [DesignerCategory("")]
  [ProvideProperty("ReadValuesOnChange", typeof(BindingSource))]
  public class BindingSourceRefresh : Component, IExtenderProvider
  {

    private Dictionary<BindingSource, bool> _sources = new Dictionary<BindingSource, bool>();

    public BindingSourceRefresh(IContainer container)
    {
      container.Add(this);
    }

    public bool CanExtend(object extendee)
    {
      if (extendee is BindingSource)
        return true;
      return false;
    }

    public bool GetReadValuesOnChange(BindingSource source)
    {
      if (_sources.ContainsKey(source))
        return _sources[source];
      return false;
    }

    public void SetReadValuesOnChange(BindingSource source, bool value)
    {
      if (_sources.ContainsKey(source))
        _sources[source] = value;
      else
        _sources.Add(source, value);
      if (value)
      {
        // hook
        source.BindingComplete += new BindingCompleteEventHandler(Source_BindingComplete);
      }
      else
      {
        // unhook
        source.BindingComplete -= new BindingCompleteEventHandler(Source_BindingComplete);
      }

    }

    private void Source_BindingComplete(object sender, BindingCompleteEventArgs e)
    {
      e.Binding.ReadValue();
    }

  }
}
