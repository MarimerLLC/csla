using System;
using System.Collections.Generic;
using System.Linq;
using Csla;
using CslaMVC.Repository;

namespace CslaMVC.Library
{
    [Serializable]
    public class CustomerList :
      BusinessListBase<CustomerList, Customer>
    {
        #region Factory Methods

        public static CustomerList NewCustomerList()
        {
            return DataPortal.Create<CustomerList>();
        }

        public static CustomerList GetCustomerList()
        {
            return DataPortal.Fetch<CustomerList>();
        }

        public static CustomerList GetCustomerListByGroup(int GroupNo)
        {
            return DataPortal.Fetch<CustomerList>(
              new SingleCriteria<CustomerList, int>(GroupNo));
        }

      public CustomerList()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        private void DataPortal_Fetch()
        {
            RaiseListChangedEvents = false;

            this.AddRange(Data.Connect().Customers
                .Select(c => Customer.GetCustomer(c)));

            RaiseListChangedEvents = true;
        }

        private void DataPortal_Fetch(
          SingleCriteria<CustomerList, int> criteria)
        {
            RaiseListChangedEvents = false;

            this.AddRange(Data.Connect().Customers
                .Where(c => c.GroupNo == criteria.Value)
                .Select(c => Customer.GetCustomer(c)));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
