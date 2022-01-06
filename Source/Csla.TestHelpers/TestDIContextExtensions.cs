using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.TestHelpers
{

  /// <summary>
  /// Extension methods for the TestDIContext class
  /// </summary>
  public static class TestDIContextExtensions
  {

    /// <summary>
    /// Create an application context for use in unit testing
    /// </summary>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An ApplicationContext constructed for use in testing</returns>
    public static ApplicationContext CreateTestApplicationContext(this TestDIContext context)
    {
      return ApplicationContextFactory.CreateTestApplicationContext(context);
    }

    /// <summary>
    /// Create a DataPortal instance for use in unit testing
    /// </summary>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An instance of IDataPortal<typeparamref name="T"/> for use in testing</returns>
    public static IDataPortal<T> CreateDataPortal<T>(this TestDIContext context)
    {
      return DataPortalFactory.CreateDataPortal<T>(context);
    }

    /// <summary>
    /// Create a Child DataPortal instance for use in unit testing
    /// </summary>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An instance of IChildDataPortal<typeparamref name="T"/> for use in testing</returns>
    public static IChildDataPortal<T> CreateChildDataPortal<T>(this TestDIContext context)
    {
      return DataPortalFactory.CreateChildDataPortal<T>(context);
    }
  }

}
