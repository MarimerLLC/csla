using Csla.Data;

namespace DeepData.Library
{

	///<summary>
	///</summary>
	public partial class LineItemList
	{
		internal void LoadItems(SafeDataReader data)
		{
			IsReadOnly = false;
			while (data.Read())
			{
				Add(LineItemInfo.GetItem(data));
			}
			IsReadOnly = true;

		}

	}
}