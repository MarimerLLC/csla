using System;
using Csla.Properties;

namespace Csla
{

    /// <summary>
    /// This is the base class from which most business objects
    /// will be derived.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is the core of the CSLA .NET framework. To create
    /// a business object, inherit from this class.
    /// </para><para>
    /// Please refer to 'Expert One-on-One VB.NET Business Objects' for
    /// full details on the use of this base class to create business
    /// objects.
    /// </para>
    /// </remarks>
    [Serializable()]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
    public abstract class BusinessBase : Csla.Core.BusinessBase
    {
        #region Constructors

        protected BusinessBase()
        {

        }

        #endregion

        #region Data Access

        /// <summary>
        /// Saves the object to the database.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calling this method starts the save operation, causing the object
        /// to be inserted, updated or deleted within the database based on the
        /// object's current state.
        /// </para><para>
        /// If <see cref="Csla.BusinessBase.IsDeleted" /> is True the object
        /// will be deleted. Otherwise, if <see cref="Csla.BusinessBase.IsNew" /> 
        /// is True the object will be inserted. Otherwise the object's data will 
        /// be updated in the database.
        /// </para><para>
        /// All this is contingent on <see cref="Csla.BusinessBase.IsDirty" />. If
        /// this value is False, no data operation occurs. It is also contingent on
        /// <see cref="Csla.BusinessBase.IsValid" />. If this value is False an
        /// exception will be thrown to indicate that the UI attempted to save an
        /// invalid object.
        /// </para><para>
        /// It is important to note that this method returns a new version of the
        /// business object that contains any data updated during the save operation.
        /// You MUST update all object references to use this new version of the
        /// business object in order to have access to the correct object data.
        /// </para><para>
        /// You can override this method to add your own custom behaviors to the save
        /// operation. For instance, you may add some security checks to make sure
        /// the user can save the object. If all security checks pass, you would then
        /// invoke the base Save method via <c>MyBase.Save()</c>.
        /// </para>
        /// </remarks>
        /// <returns>A new object containing the saved values.</returns>
        public virtual BusinessBase Save()
        {
            if (this.IsChild)
                throw new NotSupportedException(Resources.NoSaveChildException);
            if (EditLevel > 0)
                throw new Validation.ValidationException(Resources.NoSaveEditingException);
            if (!IsValid)
                throw new Validation.ValidationException(Resources.NoSaveInvalidException);
            if (IsDirty)
                return (BusinessBase)DataPortal.Update(this);
            else
                return this;
        }

        /// <summary>
        /// Saves the object to the database, forcing
        /// IsNew to False and IsDirty to True.
        /// </summary>
        /// <param name="forceUpdate">
        /// If True, triggers overriding IsNew and IsDirty. 
        /// If False then it is the same as calling Save().
        /// </param>
        /// <returns>A new object containing the saved values.</returns>
        /// <remarks>
        /// This overload is designed for use in web applications
        /// when implementing the Update method in your 
        /// data wrapper object.
        /// </remarks>
        public BusinessBase Save(bool forceUpdate)
        {
            if (forceUpdate && IsNew)
            {
                // mark the object as old - which makes it
                // not dirty
                MarkOld();
                // now mark the object as dirty so it can save
                MarkDirty();
            }
            return this.Save();
        }

        #endregion

    }
}