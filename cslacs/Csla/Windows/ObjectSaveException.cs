using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Windows
{
  public class ObjectSaveException : Exception 
  {
    public ObjectSaveException() : base() { }

    public ObjectSaveException(string message) : base(message) { }

    public ObjectSaveException(string message, Exception innerException) : base(message, innerException) { }

    public ObjectSaveException(Exception innerException) : base(Resources.ExceptionOccurredDuringSaveOperation, innerException) { }
  }
}
