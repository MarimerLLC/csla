using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Csla.Data;

namespace DeepData.DAL.Direct
{
	///<summary>
	///</summary>
	public class OrderData
	  : DeepData.DAL.OrderData
	{
		private SqlConnection _cn;
		private SqlCommand _cm;
		private SafeDataReader _data;

		///<summary>
		///</summary>
		public OrderData()
		{
			_cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DeepDataDAL"].ConnectionString);
			_cn.Open();
			_cm = _cn.CreateCommand();
			_cm.CommandText =
				"SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail";
			_cm.CommandType = CommandType.Text;
			_data = new SafeDataReader(_cm.ExecuteReader());
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public override object GetOrders()
		{
			return _data;
		}


		#region " IDisposable Support "

		// IDisposable
		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_data.Dispose();
				_cm.Dispose();
				_cn.Dispose();
			}
		}

		#endregion

	}
}