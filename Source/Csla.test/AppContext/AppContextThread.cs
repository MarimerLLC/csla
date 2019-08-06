//-----------------------------------------------------------------------
// <copyright file="AppContextThread.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Csla;

namespace Csla.Test.AppContext
{
  public class AppContextThread
  {
    //public static bool StaticRemoved = false;

    private string _Name = string.Empty;
    private bool _run = true;

    public bool Removed
    {
      get
      {
        lock (this)
        {
          if (Csla.ApplicationContext.ClientContext[this._Name] == null ||
              Csla.ApplicationContext.GlobalContext[this._Name] == null)
          {
            return true;
          }
          return false;
        }
      }
    }

    public AppContextThread(string Name)
    {
      this._Name = Name;
    }

    public void Stop()
    {
      _run = false;
    }

    public void Run()
    {
      lock (this)
      {
        Csla.ApplicationContext.GlobalContext.Add(this._Name, this._Name);
        Csla.ApplicationContext.ClientContext.Add(this._Name, this._Name);
      }
      while (_run)
      {
        //if (this.Removed) AppContextThread.StaticRemoved = true;
        Thread.Sleep(10);
      }
    }
  }
}