using Csla;
using Csla.Data;

namespace DeepData.Library
{
	[global::System.Serializable()]
	public partial class LineItemDetailList
	  : ReadOnlyListBase<LineItemDetailList, LineItemDetailInfo>
	{

	#region " Factory Methods "

	  internal static LineItemDetailList NewList()
	  {
	  	return new LineItemDetailList();

	  }

		private LineItemDetailList(){}

	#endregion

	#region " Data Access "

	  internal void LoadChild(SafeDataReader data)
	  {
	  	IsReadOnly = false;
	  	Add(LineItemDetailInfo.GetItem(data));
	  	IsReadOnly = true;
	  }

	#endregion

	}
}