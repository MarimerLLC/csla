using System;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Specialized;
using Csla.Properties;

namespace Csla.Server
{
    /// <summary>
    /// Implements the server-side DataPortal 
    /// message router as discussed
    /// in Chapter 5.
    /// </summary>
    public class DataPortal : IDataPortalServer
    {

        #region Data Access

        /// <summary>
        /// Called by the client-side DataPortal to create a new object.
        /// </summary>
        /// <param name="criteria">Object-specific criteria.</param>
        /// <param name="context">Context data from the client.</param>
        /// <returns>A populated business object.</returns>
        public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
        {
            try
            {
                SetContext(context);

                DataPortalResult result;

                MethodInfo method = GetMethod(objectType, "DataPortal_Create");

                // route to Enterprise Services if requested
                if (IsTransactionalMethod(method))
                {
                    ServicedDataPortal portal = new ServicedDataPortal();
                    try
                    {
                        result = portal.Create(objectType, criteria, context);
                    }
                    finally
                    {
                        portal.Dispose();
                    }
                }
                else
                {
                    SimpleDataPortal portal = new SimpleDataPortal();
                    result = portal.Create(objectType, criteria, context);
                }

                ClearContext(context);
                return result;
            }
            catch
            {
                ClearContext(context);
                throw;
            }
        }

        /// <summary>
        /// Called by the client-side DataProtal to retrieve an object.
        /// </summary>
        /// <param name="criteria">Object-specific criteria.</param>
        /// <param name="context">Object containing context data from client.</param>
        /// <returns>A populated business object.</returns>
        public DataPortalResult Fetch(object criteria, DataPortalContext context)
        {
            try
            {
                SetContext(context);

                DataPortalResult result;

                MethodInfo method = GetMethod(GetObjecctType(criteria), "DataPortal_Fetch");

                // route to Enterprise Services if requested
                if (IsTransactionalMethod(method))
                {
                    ServicedDataPortal portal = new ServicedDataPortal();
                    try
                    {
                        result = portal.Fetch(criteria, context);
                    }
                    finally
                    {
                        portal.Dispose();
                    }
                }
                else
                {
                    SimpleDataPortal portal = new SimpleDataPortal();
                    result = portal.Fetch(criteria, context);
                }

                ClearContext(context);
                return result;
            }
            catch
            {
                ClearContext(context);
                throw;
            }
        }

        /// <summary>
        /// Called by the client-side DataPortal to update an object.
        /// </summary>
        /// <param name="obj">A reference to the object being updated.</param>
        /// <param name="context">Context data from the client.</param>
        /// <returns>A reference to the newly updated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public DataPortalResult Update(object obj, DataPortalContext context)
        {
            try
            {
                SetContext(context);

                DataPortalResult result;

                MethodInfo method;
                if (obj is CommandBase)
                    method = GetMethod(obj.GetType(), "DataPortal_Execute");
                else
                    method = GetMethod(obj.GetType(), "DataPortal_Update");

                // route to Enterprise Services if requested
                if (IsTransactionalMethod(method))
                {
                    ServicedDataPortal portal = new ServicedDataPortal();
                    try
                    {
                        result = portal.Update(obj, context);
                    }
                    finally
                    {
                        portal.Dispose();
                    }
                }
                else
                {
                    SimpleDataPortal portal = new SimpleDataPortal();
                    result = portal.Update(obj, context);
                }

                ClearContext(context);
                return result;
            }
            catch
            {
                ClearContext(context);
                throw;
            }
        }

        /// <summary>
        /// Called by the client-side DataPortal to delete an object.
        /// </summary>
        /// <param name="criteria">Object-specific criteria.</param>
        /// <param name="context">Context data from the client.</param>
        public DataPortalResult Delete(object criteria, DataPortalContext context)
        {
            try
            {
                SetContext(context);

                DataPortalResult result;

                MethodInfo method = GetMethod(GetObjecctType(criteria), "DataPortal_Delete");

                // route to Enterprise Services if requested
                if (IsTransactionalMethod(method))
                {
                    ServicedDataPortal portal = new ServicedDataPortal();
                    try
                    {
                        result = portal.Delete(criteria, context);
                    }
                    finally
                    {
                        portal.Dispose();
                    }
                }
                else
                {
                    SimpleDataPortal portal = new SimpleDataPortal();
                    result = portal.Delete(criteria, context);
                }

                ClearContext(context);
                return result;
            }
            catch
            {
                ClearContext(context);
                throw;
            }
        }

        #endregion

        #region Context

        private static void SetContext(DataPortalContext context)
        {
            // if the dataportal is not remote then
            // do nothing
            if (!context.IsRemotePortal) return;

            // set the app context to the value we gor from the
            // client
            ApplicationContext.SetContext(context.ClientContext, context.GlobalContext);

            // set the context value so everyone knows the
            // code is running on the server
            ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);

            if (ApplicationContext.AuthenticationType == "Windows")
            {
                // When using integrated security, Principal must be null
                if (context.Principal == null)
                {
                    // Set .NET to use integrated security
                    AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                    return;
                }
                else
                {
                    throw new System.Security.SecurityException(Resources.NoPrincipalAllowedException);
                }
            }
            // We expect the Principal to be of the type BusinesPrincipal
            if (context.Principal != null)
            {
                if (context.Principal is Security.BusinessPrincipalBase)
                {
                    // See if our current principal is different from the caller's principal
                    if (!ReferenceEquals(context.Principal, System.Threading.Thread.CurrentPrincipal))
                    {
                        // The caller had a different principal, so change ours to match the
                        // caller's, so all our objects use the caller's security.
                        System.Threading.Thread.CurrentPrincipal = context.Principal;
                    }
                }
                else
                    throw new System.Security.SecurityException(Resources.BusinessPrincipalException + " " + ((object)context.Principal).ToString());
            }
            else
                throw new System.Security.SecurityException(Resources.BusinessPrincipalException + " Nothing");
        }

        private static void ClearContext(DataPortalContext context)
        {
            // if the dataportal is not remote then
            // do nothing
            if (!context.IsRemotePortal) return;
            ApplicationContext.Clear();
        }

        #endregion

        #region Helper methods

        private static bool IsTransactionalMethod(MethodInfo method)
        {
            return Attribute.IsDefined(method, typeof(TransactionalAttribute));
        }

        private static Type GetObjecctType(object criteria)
        {
            if (criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
            {
                // get the type of the actual business object
                // from CriteriaBase (using the new scheme)
                return ((CriteriaBase)criteria).ObjectType;
            }
            else
            {
                // get the type of the actual business object
                // based on the nested class scheme in the book
                return criteria.GetType().DeclaringType;
            }
        }

        private static MethodInfo GetMethod(Type objectType, string method)
        {
            return objectType.GetMethod(method,
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);
        }

        #endregion

    }
}