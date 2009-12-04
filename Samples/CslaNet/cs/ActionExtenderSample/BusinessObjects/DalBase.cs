using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Csla;
using Csla.Data;

/* 
 * Usage of this class:
 * 
 * This class serves as the base class for project DAL classes which are designed
 * to wrap stored procedures.
 * 
 * The class allows the use of any ADO.NET provider within CSLA.
 *
 * The constructors for a project DAL class need to look like this:
 * 
 *      public DataAccess()
            : base("ConnStr") { }

 * Note that "ConnStr" is the <connectionStrings> key.
 * 
 * An example of one of the methods in a project DAL class (DataAccess.cs) looks like this:
 * 
        public SafeDataReader ProductGetById(Guid productID)
		{
			IDataParameter[] parameters =
			{
				GetParameter("@p_ProductID", productID) as IDataParameter
			};

			return GetReader("ProductGetById", CommandType.StoredProcedure, parameters);
		}
 * 
 * Usage from a CSLA business object would look like this:
 * 
        using (SafeDataReader reader = new DataAccess().ProductGetById(criteria.ProductID))
            Fetch(reader);
 * 
 * When the DataAccess class is instantiated, a LocalContext store is checked for an existing 
 * connection and if found to be populated, said connection will be used instead of opening
 * a new one.  When this particular instance of the DataAccess class is disposed, the
 * LocalContext store will be cleared.  If calls to creating new instances of DataAccess
 * like in a parent/child situation are performed, they will reuse the existing connection.
 * 
        using (DataAccess dal = new DataAccess())
        {
            using (SafeDataReader reader = dal.OrderGetWithDetailByOrderNumber(criteria.OrderNumber))
            {
                Fetch(reader);
                _OrderDetails = OrderDetailCollection.GetOrderDetailCollection(_OrderID);
            }
        }
 * 
 * At this point, the connection closed here and LocalContext cleared since this is where it started.
 * If the "false" were left out from here, the connection would be closed after the reader is iterated
 * after the Fetch.  In this case, the instantiation of the DataAccess in the OrderDetailCollection
 * object would open its own connection.  Of course in this example, a much more efficient method would
 * be to do a reader.NextResult() and send that to the OrderDetailCollection object.
 * 
 * For updating:
 * 
 * Order object:
        using (DataAccess dal = new DataAccess())
        {
            SafeDataReader reader = dal.OrderUpdate(_OrderID, _OrderNumber, etc...);
            _OrderDetails.Update();
        }
 * 
 * OrderDetail object (Update method):
        SafeDataReader reader = new DataAccess().OrderDetailUpdate(_OrderDetailID, _OrderID, etc...);
 * 
 * The above statement will use the stored connection even though we're using the default
 * constructor on DataAccess because it will find a connection in the LocalContext put there by Order.
 * 
 * Remember, this class (and its derived project DAL classes - DataAccess) is meant to hide ADO.NET plumbing.
 * In the case where a business object wants to ensure that the LocalContext is cleared,
 * it has a static method called ClearConnection that can be used.
 * 
 * The connection string store in LocalContext is keyed using the connection string name so that
 * when case where multiple databases are accessed, each with their own DalBase-derived class, their
 * connection strings will be treated individually in the above-described functionality.
 * 
 * IMPORTANT:
 * Objects using this DAL class MUST override DataPortal_OnDataPortalInvokeComplete and access the
 * "ClearConnection" static method in this class.  This method requires the connection string name
 * that was used to establish the DAL class.  A best practice is to place a static method in the
 * derived DataAccess class that wraps this call and hides the connection string name from the client.
 * This is necessary in the case the object code does not place the instantiation of the DataAccess
 * class in "using" statement and instead places the direct call to the reader-retrieval method.  Examples
 * of both are shown above.
 * 
*/

namespace CslaEx.Data
{
    public abstract class DalBase : IDisposable
    {
        protected DalBase() :
            this("connStr")
        {
        }

