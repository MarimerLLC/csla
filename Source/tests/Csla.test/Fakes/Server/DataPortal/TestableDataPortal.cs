//-----------------------------------------------------------------------
// <copyright file="TestableDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Basically this test class exposes protected DataPortal constructor overloads to the </summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Server;
using Csla.Server.Dashboard;

namespace Csla.Testing.Business.DataPortal
{
  /// <summary>
  /// Basically this test class exposes protected DataPortal constructor overloads to the 
  /// Unit Testing System, allowing for a more fine-grained Unit Tests
  /// </summary>
  public class TestableDataPortal : Csla.Server.DataPortal
  {
    public TestableDataPortal(
      ApplicationContext applicationContext,
      IDashboard dashboard,
      CslaOptions options,
      IAuthorizeDataPortal authorizer,
      InterceptorManager interceptors,
      DataPortalExceptionHandler exceptionHandler,
      SecurityOptions securityOptions
    ) : base(
      applicationContext,
      dashboard,
      options,
      authorizer,
      interceptors,
      exceptionHandler,
      securityOptions
    )
    {
      AuthProvider = authorizer;
    }

    public static void Setup()
    {
    }

    public Type AuthProviderType
    {
      get
      {
        return AuthProvider.GetType();
      }
    }

    public IAuthorizeDataPortal AuthProvider { get; }

    public bool NullAuthorizerUsed
    {
      get
      {
        return AuthProviderType == typeof(NullAuthorizer);
      }
    }

  }
}