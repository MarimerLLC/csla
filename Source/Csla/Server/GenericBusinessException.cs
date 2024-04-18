#if !NETFX_CORE && !(ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="GenericBusinessException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This exception is returned as BusinessException in DataPortalException when the </summary>
//-----------------------------------------------------------------------
using System.Collections;

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
      /// <summary>
        /// Gets a string representation of the immediate frames on the call stack.
        /// </summary>
        /// <value></value>
        /// <returns>A string that describes the immediate frames of the call stack.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string StackTrace { get; }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional user-defined information about the exception.
        /// </summary>
        /// <value></value>
        /// <returns>An object that implements the <see cref="T:System.Collections.IDictionary"/> interface and contains a collection of user-defined key/value pairs. The default is an empty collection.</returns>
        public override IDictionary Data { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; }

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
            Data = wrappedException.Data;
            StackTrace = wrappedException.StackTrace;
            TypeName = wrappedException.GetType().ToString();
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
            Data = wrappedException.Data;
            StackTrace = wrappedException.StackTrace;
            TypeName = wrappedException.GetType().ToString();
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
                className = TypeName;
            }
            else
            {
                className = TypeName + ": " + message;
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