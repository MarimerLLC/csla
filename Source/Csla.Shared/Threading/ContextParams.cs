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
#if NETFX_CORE
using Windows.ApplicationModel.Resources.Core;
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
#if NETFX_CORE
      public string UICulture { get; private set; }
      public string Culture { get; private set; }
#else
    public CultureInfo UICulture { get; private set; }
    public CultureInfo Culture { get; private set; }
#endif

    public ContextParams()
    {
      this.User = Csla.ApplicationContext.User;
      this.ClientContext = Csla.ApplicationContext.ClientContext;
      this.GlobalContext = Csla.ApplicationContext.GlobalContext;
#if NETFX_CORE
      var language = ResourceContext.GetForCurrentView().Languages[0];
      this.UICulture = language;
      this.Culture = language;
#else
      this.UICulture = Thread.CurrentThread.CurrentUICulture;
      this.Culture = Thread.CurrentThread.CurrentCulture;
#endif
    }

    internal void SetThreadContext()
    {
      Csla.ApplicationContext.User = User;
      Csla.ApplicationContext.SetContext(ClientContext, GlobalContext);
#if NETFX_CORE
      var resourceContext = ResourceContext.GetForCurrentView();
      var list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { UICulture });
      resourceContext.Languages = list;
      list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { Culture });
      resourceContext.Languages = list;
#else
      Thread.CurrentThread.CurrentUICulture = UICulture;
      Thread.CurrentThread.CurrentCulture = Culture;
#endif
    }
  }
}