using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CslaMVC.Repository
{
    public class Data
    {
        private static Data instance;

        public List<Customer> Customers;
        public List<Order> Orders;
        public List<OrderDetail> OrderDetails;

        public static Data Connect()
        {
            if (instance == null)
            {
                instance = new Data();
                instance.Load();
            }

            return instance;
        }

        private void Load()
        {
            if (Customers == null)
            {
                XElement srcCustomer = LoadXElement("Customer");
                Customers = (from el in srcCustomer.Descendants("Customer")
                             select new Customer()
                             {
                                 CustomerNo = int.Parse(el.Attribute("CustomerNo").Value),
                                 Name = el.Attribute("Name").Value,
                                 GroupNo = int.Parse(el.Attribute("GroupNo").Value),
                                 City = el.Attribute("City").Value,
                                 State = el.Attribute("State").Value,
                                 Zipcode = el.Attribute("Zipcode").Value,
                             }).ToList<Customer>();
            }

            if (Orders == null)
            {
                XElement srcOrder = LoadXElement("Order");
                Orders = (from el in srcOrder.Descendants("Order")
                          select new Order()
                          {
                              CustomerNo = int.Parse(el.Attribute("CustomerNo").Value),
                              OrderId = new Guid(el.Attribute("OrderId").Value),
                              OrderDate = DateTime.Parse(el.Attribute("OrderDate").Value),
                              Status = int.Parse(el.Attribute("Status").Value),
                              ShippedDate = (el.Attribute("ShippedDate") != null) ? DateTime.Parse(el.Attribute("ShippedDate").Value) : (DateTime?)null,
                              ReceivedDate = (el.Attribute("ReceivedDate") != null) ? DateTime.Parse(el.Attribute("ReceivedDate").Value) : (DateTime?)null,
                          }).ToList<Order>();
            }

            if (OrderDetails == null)
            {
                XElement srcOrderDetail = LoadXElement("OrderDetail");
                OrderDetails = (from el in srcOrderDetail.Descendants("OrderDetail")
                                select new OrderDetail()
                                {
                                    OrderDetailId = new Guid(el.Attribute("OrderDetailId").Value),
                                    OrderId = new Guid(el.Attribute("OrderId").Value),
                                    Item = el.Attribute("Item").Value,
                                    LineNo = int.Parse(el.Attribute("LineNo").Value),
                                    Qty = int.Parse(el.Attribute("Qty").Value),
                                    Price = decimal.Parse(el.Attribute("Price").Value),
                                    Discount = decimal.Parse(el.Attribute("Discount").Value),
                                }).ToList<OrderDetail>();
            }
        }

        private XElement LoadXElement(string data)
        {
            var stream = GetType().Assembly.GetManifestResourceStream(GetType(), string.Format("{0}.xml", data));
            return XElement.Load(stream);
        }
    }
}