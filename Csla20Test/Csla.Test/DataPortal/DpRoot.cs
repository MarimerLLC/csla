using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortal
{
    [Serializable()]
    public class DpRoot : BusinessBase<DpRoot>
    {
        private string _data;
        private string _auth = "No value";

        protected override object GetIdValue()
        {
            return _data;
        }

        #region "Get/Set Private Variables" 

        public string Data
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                if (CanReadProperty())
                {
                    return _data;
                }
                else
                {
                    throw new System.Security.SecurityException("Property get not allowed");
                }
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                if (CanWriteProperty())
                {
                    if (_data != value)
                    {
                        _data = value;
                        PropertyHasChanged();
                    }
                    else
                    {
                        throw new System.Security.SecurityException("Property set not allowed");
                    }
                }
            }
        }

        public string Auth
        {
            get
            {
                return _auth;
            }

            set
            {
                //Not allowed
            }
        }

        #endregion 

        #region "Criteria class"

        [Serializable()]
        private class Criteria
        {
            public string _data;

            public Criteria()
            {
                _data = "<new>";
            }

            public Criteria(string data)
            {
                this._data = data;
            }
        }

        #endregion

        #region "New root + constructor"

        public static DpRoot NewRoot()
        {
            Criteria crit = new Criteria();
            return (Csla.DataPortal.Create<DpRoot>(crit));
        }

        public static DpRoot GetRoot(string data)
        {
            return (Csla.DataPortal.Fetch<DpRoot>(new Criteria(data)));
        }

        private DpRoot()
        {
            //prevent direct creation
            AddAuthRules();
        }

        #endregion 

        public DpRoot CloneThis()
        {
            return this.Clone();
        }


        #region "DataPortal"

        protected override void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
        }

        protected override void DataPortal_Insert()
        {
            //we would insert here
        }

        protected override void DataPortal_Update()
        {
            //we would update here
        }

        protected override void DataPortal_DeleteSelf()
        {
            //we would delete here
        }

        protected override void DataPortal_Delete(object criteria)
        {
            //we would delete here
        }

        protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["serverinvoke"] = true;
        }

        protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["serverinvokecomplete"] = true;
        }

        #endregion

        #region "Authorization Rules"

        private void AddAuthRules()
        {
            string role = "Admin";

            this.AuthorizationRules.DenyRead("DenyReadOnProperty", role);
            this.AuthorizationRules.DenyWrite("DenyWriteOnProperty", role);

            this.AuthorizationRules.DenyRead("DenyReadWriteOnProperty", role);
            this.AuthorizationRules.DenyWrite("DenyReadWriteOnProperty", role);

            this.AuthorizationRules.AllowRead("AllowReadWriteOnProperty", role);
            this.AuthorizationRules.AllowWrite("AllowReadWriteOnProperty", role);
        }

        public string DenyReadOnProperty
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                if (CanReadProperty())
                    throw new System.Security.SecurityException("Not allowed 1");
                    
                else
                    return "[DenyReadOnProperty] Can't read property";
            }

            set
            {
                //Not allowed
            }
        }

        public string DenyWriteOnProperty
        {
            get
            {
                return "<No Value>";
            }

            set
            {
                if (CanWriteProperty())
                    throw new System.Security.SecurityException("Not allowed 2");

                else
                    _auth = "[DenyWriteOnProperty] Can't write variable";

            }
        }

        public string DenyReadWriteOnProperty
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                if (CanReadProperty())
                    throw new System.Security.SecurityException("Not allowed 3");

                else
                    return "[DenyReadWriteOnProperty] Can't read property";
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                if (CanWriteProperty())
                    throw new System.Security.SecurityException("Not allowed 4");

                else
                    _auth = "[DenyReadWriteOnProperty] Can't write variable";
            }
        }

        public string AllowReadWriteOnProperty
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                if (CanReadProperty())
                    return _auth;

                else
                    throw new System.Security.SecurityException("Should be allowed 5");
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                if (CanWriteProperty())
                    _auth = value;

                else
                    throw new System.Security.SecurityException("Should be allowed 5");
            }
        }

        #endregion
    }
}
