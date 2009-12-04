using Csla.Data;

namespace DeepData.Library
{
	///<summary>
	///</summary>
	public partial class LineItemDetailList
	{
		internal void LoadDetail(SafeDataReader data)
		{
			IsReadOnly = false;
			while (data.Read())
			{
				Add(LineItemDetailInfo.GetItem(data));
			}
			IsReadOnly = true;

		}

	}
}