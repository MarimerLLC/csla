using Csla.Data;
using DeepData.DAL;

namespace DeepData.DAL.WebService
{
	///<summary>
	///</summary>
	public class OrderData : DAL.OrderData
	{
		///<summary>
		///</summary>
		///<returns></returns>
		public override object GetOrders()
		{
			var svc = new DeepData.DAL.WebService.DeepDataDAL.DeepDataDAL();
			return svc.GetOrders();
		}
	}
}