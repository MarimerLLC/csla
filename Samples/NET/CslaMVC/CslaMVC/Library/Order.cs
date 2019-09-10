using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using Csla.Data;
using Csla.Rules;
using CslaMVC.Repository;
using CslaMVC.Rules;

namespace CslaMVC.Library
{
    [Serializable]
    public class Order : BusinessBase<Order>
    {
        #region Business Methods

        public static readonly PropertyInfo<int> CustomerNoProperty = RegisterProperty<int>(o => o.CustomerNo, "Customer No");
        public int CustomerNo
        {
            get { return GetProperty(CustomerNoProperty); }
            set { SetProperty(CustomerNoProperty, value); }
        }

        public static readonly PropertyInfo<Guid> OrderIdProperty = RegisterProperty<Guid>(o => o.OrderId, "Order Id");
        public Guid OrderId
        {
            get { return GetProperty(OrderIdProperty); }
            set { SetProperty(OrderIdProperty, value); }
        }

        public static readonly PropertyInfo<DateTime> OrderDateProperty = RegisterProperty<DateTime>(o => o.OrderDate, "Order Date");
        public DateTime OrderDate
        {
            get { return GetProperty(OrderDateProperty); }
            set { SetProperty(OrderDateProperty, value); }
        }

        public static readonly PropertyInfo<int> StatusProperty = RegisterProperty<int>(o => o.Status, "Status");
        public int Status
        {
            get { return GetProperty(StatusProperty); }
            set { SetProperty(StatusProperty, value); }
        }

        public static readonly PropertyInfo<DateTime?> ShippedDateProperty = RegisterProperty<DateTime?>(o => o.ShippedDate, "Shipped Date");
        public DateTime? ShippedDate
        {
            get { return GetProperty(ShippedDateProperty); }
            set { SetProperty(ShippedDateProperty, value); }
        }

        public static readonly PropertyInfo<DateTime?> ReceivedDateProperty = RegisterProperty<DateTime?>(o => o.ReceivedDate, "Received Date");
        public DateTime? ReceivedDate
        {
            get { return GetProperty(ReceivedDateProperty); }
            set { SetProperty(ReceivedDateProperty, value); }
        }

        public static readonly PropertyInfo<OrderDetailsLists> OrderDetailsProperty = RegisterProperty<OrderDetailsLists>(o => o.OrderDetails, "Order Details");
        public OrderDetailsLists OrderDetails
        {
            get { return GetProperty(OrderDetailsProperty); }
            set { SetProperty(OrderDetailsProperty, value); }
        }

        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules(); //include and data annotation attribute rules

            //BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(StatusProperty, 1));
            //BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(c => TestRuleAction(c)));
        }

        private void TestRuleAction(IRuleContext context)
        {
            context.AddErrorResult("test rule broken");
        }

        #endregion

        #region Factory Methods

        public static Order NewOrder()
        {
            return DataPortal.Create<Order>();
        }

        public static Order GetOrder(Guid OrderId)
        {
            return DataPortal.Fetch<Order>(
              new SingleCriteria<Order, Guid>(OrderId));
        }

        internal static Order GetOrder(object data)
        {
            var order = new Order();
            DataMapper.Map(data, order);
            order.LoadProperty(OrderDetailsProperty, OrderDetailsLists.GetOrderDetailsLists(order.OrderId));
            order.MarkAsChild();
            return order;
        }

      public Order()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        [RunLocal]
        protected override void DataPortal_Create()
        {
            var now = DateTime.Now;
            this.OrderId = Guid.NewGuid();
            this.OrderDate = now;
            base.DataPortal_Create();
        }

        private void DataPortal_Fetch(SingleCriteria<Order, Guid> criteria)
        {
            var data = Data.Connect();
            var order = data.Orders.Where(o => o.OrderId == criteria.Value).SingleOrDefault();
            if (order == null) return;
            BusinessRules.SuppressRuleChecking = true;
            DataMapper.Map(order, this);
            BusinessRules.SuppressRuleChecking = false;
        }

        protected override void DataPortal_Insert()
        {
            var data = Data.Connect();
            var order = new CslaMVC.Repository.Order();
            DataMapper.Map(this, order, "OrderDetails");
            data.Orders.Add(order);
        }

        protected override void DataPortal_Update()
        {
            var data = Data.Connect();
            var order = data.Orders.Where(c => c.OrderId == this.OrderId).SingleOrDefault();
            var index = data.Orders.IndexOf(order);
            if (order == null) return;
            DataMapper.Map(this, order);
            data.Orders[index] = order;
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new SingleCriteria<Order, Guid>(GetProperty<Guid>(OrderIdProperty)));
        }

        private void DataPortal_Delete(SingleCriteria<Order, Guid> criteria)
        {
            var data = Data.Connect();
            var order = data.Orders.Where(c => c.OrderId == this.OrderId).SingleOrDefault();
            if (order == null) return;
            data.Orders.Remove(order);
        }

        #endregion

        #region Child Data Access

        protected override void Child_Create()
        {
            // TODO: load default values
            // omit this override if you have no defaults to set
            base.Child_Create();
        }

        private void Child_Insert(object parent)
        {
            // TODO: insert values
        }

        private void Child_Update(object parent)
        {
            // TODO: update values
        }

        private void Child_DeleteSelf(object parent)
        {
            // TODO: delete values
        }

        #endregion
    }
}
