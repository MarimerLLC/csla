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
    private string _message;

    public InvalidQueryException(string message)
    {
      this._message = message + " ";
    }

    public override string Message => Resources.ClientQueryIsInvalid + _message;
  }
}