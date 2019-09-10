using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace MultipleBindingSources
{
  public static class BindingSourceExtensions
  {
    /// <summary>
    /// Unbinds the binding source and the Data object. Use this Method to safely disconnect the data object from a BindingSource before saving data.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="cancel">if set to <c>true</c> then call CancelEdit else call EndEdit.</param>
    /// <param name="isRoot">if set to <c>true</c> this BindingSource contains the Root object. Set to <c>false</c> for nested BindingSources</param>
    public static void UnbindDataSource(this BindingSource source, bool cancel, bool isRoot)
    {
      IEditableObject current = null;
      // position may be -1 if bindigsource is already unbound which results in Exception when trying to address current
      if ((source.DataSource != null) && (source.Position > -1)) {
        current = source.Current as IEditableObject;
      }

      // set Raise list changed to True
      source.RaiseListChangedEvents = false;
      // tell currency manager to suspend binding
      source.SuspendBinding();

      if (isRoot) source.DataSource = null;
      if (current == null) return;

      if (cancel)
      {
        current.CancelEdit();
      }
      else
      {
        current.EndEdit();
      }
    }

    /// <summary>
    /// Binds the BindingSource to data object .
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="data">The data.</param>
    public static void BindDataSource(this BindingSource source, object data)
    {
      BindDataSource(source, data, false);
    }


    /// <summary>
    /// Binds the BindingSource to data object.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="data">The data.</param>
    /// <param name="metadataChanged">if set to <c>true</c> then metadata (ovject/list type) was changed.</param>
    public static void BindDataSource(this BindingSource source, object data, bool metadataChanged)
    {
      if (data != null)
      {
        source.DataSource = data;
      }

      // set Raise list changed to True
      source.RaiseListChangedEvents = true;
      // tell currency manager to resume binding 
      source.ResumeBinding();
      // Notify UI controls that the dataobject/list was reset - and if metadata was changed 
      source.ResetBindings(metadataChanged);
    }
  }
}
