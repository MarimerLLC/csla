using System;
using System.Collections.Generic;
using System.Linq;
using Csla;
using CslaMVC.Repository;

namespace CslaMVC.Library
{
    [Serializable]
    public class OrdersList :
      BusinessListBase<OrdersList, Order>
    {
        #region Factory Methods

        public static OrdersList NewOrdersList()
        {
            return DataPortal.Create<OrdersList>();
        }

        public static OrdersList GetOrdersList(int CustomerNo)
        {
            return DataPortal.Fetch<OrdersList>(
              new SingleCriteria<OrdersList, int>(CustomerNo));
        }

      public OrdersList()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        private void DataPortal_Fetch(
          SingleCriteria<OrdersList, int> criteria)
        {
            RaiseListChangedEvents = false;

            this.AddRange(Data.Connect().Orders
                    .Where(o => o.CustomerNo == criteria.Value)
                    .Select(o => Order.GetOrder(o)));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
