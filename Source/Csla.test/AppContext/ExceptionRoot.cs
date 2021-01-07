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
    private class Criteria
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
    public static ExceptionRoot NewExceptionRoot()
    {
      Criteria c = new Criteria();
      object result = Csla.DataPortal.Create<ExceptionRoot>(c);
      return result as ExceptionRoot;
    }

    public static ExceptionRoot GetExceptionRoot(string Data)
    {
      return Csla.DataPortal.Fetch<ExceptionRoot>(new Criteria(Data)) as ExceptionRoot;
    }

    public static void DeleteExceptionRoot(string Data)
    {
      Csla.DataPortal.Delete<ExceptionRoot>(new Criteria(Data));
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = criteria as Criteria;
      this._Data = crit.Data;
      this.MarkOld();

      Csla.ApplicationContext.GlobalContext.Add("Root", "Fetched");
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail fetch");
    }
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = criteria as Criteria;
      this._Data = crit.Data;

      Csla.ApplicationContext.GlobalContext.Add("Root", "Created");
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail create");
    }
    protected override void DataPortal_Insert()
    {
      //we would insert here
      Csla.ApplicationContext.GlobalContext["Root"] = "Inserted";
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail insert");
    }
    protected override void DataPortal_Update()
    {
      Csla.ApplicationContext.GlobalContext["Root"] = "Updated";
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail update");
    }
    protected void DataPortal_Delete(object criteria)
    {
      Csla.ApplicationContext.GlobalContext["Root"] = "Deleted";
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail delete");
    }
    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext["Root"] = "Deleted";
      Csla.ApplicationContext.GlobalContext["create"] = "create";
      throw new ApplicationException("Fail delete self");
    }



  }
}