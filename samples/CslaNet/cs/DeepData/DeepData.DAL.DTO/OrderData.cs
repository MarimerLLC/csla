using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Csla.Data;
using DeepData.DTO;

namespace DeepData.DAL.DTO
{
	///<summary>
	///</summary>
	public class OrderData : DeepData.DAL.OrderData
	{
		private List<OrderDto> _orders;
		private SafeDataReader _data;

		///<summary>
		///</summary>
		public OrderData()
		{
			using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DeepDataDAL"].ConnectionString))
			{
				cn.Open();
				using (var cm = cn.CreateCommand())
				{
					cm.CommandText =
						"SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail";
					cm.CommandType = CommandType.Text;
					using (var data = new SafeDataReader(cm.ExecuteReader()))
					{
						_data = data;
						_orders = new List<OrderDto>();
						while (data.Read())
						{
							OrderDto item = new OrderDto();
							item.Id = data.GetInt32("Id");
							item.Customer = data.GetString("Customer");
							_orders.Add(item);
						}

						data.NextResult();
						while (data.Read())
							LoadItem(data);

						data.NextResult();
						while (data.Read())
							LoadDetail(data);
					}
				}
			}
		}

		private void LoadItem(SafeDataReader data)
		{
			var orderId = data.GetInt32("OrderId");
			foreach (var order in _orders)
			{
				if (order.Id != orderId) continue;
				var item = new LineItemDto
									{
										OrderId = orderId,
										Id = data.GetInt32("Id"),
										Product = data.GetString("Product")
									};
				order.OrderLinesList.Add(item);
			}
		}

		private void LoadDetail(SafeDataReader data)
		{
			var orderId = data.GetInt32("OrderId");
			foreach (var order in _orders)
			{
				if (order.Id != orderId) continue;
				var lineId = data.GetInt32("LineId");
				foreach (var line in order.OrderLinesList)
				{
					if (line.Id != lineId) continue;
					var item = new DetailItemDto
								{
									OrderId = orderId,
									LineId = lineId,
									Id = data.GetInt32("Id"),
									Detail = data.GetString("Detail")
								};
					line.OrderLineDetailsList.Add(item);
				}
			}
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public override object GetOrders()
		{
			return _data;
		}
	}
}
