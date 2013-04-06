using System;
using Csla.Properties;

namespace Csla.Core
{
  class InvalidQueryException : System.Exception
  {
    private string message;

    public InvalidQueryException(string message)
    {
      this.message = message + " ";
    }

    public override string Message
    {
      get
      {
        return Resources.ClientQueryIsInvalid + message;
      }
    }
  }
}
