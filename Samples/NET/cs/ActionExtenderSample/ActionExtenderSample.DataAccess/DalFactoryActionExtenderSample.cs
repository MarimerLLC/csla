//-----------------------------------------------------------------------
// <copyright file="DalFactoryActionExtenderSample.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------
using System;
using System.Configuration;

namespace ActionExtenderSample.DataAccess
{
  /// <summary>
  /// Creates a ActionExtenderSample DAL manager provider.
  /// </summary>
  /// <remarks>
  /// To use the generated DAL:<br/>
  /// 1) name this assembly ActionExtenderSample.DataAccess<br/>
  /// 2) add the following line to the <strong>appSettings</strong>
  /// section of the application .config file: <br/>
  /// &lt;add key="ActionExtenderSample.DalManagerType" value="ActionExtenderSample.DataAccess.Sql.DalManagerActionExtenderSample, ActionExtenderSample.DataAccess.Sql" /&gt;
  /// </remarks>
  public static class DalFactoryActionExtenderSample
  {
    private static Type _dalType;

    /// <summary>Gets the ActionExtenderSample DAL manager type that must be set
    /// in the <strong>appSettings</strong> section of the application .config file.</summary>
    /// <returns>A new <see cref="IDalManagerActionExtenderSample"/> instance</returns>
    public static IDalManagerActionExtenderSample GetManager()
    {
      if (_dalType == null)
      {
        var dalTypeName = ConfigurationManager.AppSettings["ActionExtenderSample.DalManagerType"];
        if (!string.IsNullOrEmpty(dalTypeName))
          _dalType = Type.GetType(dalTypeName);
        else
          throw new NullReferenceException("ActionExtenderSample.DalManagerType");
        if (_dalType == null)
          throw new ArgumentException(string.Format("Type {0} could not be found", dalTypeName));
      }
      return (IDalManagerActionExtenderSample) Activator.CreateInstance(_dalType);
    }
  }
}
