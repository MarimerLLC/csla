// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LookupCustomerCommand.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The lookup customer command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using Csla;

namespace LookupRule.Commands
{
  /// <summary>
  /// The lookup customer command.
  /// </summary>
  [Serializable]
  public class LookupCustomerCommand : CommandBase<LookupCustomerCommand>
  {
    /// <summary>
    /// The customer id property.
    /// </summary>
    public static readonly PropertyInfo<int> CustomerIdProperty = RegisterProperty<int>(c => c.CustomerId);

    /// <summary>
    /// Gets or sets CustomerId.
    /// </summary>
    public int CustomerId
    {
      get { return ReadProperty(CustomerIdProperty); }
      set { LoadProperty(CustomerIdProperty, value); }
    }

    /// <summary>
    /// The name property.
    /// </summary>
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name
    {
      get { return ReadProperty(NameProperty); }
      set { LoadProperty(NameProperty, value); }
    }

    [Create]
    private void Create()
    { }

    /// <summary>
    /// Sync lookup of customer name
    /// </summary>
    /// <param name="customerId">
    /// The customer id.
    /// </param>
    /// <returns>
    /// </returns>
    public static LookupCustomerCommand Execute(int customerId)
    {
      var cmd = DataPortal.Create<LookupCustomerCommand>();
      cmd.CustomerId = customerId;
      cmd = DataPortal.Execute<LookupCustomerCommand>(cmd);
      return cmd;
    }

    /// <summary>
    /// The data portal_ execute.
    /// </summary>
    protected override void DataPortal_Execute()
    {
      // wait for 500 ms
      System.Threading.Thread.Sleep(50);

      // simulate llokup and set customer name  
      this.Name = string.Format("Name ({0})", this.CustomerId);
    }
  }
}
