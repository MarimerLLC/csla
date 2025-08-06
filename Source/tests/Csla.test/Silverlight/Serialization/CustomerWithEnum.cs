//-----------------------------------------------------------------------
// <copyright file="CustomerWithEnum.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a test business object with an enum.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace cslalighttest.Serialization
{
  public enum CustomerQuality
  {
    None = 0,
    Good = 1,
    Bad = 2
  }

  /// <summary>
  /// Implementation of a test business object with an enum.
  /// </summary>
  [Serializable()]
  public sealed class CustomerWithEnum : BusinessBase<CustomerWithEnum>
  {
    #region Constructors

    public CustomerWithEnum()
    {
      // Nothing
    }

    #endregion

    #region Properties

    private static PropertyInfo<string> Property_Name = RegisterProperty(new PropertyInfo<string>("Name", "Name"));

    /// <summary>
    /// Gets or sets the name of the CustomerWithEnum.
    /// </summary>
    public string Name
    {
      get { return GetProperty(Property_Name); }
      set { SetProperty(Property_Name, value); }
    }

    private static PropertyInfo<CustomerQuality> Property_Quality = RegisterProperty<CustomerQuality>(c => c.Quality, "Quality");

    /// <summary>
    /// Gets or sets the quality of the customer.
    /// </summary>
    public CustomerQuality Quality
    {
      get { return GetProperty(Property_Quality); }
      set { SetProperty(Property_Quality, value); }
    }

    #endregion
  }
}