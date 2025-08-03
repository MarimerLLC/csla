//-----------------------------------------------------------------------
// <copyright file="TestableDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Basically this test class exposes protected DataPortal constructor overloads to the </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Server;


namespace  Csla.Testing.Business.DataPortal
{
  /// <summary>
  /// Basically this test class exposes protected DataPortal constructor overloads to the 
  /// Unit Testing System, allowing for a more fine-grained Unit Tests
  /// </summary>
  public class TestableDataPortal : Csla.Server.DataPortal
  {
    public TestableDataPortal() : base(){}

    public TestableDataPortal(string cslaAuthorizationProviderAppSettingName):
      base(cslaAuthorizationProviderAppSettingName)
    {
    }

    public TestableDataPortal(Type authProviderType):base(authProviderType) { }

    public static void Setup()
    {
      Authorizer = null;
    }

    public Type AuthProviderType
    {
      get { return Authorizer.GetType(); }
    }

    public IAuthorizeDataPortal AuthProvider
    {
      get { return Authorizer; }
    }

    public bool NullAuthorizerUsed
    {
      get
      {
        return AuthProviderType == typeof (NullAuthorizer);
      }
    }

  }
}