//-----------------------------------------------------------------------
// <copyright file="AddressList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace DataBinding.Business
{
  [Serializable]
  public partial class AddressList : BusinessListBase<AddressList, Address>
  {
    public static void FetchByCustomer(EventHandler<DataPortalResult<AddressList>> completed, int customerId)
    {
      DataPortal<AddressList> dp = new DataPortal<AddressList>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new SingleCriteria<AddressList, int>(customerId));
    }
  }
}