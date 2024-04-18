//-----------------------------------------------------------------------
// <copyright file="HostPropertyAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>HostPropertyAttribute is used on components to </summary>
//-----------------------------------------------------------------------

namespace Csla.Windows
{
  /// <summary>
  /// HostPropertyAttribute is used on components to 
  /// indicate the property on the component that is to be used as the 
  /// parent container control in conjunction with HostComponentDesigner.
  /// </summary>
  public class HostPropertyAttribute : Attribute
  {
    #region Properties

    /// <summary>
    /// HostPropertyName gets the host property name.
    /// </summary>
    public string HostPropertyName { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor creates a new HostPropertyAttribute object instance using the information supplied.
    /// </summary>
    /// <param name="hostPropertyName">The name of the host property.</param>
    public HostPropertyAttribute(string hostPropertyName)
    {
      HostPropertyName = hostPropertyName;
    }

    #endregion

  }
}