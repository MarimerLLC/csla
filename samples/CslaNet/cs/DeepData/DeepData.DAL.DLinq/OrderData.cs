using System.Data;
using System.Linq;
using Csla.Data;

namespace DeepData.DAL.DLinq
{
	///<summary>
	///</summary>
	public class OrderData : DeepData.DAL.OrderData
	{
		private DataClasses1DataContext db = new DataClasses1DataContext();

		///<summary>
		///</summary>
		///<returns></returns>
		public override object GetOrders()
		{
			var q = from o in db.Orders select o;
			IDbCommand command = db.GetCommand(q);
			command.Connection = db.Orders.Context.Connection;
			command.Connection.Open();
			SafeDataReader reader = (SafeDataReader)command.ExecuteReader(CommandBehavior.CloseConnection);
			return reader;
		}

		#region " IDisposable "

		///<summary>
		///</summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
		}

		#endregion

	}
}
