using System;
using System.Windows.Forms;

namespace PTWin
{
	/// <summary>
	/// Utilities to assist with data binding.
	/// </summary>
	public class Util
	{
    /// <summary>
    /// Binds a control to a data source property, making sure
    /// that a previous binding doesn't already exist.
    /// </summary>
    public static void BindField(Control control, string propertyName, 
                                 object dataSource, string dataMember)
    {
      Binding bd;

      for(int index = control.DataBindings.Count - 1; index >= 0; index--)
      {
        bd = control.DataBindings[index];
        if(bd.PropertyName == propertyName)
          control.DataBindings.Remove(bd);
      }
      control.DataBindings.Add(propertyName, dataSource, dataMember);
    }
	}
}
