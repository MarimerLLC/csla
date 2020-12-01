//-----------------------------------------------------------------------
// <copyright file="IDataPortalExceptionInspector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement this interface to check a DataPortalException before returning Exception to the client. </summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server
{
    /// <summary>
    /// Implement this interface to check a DataPortalException before returning Exception to the client. 
    /// 
    /// Make sure to rethrow a new exception if you want to transfrom to new exception. 
    /// </summary>
    public interface IDataPortalExceptionInspector
    {
        /// <summary>
        /// Inspects the exception that occurred during DataPortal call
        /// If you want to transform to/return another Exception to the client
        /// you must throw the new Exception in this method.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="businessObject">The business object , if available.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="ex">The exception.</param>
        void InspectException(Type objectType, object businessObject, object criteria, string methodName, Exception ex);
    }
}