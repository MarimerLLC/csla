using System;
using Csla;

namespace AsyncLookupRule.Commands
{
  [Serializable]
  public class LookupCustomerCommand : CommandBase<LookupCustomerCommand>
  {
    public static readonly PropertyInfo<int> CustomerIdProperty = RegisterProperty<int>(c => c.CustomerId);
    public int CustomerId
    {
      get { return ReadProperty(CustomerIdProperty); }
      set { LoadProperty(CustomerIdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return ReadProperty(NameProperty); }
      set { LoadProperty(NameProperty, value); }
    }

    #region Factory Methods

    /// <summary>
    /// Sync lookup of customer name
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <returns></returns>
    public static LookupCustomerCommand Execute(int customerId)
    {
      var cmd = new LookupCustomerCommand();
      cmd.CustomerId = customerId;
      cmd = DataPortal.Execute<LookupCustomerCommand>(cmd);
      return cmd;
    }

    /// <summary>
    /// Async lookup of customer name
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <param name="callback">The callback function to execute when async call is completed.</param>
    public static void BeginExecute(int customerId, EventHandler<DataPortalResult<LookupCustomerCommand>> callback)
    {
      var cmd = new LookupCustomerCommand();
      cmd.CustomerId = customerId;
      DataPortal.BeginExecute<LookupCustomerCommand>(cmd, callback);
    }

    #endregion

    #region Server-side Code

    protected override void DataPortal_Execute()
    {
      // wait for 500 ms
      System.Threading.Thread.Sleep(500);

      // simulate llokup and set customer name  
      this.Name = string.Format("Name ({0})", this.CustomerId);
    }

    #endregion
  }
}
