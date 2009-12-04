using System;
using Csla;
using Csla.Data;
using Csla.Security;

namespace CslaStore.Business
{
    [Serializable()]
    public class OrderDetailCollection : BusinessListBase<OrderDetailCollection, OrderDetail>
    {
        #region Constructors

        private OrderDetailCollection()
        {
        }

        #endregion

        #region Authorization rules

        protected static void AddObjectAuthorizationRules()
        {
            AuthorizationRules.AllowGet(typeof(OrderDetailCollection), "user");
        }

        #endregion

        #region Factory methods

        internal static OrderDetailCollection NewOrderDetailCollection()
        {
            return DataPortal.CreateChild<OrderDetailCollection>();
        }

        internal static OrderDetailCollection GetOrderDetailCollection(SafeDataReader reader)
        {
            return DataPortal.FetchChild<OrderDetailCollection>(reader);
        }

        #endregion

        #region Data access

        protected void Child_Fetch(SafeDataReader reader)
        {
            this.RaiseListChangedEvents = false;

            while (reader.Read())
                this.Add(OrderDetail.GetOrderDetail(reader));

            this.RaiseListChangedEvents = true;
        }

        #endregion

        #region Other methods

        public OrderDetail FindByID(Guid id)
        {
            OrderDetail found = null;

            foreach(OrderDetail detail in this)
            {
                if (detail.OrderDetailID == id)
                {
                    found = detail;
                    break;
                }

            }

            return found;
        }

        #endregion
    }
}
