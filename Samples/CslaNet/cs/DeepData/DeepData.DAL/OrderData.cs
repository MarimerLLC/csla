using System;

namespace DeepData.DAL
{
	///<summary>
	///</summary>
	public abstract class OrderData : IDisposable
	{
		///<summary>
		///</summary>
		///<returns></returns>
		public abstract object GetOrders();

		#region " IDisposable Support "

		private bool disposed;    // To detect redundant calls

		// IDisposable
		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
				}
			}
			disposed = true;
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		public void Dispose()
		{
			// Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}