using System;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Csla.Server.Hosts
{
    // in asmx use web directive like
    // <%@ WebService Class="Csla.Server.Hosts.WebServicePortal" %?
    /// <summary>
    /// Exposes server-side DataPortal functionality
    /// through Web Services (asmx).
    /// </summary>
    [WebService(Namespace="http://ws.lhotka.net/Csla")]
    public class WebServicePortal : WebService
    {

        #region Request classes

        [Serializable()]
        public class CreateRequest
        {

            private Type _objectType;
            public Type ObjectType
            {
                get { return _objectType; }
                set { _objectType = value; }
            }

            private object _criteria;
            public object Criteria
            {
                get { return _criteria; }
                set { _criteria = value; }
            }

            private Server.DataPortalContext _context;
            public Server.DataPortalContext Context
            {
                get { return _context; }
                set { _context = value; }
            }
        }

        [Serializable()]
        public class FetchRequest
        {
            private object _criteria;
            public object Criteria
            {
                get { return _criteria; }
                set { _criteria = value; }
            }

            private Server.DataPortalContext _context;
            public Server.DataPortalContext Context
            {
                get { return _context; }
                set { _context = value; }
            }
        }

        [Serializable()]
        public class UpdateRequest
        {
            private object _object;
            public object Object
            {
                get { return _object; }
                set { _object = value; }
            }

            private Server.DataPortalContext _context;
            public Server.DataPortalContext Context
            {
                get { return _context; }
                set { _context = value; }
            }
        }

        [Serializable()]
        public class DeleteRequest
        {
            private object _criteria;
            public object Criteria
            {
                get { return _criteria; }
                set { _criteria = value; }
            }

            private Server.DataPortalContext _context;
            public Server.DataPortalContext Context
            {
                get { return _context; }
                set { _context = value; }
            }
        }

        #endregion

        [WebMethod()]
        public byte[] Create(byte[] requestData)
        {
            CreateRequest request = (CreateRequest)Deserialize(requestData);

            DataPortal portal = new DataPortal();
            object result;
            try
            {
                result = portal.Create(request.ObjectType, request.Criteria, request.Context);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return Serialize(result);
        }

        [WebMethod()]
        public byte[] Fetch(byte[] requestData)
        {
            FetchRequest request = (FetchRequest)Deserialize(requestData);
            
            DataPortal portal = new DataPortal();
            object result;
            try
            {
                result = portal.Fetch(request.Criteria, request.Context);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return Serialize(result);
        }

        [WebMethod()]
        public byte[] Update(byte[] requestData)
        {
            UpdateRequest request = (UpdateRequest)Deserialize(requestData);

            DataPortal portal = new DataPortal();
            object result;
            try
            {
                result = portal.Update(request.Object, request.Context);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return Serialize(result);
        }

        [WebMethod()]
        public byte[] Delete(byte[] requestData)
        {
            DeleteRequest request = (DeleteRequest)Deserialize(requestData);

            DataPortal portal = new DataPortal();
            object result;
            try
            {
                result = portal.Delete(request.Criteria, request.Context);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return Serialize(result);
        }

        #region Helper functions

        private static byte[] Serialize(object obj)
        {
            if (obj != null)
            {
                MemoryStream buffer = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, obj);
                return buffer.ToArray();
            }
            return null;
        }

        private static object Deserialize(byte[] obj)
        {
            if (obj != null)
            {
                MemoryStream buffer = new MemoryStream(obj);
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(buffer);
            }
            return null;
        }

        #endregion
    }
}