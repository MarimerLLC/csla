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
    public Core.IClientContext ClientContext { get; }
    public CultureInfo UICulture { get; }
    public CultureInfo Culture { get; }

    public ContextParams()
    {
      User = _applicationContext.User;
      ClientContext = _applicationContext.ClientContext;
      Culture = CultureInfo.CurrentCulture;
      UICulture = CultureInfo.CurrentUICulture;
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