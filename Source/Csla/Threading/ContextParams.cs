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
    private readonly ApplicationContext _applicationContext;

    public ContextParams(ApplicationContext applicationContext)
    {
      _applicationContext = Guard.NotNull(applicationContext);
      User = _applicationContext.User;
      ClientContext = _applicationContext.ClientContext;
      Culture = CultureInfo.CurrentCulture;
      UICulture = CultureInfo.CurrentUICulture;
    }

    public IPrincipal User { get; }
    public Core.IContextDictionary ClientContext { get; }
    public CultureInfo UICulture { get; }
    public CultureInfo Culture { get; }

    internal void SetThreadContext()
    {
      _applicationContext.User = User;
      _applicationContext.SetContext(ClientContext);
      Thread.CurrentThread.CurrentUICulture = UICulture;
      Thread.CurrentThread.CurrentCulture = Culture;
    }
  }
}