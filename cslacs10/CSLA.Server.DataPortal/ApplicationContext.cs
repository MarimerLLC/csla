using System;
using System.Threading;
using System.Collections.Specialized;

namespace CSLA
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public class ApplicationContext
  {
    /// <summary>
    /// Returns the application-specific context data provided
    /// by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred from
    /// the client to the server. No data is transferred from
    /// the server to the client.
    /// <para>
    /// </remarks>
    public static HybridDictionary ClientContext 
    {
      get
      {
        System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.ClientContext");
        HybridDictionary ctx = (HybridDictionary)Thread.GetData(slot);
        if(ctx == null)
        {
          ctx = new HybridDictionary();
          Thread.SetData(slot, ctx);
        }
        return ctx;
      }
    }

    /// <summary>
    /// Returns the application-specific context data shared
    /// on both client and server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred to and from
    /// the client and server. Any objects or data in this context
    /// will be transferred bi-directionally across the network.
    /// <para>
    /// </remarks>
    public static HybridDictionary GlobalContext
    {
      get
      {
        System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.GlobalContext");
        HybridDictionary ctx = (HybridDictionary)Thread.GetData(slot);
        if(ctx == null)
        {
          ctx = new HybridDictionary();
          Thread.SetData(slot, ctx);
        }
        return ctx;
      }
    }

    internal static HybridDictionary GetClientContext()
    {
      System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.ClientContext");
      return (HybridDictionary)Thread.GetData(slot);
    }

    internal static HybridDictionary GetGlobalContext() 
    {
      System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.GlobalContext");
      return (HybridDictionary)Thread.GetData(slot);
    }

    internal static void SetContext(object clientContext, object globalContext)
    {
      System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.ClientContext");
      Thread.SetData(slot, clientContext);

      slot = Thread.GetNamedDataSlot("CSLA.GlobalContext");
      Thread.SetData(slot, globalContext);
    }

    public static void Clear()
    {
      SetContext(null, null);
    }

    private ApplicationContext()
    {
      // prevent instantiation
    }

  }
}
