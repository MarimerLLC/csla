//-----------------------------------------------------------------------
// <copyright file="IUseBusinessRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
using Csla.Rules;

namespace Csla.Core
{
  internal interface IUseBusinessRules
  {
    BusinessRules BusinessRules { get; }
  }
}
