//-----------------------------------------------------------------------
// <copyright file="CslaDataSourceDesigner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements designer support for CslaDataSource.</summary>
//-----------------------------------------------------------------------
using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Csla.Web;

namespace Csla.Web.Design
{
  /// <summary>
  /// Implements designer support for CslaDataSource.
  /// </summary>
  public class CslaDataSourceDesigner : DataSourceDesigner
  {

    private DataSourceControl _control = null;
    private CslaDesignerDataSourceView _view = null;

    /// <summary>
    /// Initialize the designer component.
    /// </summary>
    /// <param name="component">The CslaDataSource control to 
    /// be designed.</param>
    public override void Initialize(IComponent component)
    {
      base.Initialize(component);
      _control = (DataSourceControl)component;
    }

    internal System.ComponentModel.ISite Site
    {
      get
      {
        return _control.Site;
      }
    }
    /// <summary>
    /// Returns the default view for this designer.
    /// </summary>
    /// <param name="viewName">Ignored</param>
    /// <returns></returns>
    /// <remarks>
    /// This designer supports only a "Default" view.
    /// </remarks>
    public override DesignerDataSourceView GetView(string viewName)
    {
      if (_view == null)
      {
        _view = new CslaDesignerDataSourceView(this, "Default");
      }
      return _view;
    }

    /// <summary>
    /// Return a list of available views.
    /// </summary>
    /// <remarks>
    /// This designer supports only a "Default" view.
    /// </remarks>
    public override string[] GetViewNames()
    {
      return new string[] { "Default" };
    }

    /// <summary>
    /// Refreshes the schema for the data.
    /// </summary>
    /// <param name="preferSilent"></param>
    /// <remarks></remarks>
    public override void RefreshSchema(bool preferSilent)
    {
      this.OnSchemaRefreshed(EventArgs.Empty);
    }

    /// <summary>
    /// Get a value indicating whether the control can
    /// refresh its schema.
    /// </summary>
    public override bool CanRefreshSchema
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Invoke the design time configuration
    /// support provided by the control.
    /// </summary>
    public override void Configure()
    {
      InvokeTransactedChange(_control, ConfigureCallback, null, "ConfigureDataSource");
    }

    private bool ConfigureCallback(object context)
    {
      bool result = false;

      string oldTypeName;
      if (string.IsNullOrEmpty(((CslaDataSource)DataSourceControl).TypeAssemblyName))
        oldTypeName = ((CslaDataSource)DataSourceControl).TypeName;
      else
        oldTypeName = string.Format("{0}, {1}", 
          ((CslaDataSource)DataSourceControl).TypeName, ((CslaDataSource)DataSourceControl).TypeAssemblyName);

      IUIService uiService = (IUIService)_control.Site.GetService(typeof(IUIService));
      CslaDataSourceConfiguration cfg = new CslaDataSourceConfiguration(_control, oldTypeName);
      if (uiService.ShowDialog(cfg) == System.Windows.Forms.DialogResult.OK)
      {
        SuppressDataSourceEvents();
        try
        {
          ((CslaDataSource)DataSourceControl).TypeAssemblyName = string.Empty;
          ((CslaDataSource)DataSourceControl).TypeName = cfg.TypeName;
          OnDataSourceChanged(EventArgs.Empty);
          result = true;
        }
        finally
        {
          ResumeDataSourceEvents();
        }
      }
      cfg.Dispose();
      return result;
    }

    /// <summary>
    /// Get a value indicating whether this control
    /// supports design time configuration.
    /// </summary>
    public override bool CanConfigure
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Get a value indicating whether the control can
    /// be resized.
    /// </summary>
    public override bool AllowResize
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Get a reference to the CslaDataSource control being
    /// designed.
    /// </summary>
    internal CslaDataSource DataSourceControl
    {
      get
      {
        return (CslaDataSource)_control;
      }
    }
  }
}
