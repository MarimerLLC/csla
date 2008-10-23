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
      _authorizer = null;
    }

    public Type AuthProviderType
    {
      get { return _authorizer.GetType(); }
    }

    public IAuthorizeDataPortal AuthProvider
    {
      get { return _authorizer; }
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