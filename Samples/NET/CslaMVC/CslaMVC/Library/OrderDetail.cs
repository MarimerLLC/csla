using System;
using System.Collections.Generic;
using Csla;
using Csla.Data;

namespace CslaMVC.Library
{
    [Serializable]
    public class OrderDetail : BusinessBase<OrderDetail>
    {
        #region Business Methods

        public static readonly PropertyInfo<Guid> OrderIdProperty = RegisterProperty<Guid>(o => o.OrderId, "Order Id");
        public Guid OrderId
        {
            get { return GetProperty(OrderIdProperty); }
            set { SetProperty(OrderIdProperty, value); }
        }

        public static readonly PropertyInfo<Guid> OrderDetailIdProperty = RegisterProperty<Guid>(o => o.OrderDetailId, "Order Detail Id");
        public Guid OrderDetailId
        {
            get { return GetProperty(OrderDetailIdProperty); }
            set { SetProperty(OrderDetailIdProperty, value); }
        }

        public static readonly PropertyInfo<int> LineNoProperty = RegisterProperty<int>(o => o.LineNo, "Line No");
        public int LineNo
        {
            get { return GetProperty(LineNoProperty); }
            set { SetProperty(LineNoProperty, value); }
        }

        public static readonly PropertyInfo<string> ItemProperty = RegisterProperty<string>(o => o.Item, "Item");
        public string Item
        {
            get { return GetProperty(ItemProperty); }
            set { SetProperty(ItemProperty, value); }
        }

        public static readonly PropertyInfo<decimal> PriceProperty = RegisterProperty<decimal>(o => o.Price, "Price");
        public decimal Price
        {
            get { return GetProperty(PriceProperty); }
            set { SetProperty(PriceProperty, value); }
        }

        public static readonly PropertyInfo<int> QtyProperty = RegisterProperty<int>(o => o.Qty, "Quantity");
        public int Qty
        {
            get { return GetProperty(QtyProperty); }
            set { SetProperty(QtyProperty, value); }
        }

        public static readonly PropertyInfo<decimal> DiscountProperty = RegisterProperty<decimal>(o => o.Discount, "Discount");
        public decimal Discount
        {
            get { return GetProperty(DiscountProperty); }
            set { SetProperty(DiscountProperty, value); }
        }

        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            // TODO: add validation rules
            //ValidationRules.AddRule(RuleMethod, "");
        }

        #endregion

        #region Factory Methods

        internal static OrderDetail NewOrderDetail()
        {
            return DataPortal.CreateChild<OrderDetail>();
        }

        internal static OrderDetail GetOrderDetail(object data)
        {
            var orderDetail = new OrderDetail();
            DataMapper.Map(data, orderDetail);
            orderDetail.MarkAsChild();
            return orderDetail;
        }

      public OrderDetail()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

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
