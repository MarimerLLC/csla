//-----------------------------------------------------------------------
// <copyright file="AddressList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class AddressList : BusinessListBase<AddressList, Address>
  {
  }
}