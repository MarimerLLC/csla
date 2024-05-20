//-----------------------------------------------------------------------
// <copyright file="InvalidQueryExpression.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Properties;

namespace Csla.Core
{
  class InvalidQueryException : Exception
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