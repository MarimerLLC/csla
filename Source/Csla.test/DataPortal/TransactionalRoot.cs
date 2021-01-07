//-----------------------------------------------------------------------
// <copyright file="TransactionalRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Csla.Test.DataPortal
{
    [Serializable()]
    public class TransactionalRoot : BusinessBase<TransactionalRoot>
    {
        #region "Business methods"

        //get the configurationmanager to work right
        //public static string CONNECTION_STRING = WellKnownValues.ConnectionStrings.DataPortalTestDatabase;

        public static PropertyInfo<int> IDProperty = RegisterProperty<int>(c => c.ID);
        public int ID
        {
          get { return GetProperty(IDProperty); }
          private set { LoadProperty(IDProperty, value); }
        }

        public static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName);
        public string FirstName
        {
          get { return GetProperty(FirstNameProperty); }
          set { SetProperty(FirstNameProperty, value); }
        }

        public static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);
        public string LastName
        {
          get { return GetProperty(LastNameProperty); }
          set { SetProperty(LastNameProperty, value); }
        }

        public static PropertyInfo<string> SmallColumnProperty = RegisterProperty<string>(c => c.SmallColumn);
        public string SmallColumn
        {
          get { return GetProperty(SmallColumnProperty); }
          set { SetProperty(SmallColumnProperty, value); }
        }

        #endregion

        protected override void AddBusinessRules()
        {
            //normally, we would add a rule that prevents SmallColumn from being too long
            //but to easily test the transactional functionality of the server-side dataportal
            //we are going to allow strings that are longer than what the database allows
        }

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
          Csla.DataPortal.Delete<TransactionalRoot>(new Criteria(ID));
        }

        #endregion

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
        protected override void DataPortal_Create()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Created");
            BusinessRules.CheckRules();
            Console.WriteLine("DataPortal_Create");
        }

        protected void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);

            if (crit._id == 13)
            {
                throw new System.ApplicationException("DataPortal_Fetch: you chose an unlucky number");
            }

            Console.WriteLine("DataPortal_Fetch");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("TransactionalRoot", "Fetched");
            BusinessRules.CheckRules();
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            SqlConnection cn = new SqlConnection(WellKnownValues.DataPortalTestDatabase);
            string firstName = this.FirstName;
            string lastName = this.LastName;
            string smallColumn = this.SmallColumn;

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

        protected void DataPortal_Delete(object criteria)
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

        #region "DataPortalException"

        protected override void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("OnDataPortalException", "Called");
            Console.WriteLine("OnDataPortalException called");
        }

        #endregion
    }
}