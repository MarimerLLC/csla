#if !NETFX_CORE && !(ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="GenericBusinessException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned as BusinessException in DataPortalException when the </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Csla.Server
{
    /// <summary>
    /// This exception is returned as BusinessException in DataPortalException when the 
    /// serverside/inner exception is not serializable 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable()]
    public class GenericBusinessException : Exception
    {

        private string _stackTrace;
        private IDictionary _data;
        private string _type;

        /// <summary>
        /// Gets a string representation of the immediate frames on the call stack.
        /// </summary>
        /// <value></value>
        /// <returns>A string that describes the immediate frames of the call stack.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string StackTrace
        {
            get
            {
                return _stackTrace;
            }
        }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional user-defined information about the exception.
        /// </summary>
        /// <value></value>
        /// <returns>An object that implements the <see cref="T:System.Collections.IDictionary"/> interface and contains a collection of user-defined key/value pairs. The default is an empty collection.</returns>
        public override IDictionary Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericBusinessException"/> class.
        /// Reads information for a NonSerializable excpeption into GenericBusinessException
        /// </summary>
        /// <param name="wrappedException">The wrapped exception.</param>
        public GenericBusinessException(Exception wrappedException)
            : base(wrappedException.Message)
        {
            this.Source = wrappedException.Source;
            this.HelpLink = wrappedException.HelpLink;
            _data = wrappedException.Data;
            _stackTrace = wrappedException.StackTrace;
            _type = wrappedException.GetType().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericBusinessException"/> class.
        /// </summary>
        /// <param name="wrappedException">The wrapped exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public GenericBusinessException(Exception wrappedException, Exception innerException)
            : base(wrappedException.Message, innerException)
        {
            this.Source = wrappedException.Source;
            this.HelpLink = wrappedException.HelpLink;
            _data = wrappedException.Data;
            _stackTrace = wrappedException.StackTrace;
            _type = wrappedException.GetType().ToString();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GenericBusinessException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public GenericBusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            _data = (IDictionary)info.GetValue("_data", typeof(IDictionary));
            _stackTrace = info.GetString("_stackTrace");
            _type = info.GetString("_type");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
#if !NET5_0
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("_data", _data);
            info.AddValue("_stackTrace", _stackTrace);
            info.AddValue("_type", _type);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string ToString()
        {
            string className;
            string message = this.Message;
            if ((message == null) || (message.Length <= 0))
            {
                className = _type;
            }
            else
            {
                className = _type + ": " + message;
            }
            string stackTrace = this.StackTrace;
            if (stackTrace != null)
            {
                className = className + Environment.NewLine + stackTrace;
            }
            return className;
        }
    }
}
#endif