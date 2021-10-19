//-----------------------------------------------------------------------
// <copyright file="CslaDataSourceConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CslaDataSource configuration form.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Web.UI;
using System.ComponentModel.Design;
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
              string name = type.AssemblyQualifiedName;
              if (name.Substring(name.Length - 19, 19) == "PublicKeyToken=null")
                name = name.Substring(0, name.IndexOf(",", name.IndexOf(",") + 1));
              TypeComboBox.Items.Add(name);
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