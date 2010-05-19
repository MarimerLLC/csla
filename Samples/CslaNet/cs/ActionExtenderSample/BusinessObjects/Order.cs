using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Core;
using Csla.Data;
using Csla.Security;
using Csla.Reflection;
using CslaStore.Data;

namespace CslaStore.Business
{
    [Serializable()]
    public class Order : BusinessBase<Order>
    {       
        #region Constructors

        private Order()
        {
            // force use of factory methods
        }

        #endregion

        #region Property registrations

        protected static PropertyInfo<Guid> orderIDProperty = RegisterProperty(p => p.OrderID, "Order ID", Guid.Empty, RelationshipTypes.PrivateField);

        protected static PropertyInfo<string> userNameProperty = RegisterProperty(p => p.UserName, "User Name", string.Empty, RelationshipTypes.PrivateField);

        protected static PropertyInfo<string> orderNumberProperty =
            RegisterProperty<string>(typeof(Order), new PropertyInfo<string>("OrderNumber", "Order Number", string.Empty, RelationshipTypes.PrivateField));

        protected static PropertyInfo<SmartDate> orderDateProperty =
            RegisterProperty<SmartDate>(typeof(Order), new PropertyInfo<SmartDate>("OrderDate", "Order Date", null, RelationshipTypes.PrivateField));

        protected static PropertyInfo<string> cardTypeProperty =
            RegisterProperty<string>(typeof(Order), new PropertyInfo<string>("CardType", "Card Type", string.Empty, RelationshipTypes.PrivateField));

        protected static PropertyInfo<string> cardHolderProperty =
            RegisterProperty<string>(typeof(Order), new PropertyInfo<string>("CardHolder", "Card Holder", string.Empty, RelationshipTypes.PrivateField));

        protected static PropertyInfo<string> creditCardProperty =
            RegisterProperty<string>(typeof(Order), new PropertyInfo<string>("CreditCard", "Credit Card", string.Empty, RelationshipTypes.PrivateField));

        protected static PropertyInfo<string> expDateProperty =
            RegisterProperty<string>(typeof(Order), new PropertyInfo<string>("ExpDate", "Expiration Date", string.Empty, RelationshipTypes.PrivateField));

        protected static PropertyInfo<OrderDetailCollection> orderDetailListProperty =
            RegisterProperty<OrderDetailCollection>(typeof(Order), new PropertyInfo<OrderDetailCollection>("OrderDetailList",
                "Order Details", OrderDetailCollection.NewOrderDetailCollection()));

        #endregion

        #region Member variables

        protected Guid _OrderID = orderIDProperty.DefaultValue;
        protected string _UserName = userNameProperty.DefaultValue;
        protected string _OrderNumber = orderNumberProperty.DefaultValue;
        protected SmartDate _OrderDate = orderDateProperty.DefaultValue;
        protected string _CardType = cardTypeProperty.DefaultValue;
        protected string _CardHolder = cardHolderProperty.DefaultValue;
        protected string _CreditCard = creditCardProperty.DefaultValue;
        protected string _ExpDate = expDateProperty.DefaultValue;

        #endregion

        #region Public properties

        [DataObjectField(true, false)]
        public virtual Guid OrderID
        {
            get
            {
                return GetProperty<Guid>(orderIDProperty, _OrderID);
            }
        }

        [Required]
        [StringLength(50)]
        public virtual string UserName
        {
            get
            {
                return GetProperty<string>(userNameProperty, _UserName);
            }
            set
            {
                SetProperty<string>(userNameProperty, ref _UserName, value);
            }
        }

        [Required]
        public virtual string OrderNumber
        {
            get
            {
                return GetProperty<string>(orderNumberProperty, _OrderNumber);
            }
            set
            {
                SetProperty<string>(orderNumberProperty, ref _OrderNumber, value);
            }
        }

        public virtual SmartDate OrderDate
        {
            get
            {
                return GetProperty<SmartDate>(orderDateProperty, _OrderDate);
            }
            set
            {
                SetProperty<SmartDate>(orderDateProperty, ref _OrderDate, value);
            }
        }

        [Required]
        public virtual string CardType
        {
            get
            {
                return GetProperty<string>(cardTypeProperty, _CardType);
            }
            set
            {
                SetProperty<string>(cardTypeProperty, ref _CardType, value);
            }
        }

        [Required]
        public virtual string CardHolder
        {
            get
            {
                return GetProperty<string>(cardHolderProperty, _CardHolder);
            }
            set
            {
                SetProperty<string>(cardHolderProperty, ref _CardHolder, value);
            }
        }

        [Required]
        [StringLength(16)]
        public virtual string CreditCard
        {
            get
            {
                return GetProperty<string>(creditCardProperty, _CreditCard);
            }
            set
            {
                SetProperty<string>(creditCardProperty, ref _CreditCard, value);
            }
        }

        [Required]
        public virtual string ExpDate
        {
            get
            {
                return GetProperty<string>(expDateProperty, _ExpDate);
            }
            set
            {
                SetProperty<string>(expDateProperty, ref _ExpDate, value);
            }
        }

        public OrderDetailCollection OrderDetailList
        {
            get
            {
                if (!(FieldManager.FieldExists(orderDetailListProperty)))
                    SetProperty<OrderDetailCollection>(orderDetailListProperty, orderDetailListProperty.DefaultValue);

                return GetProperty<OrderDetailCollection>(orderDetailListProperty);
            }
        }

