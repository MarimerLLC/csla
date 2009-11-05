using System.Collections.Generic;

namespace DeepData.DTO
{
	///<summary>
	///</summary>
	public class LineItemDto
	{
		///<summary>
		///</summary>
		public int OrderId { get; set; }

		///<summary>
		///</summary>
		public int Id { get; set; }

		///<summary>
		///</summary>
		public string Product { get; set; }

		private List<DetailItemDto> _detailItems;
		///<summary>
		///</summary>
		[System.Xml.Serialization.XmlIgnore]
		public List<DetailItemDto> OrderLineDetailsList
		{
			get
			{
				if (_detailItems == null)
				{
					_detailItems = new List<DetailItemDto>();
				}
				return _detailItems;
			}
		}

		///<summary>
		///</summary>
		public DetailItemDto[] OrderLineDetails
		{
			get
			{
				return OrderLineDetailsList.ToArray();
			}
			set
			{
				_detailItems = new List<DetailItemDto>(value);
			}
		}

	}
}
