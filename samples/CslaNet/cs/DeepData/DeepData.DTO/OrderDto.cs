using System.Collections.Generic;

namespace DeepData.DTO
{
	///<summary>
	///</summary>
	public class OrderDto
	{
		private int _id;
		///<summary>
		///</summary>
		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		private string _customer;
		///<summary>
		///</summary>
		public string Customer
		{
			get
			{
				return _customer;
			}
			set
			{
				_customer = value;
			}
		}

		private List<LineItemDto> _lineItems;
		///<summary>
		///</summary>
		[System.Xml.Serialization.XmlIgnore()]
		public List<LineItemDto> OrderLinesList
		{
			get
			{
				if (_lineItems == null)
				{
					_lineItems = new List<LineItemDto>();
				}
				return _lineItems;
			}
		}

		///<summary>
		///</summary>
		public LineItemDto[] OrderLines
		{
			get
			{
				return OrderLinesList.ToArray();
			}
			set
			{
				_lineItems = new List<LineItemDto>(value);
			}
		}

	}
}
