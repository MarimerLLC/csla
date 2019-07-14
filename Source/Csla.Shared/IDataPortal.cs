//-----------------------------------------------------------------------
// <copyright file="IDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining the members of the data portal type</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla
{
    /// <summary>
    /// Interface defining the members of the data portal type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataPortal<T>
  {
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<T> CreateAsync(params object[] criteria);
    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    Task<T> FetchAsync(params object[] criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    Task<T> UpdateAsync(T obj);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    Task<T> ExecuteAsync(T command);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    Task DeleteAsync(params object[] criteria);
    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    T Create(params object[] criteria);
    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    T Fetch(params object[] criteria);
    /// <summary>
    /// Called to execute a Command object on the server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To be a Command object, the object must inherit from
    /// CommandBase.
    /// </para><para>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </para><para>
    /// On the server, the Command object's DataPortal_Execute() method will
    /// be invoked and on an ObjectFactory the Execute method will be invoked. 
    /// Write any server-side code in that method. 
    /// </para>
    /// </remarks>
    /// <param name="obj">A reference to the Command object to be executed.</param>
    /// <returns>A reference to the updated Command object.</returns>
    T Execute(T obj);
    /// <summary>
    /// Called by the business object's Save() method to
    /// insert, update or delete an object in the database.
    /// </summary>
    /// <remarks>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </remarks>
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <returns>A reference to the updated business object.</returns>
    T Update(T obj);
    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    void Delete(params object[] criteria);
    /// <summary>
    /// Gets a reference to the global context returned from
    /// the background thread and/or server.
    /// </summary>
    Csla.Core.ContextDictionary GlobalContext { get; }
    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    [Obsolete]
    event EventHandler<DataPortalResult<T>> CreateCompleted;
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    [Obsolete]
    void BeginCreate();
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [Obsolete]
    void BeginCreate(object criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    void BeginCreate(object criteria, object userState);
    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    [Obsolete]
    event EventHandler<DataPortalResult<T>> FetchCompleted;
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    [Obsolete]
    void BeginFetch();
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [Obsolete]
    void BeginFetch(object criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to retrieve an existing object, which is loaded 
    /// with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    void BeginFetch(object criteria, object userState);
    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    [Obsolete]
    event EventHandler<DataPortalResult<T>> UpdateCompleted;
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    [Obsolete]
    void BeginUpdate(T obj);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    void BeginUpdate(T obj, object userState);
    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    [Obsolete]
    event EventHandler<DataPortalResult<T>> DeleteCompleted;
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [Obsolete]
    void BeginDelete(object criteria);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    void BeginDelete(object criteria, object userState);
    /// <summary>
    /// Event indicating an execute operation is complete.
    /// </summary>
    [Obsolete]
    event EventHandler<DataPortalResult<T>> ExecuteCompleted;
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    [Obsolete]
    void BeginExecute(T command);
    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    /// <param name="userState">User state data.</param>
    [Obsolete]
    void BeginExecute(T command, object userState);
  }
}
