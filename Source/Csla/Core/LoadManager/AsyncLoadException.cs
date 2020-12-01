using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Core.LoadManager
{
    /// <summary>
    /// Exception class to add the PropertyInfo and better nmessage to an async exception
    /// </summary>
    [Serializable]
    public class AsyncLoadException : Exception
    {

      /// <summary>
      /// The property that Async LazyLoad failed on.  
      /// </summary>
      /// <value>
      /// The property.
      /// </value>
      public IPropertyInfo Property { get; set; }

        /// <summary>
        /// Constructor for AsyncLoadException
        /// </summary>
        /// <param name="property">the IPropertyInfo that desccribes the property</param>
        /// <param name="message">Clear text message for user</param>
        /// <param name="ex">the actual exception</param>
        public AsyncLoadException(IPropertyInfo property, string message, Exception ex) : base(message, ex)
        {
            Property = property;
        }
    }
}
