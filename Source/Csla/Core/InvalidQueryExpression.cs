//-----------------------------------------------------------------------
// <copyright file="InvalidQueryExpression.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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