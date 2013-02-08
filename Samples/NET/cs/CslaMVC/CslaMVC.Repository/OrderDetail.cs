using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CslaMVC.Repository
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; }
        public Guid OrderDetailId { get; set; }
        public int LineNo { get; set; }
        public string Item { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
    }
}
