using System;
using System.Security.Principal;
using CSLA;
using CSLA.Security;
using CSLA.Resources;

namespace CSLA.BatchQueue.Server
{
  /// <summary>
  /// This is the entry point for all queue requests from
  /// the client via Remoting.
  /// </summary>
  public class BatchQueue : MarshalByRefObject
  {
    #region Public methods

    /// <summary>
    /// Submits a batch entry to the queue.
    /// </summary>
    /// <param name="Entry">A reference to the batch entry.</param>
    public void Submit(BatchEntry entry)
    {
      BatchQueueService.Enqueue(entry);
    }

    /// <summary>
    /// Removes a holding or pending entry from the queue.
    /// </summary>
    /// <param name="Entry">A reference to the info object for the batch entry.</param>
    public void Remove(BatchEntryInfo entry)
    {
      BatchQueueService.Dequeue(entry);
    }

    /// <summary>
    /// Gets a list of the entries currently in the
    /// batch queue.
    /// </summary>
    /// <param name="Principal">The requesting user's security credentials.</param>
    public BatchEntries GetEntries(IPrincipal principal) 
    {
      SetPrincipal(principal);

      BatchEntries entries = new BatchEntries();
      BatchQueueService.LoadEntries(entries);

      return entries;
    }

    #endregion

    #region Security

    string AUTHENTICATION
    {
      get
      {
        return ConfigurationSettings.AppSettings["Authentication"];
      }
    }

    void SetPrincipal(object principal)
    {
      if(AUTHENTICATION == "Windows")
      {
        // when using integrated security, Principal must be Nothing
        // and we need to set our policy to use the Windows principal
        if(principal == null)
        {
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
          return;
        }
        else
          throw new System.Security.SecurityException(Strings.GetResourceString("NoPrincipalAllowedException"));
      }

      // we expect Principal to be of type BusinessPrincipal, but
      // we can't enforce that since it causes a circular reference
      // with the business library so instead we must use type Object
      // for the parameter, so here we do a check on the type of the
      // parameter
      if(((IPrincipal)principal).Identity.AuthenticationType == "CSLA")
      {
        // see if our current principal is
        // different from the caller's principal
        if(!ReferenceEquals(principal, System.Threading.Thread.CurrentPrincipal))
        {
          // the caller had a different principal, so change ours to
          // match the caller's so all our objects use the caller's
          // security
          System.Threading.Thread.CurrentPrincipal = (IPrincipal)principal;
        }
      }
      else
        throw new System.Security.SecurityException(Strings.GetResourceString("BusinessPrincipalException") + principal.ToString());
    }


    #endregion
  }
}
