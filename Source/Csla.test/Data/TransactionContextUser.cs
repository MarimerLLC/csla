//-----------------------------------------------------------------------
// <copyright file="TransactionContextUser.cs" company="Marimer LLC">
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
using Csla.Data;

namespace Csla.Test.Data
{
  [Serializable()]
  public class TransactionContextUser : BusinessBase<TransactionContextUser>
  {
    private static PropertyInfo<string> firstNameProperty = RegisterProperty<string>(new PropertyInfo<string>("FirstName"));

    public string FirstName
    {
      get { return GetProperty(firstNameProperty); }
      set { SetProperty(firstNameProperty, value); }
    }
    private static PropertyInfo<string> lastNameProperty = RegisterProperty<string>(new PropertyInfo<string>("LastName"));
    public string LastName
    {
      get { return GetProperty(lastNameProperty); }
      set { SetProperty(lastNameProperty, value); }
    }

    private static PropertyInfo<string> smallColumnProperty = RegisterProperty<string>(new PropertyInfo<string>("SmallColumn"));
    public string SmallColumn
    {
      get { return GetProperty(smallColumnProperty); }
      set { SetProperty(smallColumnProperty, value); }
    }

    public static TransactionContextUser NewTransactionContextUser()
    {
      return Csla.DataPortal.Create<TransactionContextUser>();
    }

    public static TransactionContextUser GetTransactionContextUser(Csla.Data.SafeDataReader reader)
    {
      return Csla.DataPortal.FetchChild<TransactionContextUser>(reader);
    }


    [RunLocal]
    protected override void DataPortal_Create()
    {

    }

    protected void Child_Fetch(Csla.Data.SafeDataReader reader)
    {
      LoadProperty(firstNameProperty, reader.GetString("FirstName"));
      LoadProperty(lastNameProperty, reader.GetString("LastName"));
      LoadProperty(smallColumnProperty, reader.GetString("SmallColumn"));
    }

    protected void Child_DeleteSelf(TransactionContextUserList parent)
    {
      using (TransactionManager<SqlConnection, SqlTransaction> manager = TransactionManager<SqlConnection, SqlTransaction>.GetManager(nameof(WellKnownValues.DataPortalTestDatabase), true))
      {
        using (SqlCommand command = new SqlCommand("Delete From Table2 Where FirstName = '" + ReadProperty(firstNameProperty) + "' And LastName = '" + ReadProperty(lastNameProperty) + "' And SmallColumn = '" + ReadProperty(smallColumnProperty) + "'", manager.Transaction.Connection, manager.Transaction))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    protected void Child_Insert(TransactionContextUserList parent)
    {
      using (TransactionManager<SqlConnection, SqlTransaction> manager = TransactionManager<SqlConnection, SqlTransaction>.GetManager(nameof(WellKnownValues.DataPortalTestDatabase), true))
      {
        using (SqlCommand command = new SqlCommand("INSERT INTO Table2(FirstName, LastName, SmallColumn) VALUES('" + ReadProperty(firstNameProperty) + "', '" + ReadProperty(lastNameProperty) + "', '" + ReadProperty(smallColumnProperty) + "')", manager.Transaction.Connection, manager.Transaction))
        {
          command.ExecuteNonQuery();
        }
      }
    }

  }
}