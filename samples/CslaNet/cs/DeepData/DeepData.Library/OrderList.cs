using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace DeepData.Library
{
	public partial class OrderList
	  : ReadOnlyListBase<OrderList, OrderInfo>
	{
		#region " Business Methods "

		///<summary>
		///</summary>
		///<param name="id"></param>
		///<returns></returns>
		public OrderInfo FindById(int id)
		{
			foreach (var item in this)
			{
				if (item.Id == id) return item;
			}
			return null;
		}

		#endregion

		#region " Factory Methods "

		///<summary>
		///</summary>
		///<returns></returns>
		public static OrderList GetList()
		{
			return DataPortal.Fetch<OrderList>();
		}

		private OrderList() { }

		#endregion

		#region " Data Access "

		private void DataPortal_Fetch()
		{
			string dalTypeName = ConfigurationManager.AppSettings["OrderData"];
			if (String.IsNullOrEmpty(dalTypeName))
			{
				Fetch();
			}
			else if (dalTypeName.IndexOf("DAL.Direct") > 0)
			{
				FetchDal();
			}
			else
			{
				FetchDto();
			}
		}

		private void Fetch()
		{
			try
			{
				RaiseListChangedEvents = false;
				IsReadOnly = false;
				using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DeepDataDAL"].ConnectionString))
				{
					cn.Open();
					using (var cm = cn.CreateCommand())
					{
						cm.CommandText = "SELECT id,customer FROM [Order];SELECT orderid,id,product FROM OrderLine;SELECT orderid,lineid,id,detail FROM OrderLineDetail";
						cm.CommandType = CommandType.Text;
						using (var data = new SafeDataReader(cm.ExecuteReader()))
						{
							LoadOrders(data);
						}
					}
				}
			}
			catch
			{
				Exception ex;
			}
			finally
			{
				IsReadOnly = true;
				RaiseListChangedEvents = true;
			}

		}

		private void FetchDal()
		{
			try
			{
				RaiseListChangedEvents = false;
				IsReadOnly = false;
				DeepData.DAL.DataFactory df = new DAL.DataFactory();
				using (var dal = df.GetOrderDataObject())
				{
					SafeDataReader data = ((SafeDataReader)(dal.GetOrders()));
					LoadOrders(data);
				}

			}
			finally
			{
				IsReadOnly = true;
				RaiseListChangedEvents = true;
			}

		}

		private void LoadOrders(SafeDataReader data)
		{
			while (data.Read())
				Add(OrderInfo.GetOrderInfo(data));

			data.NextResult();
			while (data.Read())
				FindById(data.GetInt32("OrderId")).LoadItem(data);

			data.NextResult();
			while (data.Read())
				FindById(data.GetInt32("OrderId")).LoadDetail(data);

		}

		#endregion

	}

}