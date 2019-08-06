#if !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="WcfErrorInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Message containing details about any</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Message containing details about any
  /// server-side exception.
  /// </summary>
  [DataContract]
  public class WcfErrorInfo
  {
    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    [DataMember]
    public string ExceptionTypeName { get; set; }
    /// <summary>
    /// Message from the exception object.
    /// </summary>
    [DataMember]
    public string Message { get; set; }
    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    [DataMember]
    public string StackTrace { get; set; }
    /// <summary>
    /// Source of the exception object.
    /// </summary>
    [DataMember]
    public string Source { get; set; }
    /// <summary>
    /// Target site name from the exception object.
    /// </summary>
    [DataMember]
    public string TargetSiteName { get; set; }
    /// <summary>
    /// WcfErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    [DataMember]
    public WcfErrorInfo InnerError { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="ex">
    /// The Exception to encapusulate.
    /// </param>
    public WcfErrorInfo(Exception ex)
    {
      this.ExceptionTypeName = ex.GetType().FullName;
      this.Message = ex.Message;
      this.StackTrace = ex.StackTrace;
      this.Source = ex.Source;
      if (ex.TargetSite != null)
        this.TargetSiteName = ex.TargetSite.Name;
      if (ex.InnerException != null)
        this.InnerError = new WcfErrorInfo(ex.InnerException);
    }
  }
}
#endif