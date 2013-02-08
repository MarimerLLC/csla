using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CslaMVC.Library;
using Csla.Data;

namespace CslaMVC.ViewModels
{
    public class CustomerListViewModel
    {
        //business object
        public CustomerList Customers;

        //default
        public CustomerListViewModel()
        {
            Customers = CustomerList.GetCustomerList();
        }
    }
}
