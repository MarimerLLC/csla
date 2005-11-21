using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Csla.Test.DataPortal
{
    [Serializable()]
    public class TransactionalRoot : BusinessBase<TransactionalRoot>
    {
        #region "Business methods"

        private int _ID;
        private string _firstName;
        private string _lastName;
        private string _smallColumn;

        public int ID
        {
            get { return _ID; }
        }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                PropertyHasChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                PropertyHasChanged("LastName");
            }
        }

        public string SmallColumn
        {
            get { return _smallColumn; }
            set
            {
                _smallColumn = value;
                PropertyHasChanged("SmallColumn");
            }
        }

        #endregion

        #region "Object ID value"

        protected override object GetIdValue()
        {
            return _ID;
        }

        #endregion

        protected override void AddBusinessRules()
        {
            //normally, we would add a rule that prevents SmallColumn from being too long
            //but to easily test the transactional functionality of the server-side dataportal
            //we are going to allow strings that are longer than what the database allows
        }

        #region "constructors"

        private TransactionalRoot()
        { 
            //require factory method 
        }

        #endregion

        #region "Factory Methods"

        public static TransactionalRoot NewTransactionalRoot()
        {
            return Csla.DataPortal.Create<TransactionalRoot>();
        }

        public static TransactionalRoot GetTransactionalRoot(int ID)
        {
            return Csla.DataPortal.Fetch<TransactionalRoot>(new Criteria(ID));
        }

        public static void DeleteTransactionalRoot(int ID)
        {
            Csla.DataPortal.Delete(new Criteria(ID));
        }

        #endregion

        public override TransactionalRoot Save()
        {
            return base.Save();
        }

        #region "Criteria"

        [Serializable()]
        private class Criteria
        {
            public int _id;

            public Criteria(int id)
            {
                this._id = id;
            }
        }

        #endregion

        #region "Data Access"

        [RunLocal()]
        protected override void DataPortal_Create(object criteria)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Created");
            ValidationRules.CheckRules();
            Console.WriteLine("DataPortal_Create");
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Console.WriteLine("DataPortal_Fetch");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Fetched");
            ValidationRules.CheckRules();
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Inserted");
            Console.WriteLine("DataPortal_Insert");
        }

        protected override void DataPortal_Update()
        {
            Console.WriteLine("DataPortal_Update");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Updated");
        }

        protected override void DataPortal_DeleteSelf()
        {
            Console.WriteLine("DataPortal_DeleteSelf");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Deleted Self");
        }

        [Transactional(TransactionalTypes.EnterpriseServices)]
        protected override void DataPortal_Delete(object criteria)
        {
            Console.WriteLine("DataPortal_Delete");
            System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|DataPortalTestDatabase.mdf;Integrated Security=True;User Instance=True");

            System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand("INSERT INTO Table2(FirstName, LastName, SmallColumn) VALUES('Bill', 'Johnson', 's')", cn);
            System.Data.SqlClient.SqlCommand cm2 = new System.Data.SqlClient.SqlCommand("INSERT INTO Table2(Firstname, LastName, Smallcolumn) VALUES('jimmy', 'thompson', 'this will cause an exception')", cn);
            cn.Open();
            cm.ExecuteNonQuery();
            cm2.ExecuteNonQuery();
            cn.Close();

            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Deleted");
        }

        #endregion
    }
}