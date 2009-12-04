using System.Web.Services;
using Csla.Data;
using DeepData.DAL;
using DeepData.DTO;

[WebService(Namespace = "http://ws.lhotka.net/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class DeepDataDAL : WebService
{
	[WebMethod]
	public DeepData.DTO.OrderDto[] GetOrders()
	{
		var df = new DataFactory();
		using (var dal = df.GetOrderDataObject())
		{
			return ((DeepData.DTO.OrderDto[])(dal.GetOrders()));
		}
	}
}
