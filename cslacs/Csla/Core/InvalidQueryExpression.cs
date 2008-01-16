using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        return "The client query is invalid: " + message;
      }
    }
  }
}
