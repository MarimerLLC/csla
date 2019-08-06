//-----------------------------------------------------------------------
// <copyright file="TransactionContextUserList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Data.SqlClient;
using Csla.Data;

namespace Csla.Test.Data
{
  [Serializable]
  public class TransactionContextUserList : BusinessBindingListBase<TransactionContextUserList, TransactionContextUser>
  {
    public const string TestDBConnection = nameof(WellKnownValues.DataPortalTestDatabase);

    protected override object AddNewCore()
    {
      var newUser = TransactionContextUser.NewTransactionContextUser();
      Add(newUser);
      return newUser;
    }

    public static TransactionContextUserList GetList()
    {
      return Csla.DataPortal.Fetch<TransactionContextUserList>();
    }

    protected void DataPortal_Fetch()
    {
      using (var manager = ConnectionManager<SqlConnection>.GetManager(TestDBConnection, true))
      {
        using (var command = new SqlCommand("Select * From Table2", manager.Connection))
        {
          using (var reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
          {
            while (reader.Read())
              Add(TransactionContextUser.GetTransactionContextUser(reader));
          }
        }
      }
    }

    protected override void DataPortal_Update()
    {
      using (var manager = TransactionManager<SqlConnection, SqlTransaction>.GetManager(TestDBConnection, true))
      {
        foreach (var oneItem in DeletedList)
          Csla.DataPortal.UpdateChild(oneItem, this);

        foreach (var oneItem in this)
          Csla.DataPortal.UpdateChild(oneItem, this);

        DeletedList.Clear();

        manager.Commit();
      }
    }
  }
}