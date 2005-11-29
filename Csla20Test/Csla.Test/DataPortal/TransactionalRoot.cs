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
        //get the configurationmanager to work right
        public const string CONNECTION_STRING = 
            "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|DataPortalTestDatabase.mdf;Integrated Security=True;User Instance=True";

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
            Criteria crit = (Criteria)(criteria);

            if (crit._id == 13)
            {
                throw new System.ApplicationException("DataPortal_Fetch: you chose an unlucky number");
            }

            Console.WriteLine("DataPortal_Fetch");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Fetched");
            ValidationRules.CheckRules();
        }

        protected override void DataPortal_Insert()
        {
            SqlConnection cn = new SqlConnection(CONNECTION_STRING);
            string firstName = this._firstName;
            string lastName = this._lastName;
            string smallColumn = this._smallColumn;

            //this command will always execute successfully
            //since it inserts a string less than 5 characters
            //into SmallColumn
            SqlCommand cm1 = new SqlCommand();
            cm1.Connection = cn;
            cm1.CommandText = "INSERT INTO Table2(FirstName, LastName, SmallColumn) VALUES('Bill', 'Thompson', 'abc')";

            //this command will throw an exception
            //if SmallColumn is set to a string longer than 
            //5 characters
            SqlCommand cm2 = new SqlCommand();
            cm2.Connection = cn;
            //use stringbuilder
            cm2.CommandText = "INSERT INTO Table2(FirstName, LastName, SmallColumn) VALUES('";
            cm2.CommandText += firstName;
            cm2.CommandText += "', '" + lastName + "', '" + smallColumn + "')";

            cn.Open();
            cm1.ExecuteNonQuery();
            cm2.ExecuteNonQuery();
            cn.Close();

            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Inserted");
            Console.WriteLine("DataPortal_Insert");
        }

        [Transactional(TransactionalTypes.TransactionScope)]
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

        protected override void DataPortal_Delete(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            if (crit._id == 13)
            {
                throw new System.ApplicationException("DataPortal_Delete: you chose an unlucky number");
            }

            Console.WriteLine("DataPortal_Delete");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionRoot", "Deleted");
        }

        #endregion
    }
}