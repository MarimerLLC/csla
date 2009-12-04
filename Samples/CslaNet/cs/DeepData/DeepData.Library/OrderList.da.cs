using Csla.Data;
using DeepData.DTO;

namespace DeepData.Library
{
	///<summary>
	///</summary>
	public partial class OrderList
	{
		private void FetchDto()
		{
			try
			{
				RaiseListChangedEvents = false;
				IsReadOnly = false;
				DeepData.DAL.DataFactory df = new DeepData.DAL.DataFactory();
				//Dim data() As DeepData.DTO.OrderDto
				DeepData.DTO.OrderDto[] data; //() As Object
				using (DeepData.DAL.OrderData dal = df.GetOrderDataObject())
				{
					//data = CType(dal.GetOrders, DeepData.DTO.OrderDto())
					data = ((DeepData.DTO.OrderDto[])(dal.GetOrders()));
					if (data != null)
					{
						foreach (var order in data)
						{
							Add(OrderInfo.GetOrderInfo(order));
						}
					}
				}
			}
			finally
			{
				IsReadOnly = true;
				RaiseListChangedEvents = true;
			}

		}

	}

}