        #endregion

        #region Read/Load overloads -  Required for Rules to work properly with private backing fields

        //protected override object ReadProperty(Csla.Core.IPropertyInfo propertyInfo)
        //{
        //      using (BypassPropertyChecks) {
        //           return MethodCaller.CallPropertyGetter(this, propertyInfo.Name);
        //      }
        //}

        //protected override void LoadProperty(Csla.Core.IPropertyInfo propertyInfo, object newValue)
        //{
        //      using (BypassPropertyChecks) {
        //           MethodCaller.CallPropertySetter(this, propertyInfo.Name, newValue);
        //      }
        //}

        #endregion

        #region Factory methods

        public static Order NewOrder()
        {
            return DataPortal.Create<Order>();
        }

        public static Order GetOrder(Guid orderID)
        {
            return DataPortal.Fetch<Order>(new SingleCriteria<Order, Guid>(orderID));
        }

        public static void DeleteOrder(Guid orderID)
        {
            DataPortal.Delete<Order>(new SingleCriteria<Order, Guid>(orderID));
        }
        
        public static Order GetOrderWithDetail(string orderNumber)
        {
            return DataPortal.Fetch<Order>(
                new GetOrderWithDetailCriteria(orderNumber));
        }

        public static Order GetOrderWithDetail(Guid orderID)
        {
            return DataPortal.Fetch<Order>(
                new GetOrderWithDetailCriteria(orderID));
        }

        internal static Order GetOrder(SafeDataReader reader)
        {
            return DataPortal.FetchChild<Order>(reader);
        }

        public OrderDetail NewOrderDetail()
        {
            OrderDetail orderDetail = OrderDetail.NewOrderDetail();
            orderDetail.OrderID = this.OrderID;
            return orderDetail;
        }
        
        #endregion

        #region Criteria classes

        [Serializable()]
        protected class GetOrderWithDetailCriteria
        {
            public GetOrderWithDetailCriteria() { }

            public GetOrderWithDetailCriteria(Guid orderID)
            {
                _OrderID = orderID;
            }

            public GetOrderWithDetailCriteria(string orderNumber)
            {
                _OrderNumber = orderNumber;
            }

            private Guid _OrderID;
            private string _OrderNumber = string.Empty;

            public Guid OrderID
            {
                get
                {
                    return _OrderID;
                }
            }

            public string OrderNumber
            {
                get
                {
                    return _OrderNumber;
                }
            }
        }

        #endregion

        #region Data access



        [RunLocal()]
        protected override void DataPortal_Create()
        {
            _OrderID = Guid.NewGuid();
            BusinessRules.CheckRules();
        }

        protected void DataPortal_Fetch(SingleCriteria<Order, Guid> criteria)
        {
            using (SafeDataReader reader = new DataAccess().OrderGetById(criteria.Value))
            {
                if(reader.Read())
                    Fetch(reader);
            }
        }

        protected void Child_Fetch(SafeDataReader reader)
        {
            Fetch(reader);
        }

        protected void Fetch(SafeDataReader reader)
        {
            _OrderID = reader.GetGuid("OrderID");
            _UserName = reader.GetString("UserName");
            _OrderNumber = reader.GetString("OrderNumber");
            _OrderDate = reader.GetSmartDate("OrderDate");
            _CardType = reader.GetString("CardType");
            _CardHolder = reader.GetString("CardHolder");
            _CreditCard = reader.GetString("CreditCard");
            _ExpDate = reader.GetString("ExpDate");
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            using (SafeDataReader reader = new DataAccess().OrderInsert(
                _OrderID, _UserName, _OrderNumber, _OrderDate, _CardType, _CardHolder, _CreditCard, _ExpDate))
            {
                reader.Close();
                FieldManager.UpdateChildren(this);
            }
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            using (SafeDataReader reader = new DataAccess().OrderUpdate(
                _OrderID, _UserName, _OrderNumber, _OrderDate, _CardType, _CardHolder, _CreditCard, _ExpDate))
            {
                reader.Close();
                FieldManager.UpdateChildren(this);
            }
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new SingleCriteria<Order, Guid>(_OrderID));
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(SingleCriteria<Order, Guid> criteria)
        {
            using (DataAccess data = new DataAccess())
            {
                FieldManager.UpdateChildren(this);
                using (SafeDataReader reader = data.OrderDelete(criteria.Value)) { }
            }
        }

        protected void DataPortal_Fetch(GetOrderWithDetailCriteria criteria)
        {
            using (DataAccess dal = new DataAccess())
            {
                SafeDataReader reader = null;
                if (criteria.OrderNumber == "")
                    reader = dal.OrderGetWithDetail(criteria.OrderID);
                else
                    reader = dal.OrderGetWithDetailByOrderNumber(criteria.OrderNumber);

                using (reader)
                {
                    if(reader.Read())
                    {
                        Fetch(reader);
                        reader.NextResult();
                        
                        LoadProperty<OrderDetailCollection>(orderDetailListProperty,
                            OrderDetailCollection.GetOrderDetailCollection(reader));
                    }
                }
            }
        }

        protected void Child_Insert()
        {
            DataPortal_Insert();
        }

        protected void Child_Update()
        {
            DataPortal_Update();
        }

        protected void Child_DeleteSelf()
        {
            DataPortal_DeleteSelf();
        }

		protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
		{
			DataAccess.ClearConnection();
		}

        #endregion

    }
}
