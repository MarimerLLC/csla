using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CslaMVC.Repository
{
    public class Order
    {
        public int CustomerNo { get; set; }
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}
