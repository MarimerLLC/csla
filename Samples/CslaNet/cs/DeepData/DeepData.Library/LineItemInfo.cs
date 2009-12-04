using Csla;
using Csla.Data;
using System;

namespace DeepData.Library
{
	///<summary>
	///</summary>
	[Serializable]
	public partial class LineItemInfo : ReadOnlyBase<LineItemInfo>
	{
		#region " Business Methods "

		private int _orderId;
		///<summary>
		///</summary>
		public int OrderId
		{
			[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("OrderId", true);
				return _orderId;
			}
		}

		private int _id;
		///<summary>
		///</summary>
		public int Id
		{
			[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("Id", true);
				return _id;
			}
		}

		private string _product = "";
		///<summary>
		///</summary>
		public string Product
		{
			[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("Product", true);
				return _product;
			}
		}

		private LineItemDetailList _detailList = LineItemDetailList.NewList();
		///<summary>
		///</summary>
		public LineItemDetailList Details
		{
			get
			{
				return _detailList;
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
			return string.Format("{0}::{1}", _orderId, _id);
		}

		#endregion

		#region " Factory Methods "

		internal static LineItemInfo GetItem(SafeDataReader data)
		{
			return new LineItemInfo(data);

		}

		private LineItemInfo()
		{

		}

		private LineItemInfo(SafeDataReader data)
		{

			Fetch(data);

		}

		#endregion

		#region " Data Access "

		private void Fetch(SafeDataReader data)
		{
			_orderId = data.GetInt32("OrderId");
			_id = data.GetInt32("Id");
			_product = data.GetString("Product");

		}

		internal void LoadDetail(SafeDataReader data)
		{
			_detailList.LoadChild(data);

		}

		#endregion

	}
}