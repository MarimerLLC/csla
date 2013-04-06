using System;
using Csla;
using Csla.Data;
using Csla.Rules;
using Csla.Security;
using CslaStore.Data;
using Csla.Rules.CommonRules;

namespace CslaStore.Business
{
    [Serializable()]
    public class OrderCollection 
        : DynamicBindingListBase<Order>
        // : BusinessListBase<OrderCollection, Order>
    {
        #region Constructors

        private OrderCollection()
        {
        }
        
        #endregion

        #region Authorization rules

        protected static void AddObjectAuthorizationRules()
        {
            BusinessRules.AddRule(typeof(OrderCollection), new IsInRole(AuthorizationActions.GetObject, ("admin"))) ;
        }

        #endregion

        #region Factory methods

        public static OrderCollection GetOrderCollection()
        {
            return DataPortal.Fetch<OrderCollection>(new GetAllCriteria());
        }

        public static OrderCollection GetForUser(string userName)
        {
            return DataPortal.Fetch<OrderCollection>(new GetForUserCriteria(userName));
        }

        #endregion

        #region Criteria classes

        [Serializable()]
        protected class GetAllCriteria { }

        [Serializable()]
        protected class GetForUserCriteria
        {
            public GetForUserCriteria() { }

            public GetForUserCriteria(string userName)
            {
                _UserName = userName;
            }

            private string _UserName = string.Empty;

            public string UserName
            {
                get
                {
                    return _UserName;
                }
            }
        }

        #endregion

        #region Data access

        protected void DataPortal_Fetch(GetAllCriteria criteria)
        {
            using (SafeDataReader reader = new DataAccess().OrderGetAll())
                Fetch(reader);
        }

        protected void DataPortal_Fetch(GetForUserCriteria criteria)
        {
            using (SafeDataReader reader = new DataAccess().OrderGetForUser(criteria.UserName))
                Fetch(reader);
        }

        protected void Fetch(SafeDataReader reader)
        {
            this.RaiseListChangedEvents = false;

            while (reader.Read())
            {
                Order order = Order.GetOrder(reader);
                this.Add(order);
            }
    
            this.RaiseListChangedEvents = true;
        }

        //[Transactional(TransactionalTypes.TransactionScope)]
        //protected override void DataPortal_Update()
        //{
        //    using (DataAccess dal = new DataAccess())
        //      Child_Update();
        //}

        #endregion
        
    }
}
