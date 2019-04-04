//-----------------------------------------------------------------------
// <copyright file="ContextParams.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implementation of a lock that waits while</summary>
//----------------------------------------------------------------------
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using Csla.Core;
#if NETFX_CORE && !NETCORE
using System.Collections.Generic;
#endif


namespace Csla.Threading
{
  /// <summary>
  /// Holds the CSLA Context values
  /// </summary>
  internal class ContextParams
  {
    public IPrincipal User { get; private set; }
    public Csla.Core.ContextDictionary ClientContext { get; private set; }
    public Csla.Core.ContextDictionary GlobalContext { get; private set; }
    public CultureInfo UICulture { get; private set; }
    public CultureInfo Culture { get; private set; }

    public ContextParams()
    {
      this.User = Csla.ApplicationContext.User;
      this.ClientContext = Csla.ApplicationContext.ClientContext;
      this.GlobalContext = Csla.ApplicationContext.GlobalContext;
      this.Culture = System.Globalization.CultureInfo.CurrentCulture;
      this.UICulture = System.Globalization.CultureInfo.CurrentUICulture;
    }

    internal void SetThreadContext()
    {
      Csla.ApplicationContext.User = User;
      Csla.ApplicationContext.SetContext(ClientContext, GlobalContext);
      Thread.CurrentThread.CurrentUICulture = UICulture;
      Thread.CurrentThread.CurrentCulture = Culture;
    }
  }
}