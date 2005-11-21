using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Csla;

namespace Csla.Test.AppContext
{
    public class AppContextThread
    {
        public static bool StaticRemoved = false;

        private string _Name = string.Empty;

        public bool Removed
        {
            get
            {
                if (Csla.ApplicationContext.ClientContext[this._Name] == null ||
                    Csla.ApplicationContext.GlobalContext[this._Name] == null)
                {
                    return true;
                }
                return false;
            }
        }
        public AppContextThread(string Name)
        {
            this._Name = Name;
        }
        public void Run()
        {
            Csla.ApplicationContext.GlobalContext.Add(this._Name, this._Name);
            Csla.ApplicationContext.ClientContext.Add(this._Name, this._Name);
            while (true)
            {
                if (this.Removed) AppContextThread.StaticRemoved = true;
                Thread.Sleep(10);
            }
        }
    }
}
