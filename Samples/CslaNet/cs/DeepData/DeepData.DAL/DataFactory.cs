using System;
using System.Configuration;

namespace DeepData.DAL
{
	///<summary>
	///</summary>
	public class DataFactory
	{
		///<summary>
		///</summary>
		///<returns></returns>
		public OrderData GetOrderDataObject()
		{
			var appset = ConfigurationManager.AppSettings;
			var dal = appset.Get("OrderData");
			if (!string.IsNullOrEmpty(dal))
			{
				var dalType = Type.GetType(dal, true, true);
				return (OrderData)(Activator.CreateInstance(dalType));
			}
			return null;
		}

	}
}