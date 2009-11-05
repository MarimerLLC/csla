using Csla;
using Csla.Data;

namespace DeepData.Library
{
	[System.Serializable()]
	public partial class LineItemList
	  : ReadOnlyListBase<LineItemList, LineItemInfo>
	{
		#region " Business Methods "

		///<summary>
		///</summary>
		///<param name="id"></param>
		///<returns></returns>
		public LineItemInfo FindById(int id)
		{
			foreach (LineItemInfo child in this)
			{
				if (child.Id == id)
				{
					return child;
				}
			}
			return null;
		}

		#endregion

		#region " Factory Methods "

		internal static LineItemList NewList()
		{
			return new LineItemList();
		}

		private LineItemList()
		{
		}

		#endregion

		#region " Data Access "

		internal void LoadChild(SafeDataReader data)
		{
			IsReadOnly = false;
			Add(LineItemInfo.GetItem(data));
			IsReadOnly = true;
		}

		#endregion

	}
}
