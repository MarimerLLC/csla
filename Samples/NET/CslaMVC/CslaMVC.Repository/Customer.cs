using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CslaMVC.Repository
{
    public class Customer
    {
        public int CustomerNo { get; set; }
        public string Name { get; set; }
        public int GroupNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }
}
