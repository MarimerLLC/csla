//-----------------------------------------------------------------------
// <copyright file="RootWithValidation.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Csla.Rules;

namespace Csla.Web.Mvc.Test.ModelBinderTest
{
  [Serializable()]
  public class RootWithValidation : Csla.BusinessBase<RootWithValidation>
  {
    public static readonly PropertyInfo<string> Max5CharsProperty = RegisterProperty<string>(p => p.Max5Chars);
    public string Max5Chars
    {
      get { return GetProperty<string>(Max5CharsProperty); }
      set { SetProperty<string>(Max5CharsProperty, value); }
    }

    public static readonly PropertyInfo<int> Between2And10Property = RegisterProperty<int>(p => p.Between2And10);
    [Range(2, 10)]
    public int Between2And10
    {
      get { return GetProperty<int>(Between2And10Property); }
      set { SetProperty<int>(Between2And10Property, value); }
    }

    public static readonly PropertyInfo<string> AlwaysInvalidProperty = RegisterProperty<string>(p => p.AlwaysInvalid);
    [ImAlwaysInvalid("Don't try or invalid!")]
    public string AlwaysInvalid
    {
      get { return GetProperty<string>(AlwaysInvalidProperty); }
      set { SetProperty<string>(AlwaysInvalidProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(Max5CharsProperty, 5));

      base.AddBusinessRules();
    }

    public static RootWithValidation NewRootWithValidation()
    {
      return DataPortal.Create<RootWithValidation>();
    }
    protected override void DataPortal_Create()
    {

    }
  }

  public class ImAlwaysInvalidAttribute : ValidationAttribute
  {
    public ImAlwaysInvalidAttribute(string errorMessage)
      : base(errorMessage)
    { }

    public override bool IsValid(object value)
    {
      return false;
    }
  }
}