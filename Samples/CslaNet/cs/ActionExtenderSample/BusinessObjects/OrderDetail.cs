using System;
using System.ComponentModel;
using Csla;
using Csla.Data;
using Csla.Security;
using CslaStore.Data;

using System.ComponentModel.DataAnnotations;
using Csla.Reflection;

namespace CslaStore.Business
{
    [Serializable()]
    public class OrderDetail : BusinessBase<OrderDetail>
    {

        #region Constructors

        private OrderDetail() 
        {
        }

        #endregion

        #region Property registrations

        protected static PropertyInfo<Guid> orderDetailIDProperty =
            RegisterProperty<Guid>(typeof(OrderDetail), 
            new PropertyInfo<Guid>("OrderDetailID", "Order Detail ID", Guid.Empty));

        protected static PropertyInfo<Guid> orderIDProperty =
            RegisterProperty<Guid>(typeof(OrderDetail), 
            new PropertyInfo<Guid>("OrderID", "Order ID", Guid.Empty));

        protected static PropertyInfo<Guid> productIDProperty =
            RegisterProperty<Guid>(typeof(OrderDetail), 
            new PropertyInfo<Guid>("ProductID", "Product ID", Guid.Empty));

        protected static PropertyInfo<decimal> purchaseUnitPriceProperty =
            RegisterProperty<decimal>(typeof(OrderDetail), 
            new PropertyInfo<decimal>("PurchaseUnitPrice", "Purchase Unit Price", 0M));

        protected static PropertyInfo<int> quantityProperty =
            RegisterProperty<int>(typeof(OrderDetail), 
            new PropertyInfo<int>("Quantity", "Quantity", 0));

        #endregion

        #region Member variables

        protected Guid _OrderDetailID = orderDetailIDProperty.DefaultValue;
        protected Guid _OrderID = orderIDProperty.DefaultValue;
        protected Guid _ProductID = productIDProperty.DefaultValue;
        protected decimal _PurchaseUnitPrice = purchaseUnitPriceProperty.DefaultValue;
        protected int _Quantity = quantityProperty.DefaultValue;

        #endregion

        #region Public properties

        [DataObjectField(true, false)]
        public virtual Guid OrderDetailID
        {
            get
            {
                return GetProperty<Guid>(orderDetailIDProperty, _OrderDetailID);
            }
        }

        public virtual Guid OrderID
        {
            get
            {
                return GetProperty<Guid>(orderIDProperty, _OrderID);
            }
            set
            {
                SetProperty<Guid>(orderIDProperty, ref _OrderID, value); 
            }
        }
        
        public virtual Guid ProductID
        {
            get
            {
                return GetProperty<Guid>(productIDProperty, _ProductID);
            }
            set
            {
                SetProperty<Guid>(productIDProperty, ref _ProductID, value);
            }
        }

        public virtual decimal PurchaseUnitPrice
        {
            get
            {
                return GetProperty<decimal>(purchaseUnitPriceProperty, _PurchaseUnitPrice);
            }
            set
            {
                SetProperty<decimal>(purchaseUnitPriceProperty, ref _PurchaseUnitPrice, value);
            }
        }

        public virtual int Quantity
        {
            get
            {
                return GetProperty<int>(quantityProperty, _Quantity);
            }
            set
            {
                SetProperty<int>(quantityProperty, ref _Quantity, value);
            }
        }

        #endregion

        #region Read/Load overloads -  Required for Rules to work properly with private backing fields
        
        /// <summary>
        /// Gets a property's value.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
        /// <returns></returns>
        protected override object ReadProperty(Csla.Core.IPropertyInfo propertyInfo)
        {
            using (BypassPropertyChecks)
            {
                return MethodCaller.CallPropertyGetter(this, propertyInfo.Name);
            }
        }

        /// <summary>
        /// Loads a property's managed field with the
        /// supplied value.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <remarks>
        /// No authorization checks occur when this method is called,
        /// and no PropertyChanging or PropertyChanged events are raised.
        /// Loading values does not cause validation rules to be
        /// invoked.
        /// </remarks>
        protected override void LoadProperty(Csla.Core.IPropertyInfo propertyInfo, object newValue)
        {
            using (BypassPropertyChecks)
            {
                MethodCaller.CallPropertySetter(this, propertyInfo.Name, newValue);
            }
        }

        #endregion

        #region Authorization rules

        protected static void AddObjectAuthorizationRules()
        {
        }

        #endregion

        #region Business Rules
        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();
            BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(quantityProperty, 1));
        }
        #endregion

        #region Factory methods

        internal static OrderDetail NewOrderDetail()
        {
            return DataPortal.CreateChild<OrderDetail>();
        }

        internal static OrderDetail GetOrderDetail(SafeDataReader reader)
        {
            return DataPortal.FetchChild<OrderDetail>(reader);
        }

        #endregion

        #region Data access

        protected override void Child_Create()
        {
            _OrderDetailID = Guid.NewGuid();
            BusinessRules.CheckRules();
        }

        protected void Child_Fetch(SafeDataReader reader)
        {
            _OrderDetailID = reader.GetGuid("OrderDetailID");
            _OrderID = reader.GetGuid("OrderID");
            _ProductID = reader.GetGuid("ProductID");
            _PurchaseUnitPrice = reader.GetDecimal("PurchaseUnitPrice");
            _Quantity = reader.GetInt32("Quantity");
        }

        protected void Child_Insert()
        {
            using (SafeDataReader reader = new DataAccess().OrderDetailInsert(
                _OrderDetailID, _OrderID, _ProductID, _PurchaseUnitPrice, _Quantity)) { }
        }

        protected void Child_Update()
        {
            using (SafeDataReader reader = new DataAccess().OrderDetailUpdate(
                _OrderDetailID, _OrderID, _ProductID, _PurchaseUnitPrice, _Quantity)) { }
        }

        protected void Child_DeleteSelf()
        {
            using (SafeDataReader reader = new DataAccess().OrderDetailDelete(
                _OrderDetailID)) { }
        }

        #endregion

    }
}