        protected DalBase(string connectionStringName)
        {
			_ConnStr = connectionStringName;

            string connStr = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (string.IsNullOrEmpty(connStr))
                connStr = "";

            if (Csla.ApplicationContext.LocalContext.Contains("conn_" + _ConnStr) &&
                Csla.ApplicationContext.LocalContext["conn_" + _ConnStr] != null &&
                (Csla.ApplicationContext.LocalContext["conn_" + _ConnStr] as IDbConnection).State == ConnectionState.Open)
            {
                _Conn = Csla.ApplicationContext.LocalContext["conn_" + _ConnStr] as IDbConnection;
                _ConnectionFromStore = true;
            }
            else
            {
                string provider = ConfigurationManager.AppSettings["dbProvider"];
                if (string.IsNullOrEmpty(provider))
                    provider = "System.Data.SqlClient";

                DbProviderFactory factory = DbProviderFactories.GetFactory(provider);

                _Conn = factory.CreateConnection();
                _Conn.ConnectionString = connStr;
                _Conn.Open();

				Csla.ApplicationContext.LocalContext["conn_" + _ConnStr] = _Conn;

                _ConnectionFromStore = false;
            }

            _Cmd = _Conn.CreateCommand();
        }

        ~DalBase()
        {
            Dispose(false);
        }

        private bool _Disposed = false;
        private bool _ConnectionFromStore = false; // determines if connection was opened or obtained from LocalContext
		private string _ConnStr = string.Empty;

        protected IDbConnection _Conn = null;
        protected IDbCommand _Cmd = null;

        public static void ClearConnection(string connectionString)
        {
			if (Csla.ApplicationContext.LocalContext.Contains("conn_" + connectionString))
			{
				IDbConnection conn = Csla.ApplicationContext.LocalContext["conn_" + connectionString] as IDbConnection;
				if (conn != null)
					conn.Dispose();

				Csla.ApplicationContext.LocalContext.Remove("conn_" + connectionString);
			}
        }

        private IDbCommand CreateCommand(string sqlText, CommandType cmdType)
        {
            return CreateCommand(sqlText, null, cmdType);
        }

        private IDbCommand CreateCommand(string sqlText, IDataParameter[] dbParams, CommandType cmdType)
        {
            _Cmd.CommandText = sqlText;
            _Cmd.CommandType = cmdType;
            return CreateCommand(_Conn, _Cmd, dbParams);
        }

        private IDbCommand CreateCommand(IDbConnection conn, IDbCommand cmd, IDataParameter[] cmdParams)
        {
            if (cmdParams != null)
            {
                foreach (IDbDataParameter param in cmdParams)
                    cmd.Parameters.Add(param);
            }

            return cmd;
        }

        protected IDataParameter GetParameter(string paramName, object paramValue)
        {
            return GetParameter(paramName, paramValue, ParameterDirection.Input);
        }

        protected IDataParameter GetParameter(string paramName, object paramValue, ParameterDirection direction)
        {
            IDataParameter param = _Cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            param.Direction = direction;
            return param;
        }

        protected SafeDataReader GetReader(string sqlText, CommandType cmdType)
        {
            return GetReader(sqlText, cmdType, null);
        }

        protected SafeDataReader GetReader(string sqlText, CommandType cmdType, IDataParameter[] param)
        {
            return GetReader(CreateCommand(sqlText, param, cmdType));
        }

        protected SafeDataReader GetReader(IDbCommand cmd)
        {
            return new SafeDataReader(cmd.ExecuteReader());
        }

        protected int ExecuteSql(string sqlText, IDbDataParameter[] dbParams)
        {
            return ExecuteSql(CreateCommand(sqlText, dbParams, CommandType.StoredProcedure));
        }

        protected int ExecuteSql(IDbCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    if (!_ConnectionFromStore)
                    {
                        // clear this if the connection was not obtained from this store,
                        // meaning the holder of his object was the original opener
                        if (Csla.ApplicationContext.LocalContext.Contains("conn_" + _ConnStr))
                            Csla.ApplicationContext.LocalContext.Remove("conn_" + _ConnStr);

                      _Conn.Dispose();
                    }
                }

                _Disposed = true;
            }
        }

    }
}
