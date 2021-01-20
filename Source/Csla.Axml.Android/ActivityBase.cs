﻿//-----------------------------------------------------------------------
// <copyright file="ActivityBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>CSLA helper to be used in place of the normal Xamarin Android activity that contains a reference to a CSLA ViewModel and a BindingManager</summary>
//-----------------------------------------------------------------------

using Android.App;
using Android.OS;
using Csla.Axml.Binding;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace Csla.Axml
{
    /// <summary>
    /// CSLA helper to be used in place of the normal Xamarin Android activity that contains a reference to a CSLA ViewModel and a BindingManager.
    /// </summary>
    /// <typeparam name="T">A type that inherits from the ViewModel.</typeparam>
    /// <typeparam name="Z">The type that is defined as used by T.</typeparam>
    public abstract class ActivityBase<T, Z> : Activity where T : ViewModel<Z>
    {
        /// <summary>
        /// A reference to the view model.
        /// </summary>
        protected T viewModel = null;

        /// <summary>
        /// The BindingManager for this Activity.
        /// </summary>
        protected BindingManager Bindings = null;

        /// <summary>
        /// Event that occurs when the activity is created.
        /// </summary>
        /// <param name="bundle">Information about the activities creation.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Bindings = new BindingManager(this);
        }

        /// <summary>
        /// Serializes the ViewModel's model property using the SerializationFormatterFactory.GetFormatter().  Used to serialize and transfer models between activities.
        /// </summary>
        /// <returns>An array of bytes that contains the serialized model.</returns>
        protected byte[] SerilizeModelForParameter()
        {
            return this.SerilizeModelForParameter(this.viewModel.Model);
        }

        /// <summary>
        /// Serialized the provided model using the SerializationFormatterFactory.GetFormatter().  Used to serialize and transfer models between activities.
        /// </summary>
        /// <param name="model">A reference to the model to serialize.</param>
        /// <returns>An array of bytes that contains the serialized model.</returns>
        protected byte[] SerilizeModelForParameter(object model)
        {
            return SerializationFormatterFactory.GetFormatter().Serialize(model);
        }

        /// <summary>
        /// Takes a byte array and uses the MobileFormatter to reconstitute it into a model reference.
        /// </summary>
        /// <param name="parameter">An array of bytes to deserialized.</param>
        /// <returns>A reference to the deserialized object.  An exception will be thrown if the supplied byte array is not valid for deserializaiton.</returns>
        protected object DeserializeFromParameter(byte[] parameter)
        {
            return SerializationFormatterFactory.GetFormatter().Deserialize(parameter);
        }
    }
}