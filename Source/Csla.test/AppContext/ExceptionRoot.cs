//-----------------------------------------------------------------------
// <copyright file="ExceptionRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.AppContext
{
  [Serializable()]
  class ExceptionRoot : BusinessBase<ExceptionRoot>
  {
    private string _Data = string.Empty;
    public string Data
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get { return this._Data; }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set { this._Data = value; }
    }

    protected override object GetIdValue()
    {
      return this._Data;
    }

    [Serializable()]
    internal class Criteria
    {
      public const string DefaultData = "<new>";

      private string _Data;

      public string Data
      {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        get { return this._Data; }
      }

      public Criteria()
      {
        this._Data = Criteria.DefaultData;
      }

      public Criteria(string Data)
      {
        this._Data = Data;
      }
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = criteria as Criteria;
      this._Data = crit.Data;
      this.MarkOld();

      TestResults.Add("Root", "Fetched");
      TestResults.Add("create", "create");
      throw new ApplicationException("Fail fetch");
    }

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = criteria as Criteria;
      this._Data = crit.Data;

      TestResults.Add("Root", "Created");
      TestResults.Add("create", "create");
      throw new ApplicationException("Fail create");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      //we would insert here
      TestResults.AddOrOverwrite("Root", "Inserted");
      TestResults.AddOrOverwrite("create", "create");
      throw new ApplicationException("Fail insert");
    }

    [Update]
	protected void DataPortal_Update()
    {
      TestResults.AddOrOverwrite("Root", "Updated");
      TestResults.AddOrOverwrite("create", "create");
      throw new ApplicationException("Fail update");
    }

    [Delete]
	protected void DataPortal_Delete(object criteria)
    {
      TestResults.AddOrOverwrite("Root", "Deleted");
      TestResults.AddOrOverwrite("create", "create");
      throw new ApplicationException("Fail delete");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Add("Root", "Deleted");
      TestResults.Add("create", "create");
      throw new ApplicationException("Fail delete self");
    }
  }
}