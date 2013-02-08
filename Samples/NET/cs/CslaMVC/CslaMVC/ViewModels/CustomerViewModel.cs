using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CslaMVC.Library;
using Csla.Data;
using Csla.Web.Mvc;

namespace CslaMVC.ViewModels
{
    public class CustomerViewModel : ViewModelBase<Customer>, IViewModel
    {
        //setter for internal id prop
        public int CustomerNo
        {
            get { return ModelObject.CustomerNo; }
            set { ModelObject.CustomerNo = value; }
        }

        //value list (use private backing field to load on-demand, not every view needs list
        private List<string> states;
        public List<string> StateList
        {
            get 
            {
                if (states == null)
                {
                    //can replace with CSLA ROL, NVL or other collection
                    states = new List<string>();
                    states.Add("IL");
                    states.Add("MN");
                    states.Add("WA");
                    states.Add("XX");
                    states.Add("YY");
                    states.Add("ZZ");
                }
                return states; 
            }
        }

        //default
        public CustomerViewModel()
        {
            ModelObject = Customer.NewCustomer();
        }

        //convenience
        public CustomerViewModel(int id)
        {
            ModelObject = Customer.GetCustomer(id);
        }
    }
}