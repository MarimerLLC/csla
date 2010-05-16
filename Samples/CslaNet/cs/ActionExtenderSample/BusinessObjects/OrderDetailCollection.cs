using System;
using Csla;
using Csla.Data;
using Csla.Rules;
using Csla.Security;

namespace CslaStore.Business
{
    [Serializable()]
    public class OrderDetailCollection : Csla.BusinessBindingListBase<OrderDetailCollection, OrderDetail>
    {
        #region Constructors

        private OrderDetailCollection()
        {
        }

        #endregion

        #region Authorization rules

        protected static void AddObjectAuthorizationRules()
        {
            BusinessRules.AddRule(typeof(OrderDetailCollection), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.GetObject, ("user")));
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
