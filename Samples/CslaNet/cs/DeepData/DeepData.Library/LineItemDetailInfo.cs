using System;
using Csla;
using Csla.Data;

namespace DeepData.Library
{
	///<summary>
	///</summary>
	[System.Serializable()]
	public partial class LineItemDetailInfo
	  : ReadOnlyBase<LineItemDetailInfo>
	{
		#region " Business Methods "

		private int _orderId;
		///<summary>
		///</summary>
		public int OrderId
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("OrderId", true);
				return _orderId;
			}
		}

		private int _lineId;
		///<summary>
		///</summary>
		public int LineId
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("LineId", true);
				return _lineId;
			}
		}

		private int _id;
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

		private string _detail = "";
		///<summary>
		///</summary>
		public string Detail
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
			get
			{
				CanReadProperty("Detail", true);
				return _detail;
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
			return string.Format("{0}::{1}::{2}", _orderId, _lineId, _id);
		}

		#endregion

		#region " Factory Methods "

		internal static LineItemDetailInfo GetItem(SafeDataReader data)
		{

			return new LineItemDetailInfo(data);

		}

		private LineItemDetailInfo() { }

		private LineItemDetailInfo(SafeDataReader data)
		{

			Fetch(data);

		}

		#endregion

		#region " Data Access "

		private void Fetch(SafeDataReader data)
		{
			_orderId = data.GetInt32("OrderId");
			_lineId = data.GetInt32("LineId");
			_id = data.GetInt32("Id");
			_detail = data.GetString("Detail");

		}

		#endregion

	}
}