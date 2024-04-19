//-----------------------------------------------------------------------
// <copyright file="ContextParams.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a lock that waits while</summary>
//----------------------------------------------------------------------
using System.Globalization;
using System.Security.Principal;

namespace Csla.Threading
{
  /// <summary>
  /// Holds the CSLA Context values
  /// </summary>
  internal class ContextParams
  {
    public ContextParams(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    private ApplicationContext _applicationContext;

    public IPrincipal User { get; }
    public Csla.Core.ContextDictionary ClientContext { get; }
    public CultureInfo UICulture { get; }
    public CultureInfo Culture { get; }

    public ContextParams()
    {
      this.User = _applicationContext.User;
      this.ClientContext = _applicationContext.ClientContext;
      this.Culture = System.Globalization.CultureInfo.CurrentCulture;
      this.UICulture = System.Globalization.CultureInfo.CurrentUICulture;
    }

    internal void SetThreadContext()
    {
      _applicationContext.User = User;
      _applicationContext.SetContext(ClientContext);
      Thread.CurrentThread.CurrentUICulture = UICulture;
      Thread.CurrentThread.CurrentCulture = Culture;
    }
  }
}