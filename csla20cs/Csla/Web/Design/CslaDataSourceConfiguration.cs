using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Csla.Web.Design
{
  /// <summary>
  /// CslaDataSource configuration form.
  /// </summary>
  public partial class CslaDataSourceConfiguration : Form
  {
    private DataSourceControl _control;

    /// <summary>
    /// Create instance of object.
    /// </summary>
    public CslaDataSourceConfiguration()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Create instance of object.
    /// </summary>
    /// <param name="control">Reference to the data source control.</param>
    /// <param name="oldTypeName">Existing type name.</param>
    public CslaDataSourceConfiguration(DataSourceControl control, string oldTypeName)
      : this()
    {
      _control = control;
      DiscoverTypes();
      this.TypeComboBox.Text = oldTypeName;
    }

    /// <summary>
    /// Gets the type name entered by the user.
    /// </summary>
    public string TypeName
    {
      get { return this.TypeComboBox.Text; }
    }

    private void DiscoverTypes()
    {
      // try to get a reference to the type discovery service
      ITypeDiscoveryService discovery = null;
      if (_control.Site != null)
        discovery = (ITypeDiscoveryService)_control.Site.GetService(typeof(ITypeDiscoveryService));

      if (discovery != null)
      {
        // saves the cursor and sets the wait cursor
        Cursor previousCursor = Cursor.Current;
        Cursor.Current = Cursors.WaitCursor;
        try
        {
          // gets all types using the type discovery service
          ICollection types = discovery.GetTypes(typeof(object), true);
          TypeComboBox.BeginUpdate();
          TypeComboBox.Items.Clear();
          // adds the types to the list
          foreach (Type type in types)
          {
            if (type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(",")) != "Csla" &&
              typeof(Csla.Core.IBusinessObject).IsAssignableFrom(type))
            {
              TypeComboBox.Items.Add(type.FullName);
            }
          }
        }
        finally
        {
          Cursor.Current = previousCursor;
          TypeComboBox.EndUpdate();
        }
      }
    }

  }
}