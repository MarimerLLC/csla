using Csla;
using Csla.Data;

namespace DeepData.Library
{
	///<summary>
	///</summary>
	[System.Serializable()]
	public partial class OrderInfo
	  : ReadOnlyBase<OrderInfo>
	{
		#region " Business Methods "

		int _id;
		///<summary>
		///</summary>
		public int Id
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("Id", true);
				return _id;
			}
		}

		private string _customer = "";
		///<summary>
		///</summary>
		public string Customer
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("Customer", true);
				return _customer;
			}
		}

		private LineItemList _lineItems = LineItemList.NewList();
		///<summary>
		///</summary>
		public LineItemList LineItems
		{
			get
			{
				return _lineItems;
			}
		}

		///<summary>
		///
		///            Override this method to return a unique identifying
		///            value for this object.
		///            
		///</summary>
		///
		protected override object GetIdValue()
		{
			return _id;
		}

		#endregion

		#region " Factory Methods "

		internal static OrderInfo GetOrderInfo(object data)
		{
			return new OrderInfo((Csla.Data.SafeDataReader)(data));

		}

		private OrderInfo() { }

		private OrderInfo(SafeDataReader data)
		{
			Fetch(data);

		}
		#endregion

		#region " Data Access "

		private void Fetch(SafeDataReader data)
		{
			_id = data.GetInt32("Id");
			_customer = data.GetString("Customer");

		}

		internal void LoadItem(SafeDataReader data)
		{
			_lineItems.LoadChild(data);

		}

		internal void LoadDetail(SafeDataReader data)
		{
			_lineItems.FindById(data.GetInt32("LineId")).LoadDetail(data);

		}

		#endregion

	}
}