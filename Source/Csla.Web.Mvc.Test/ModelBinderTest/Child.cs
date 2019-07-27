//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
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

namespace Csla.Web.Mvc.Test.ModelBinderTest
{
  [Serializable()]
  public class Child : Csla.BusinessBase<Child>
  {
    public static readonly PropertyInfo<string> AnyStringProperty = RegisterProperty<string>(p => p.AnyString);
    public string AnyString
    {
      get { return GetProperty<string>(AnyStringProperty); }
      set { SetProperty<string>(AnyStringProperty, value); }
    }
    public static readonly PropertyInfo<string> Max5CharsProperty = RegisterProperty<string>(p => p.Max5Chars);
    [StringLength(5, ErrorMessage="Must be at max 5 chararcters")]
    public string Max5Chars
    {
      get { return GetProperty<string>(Max5CharsProperty); }
      set { SetProperty<string>(Max5CharsProperty, value); }
    }
    private Child()
    {}

    private void Child_Fetch(int idx)
    {
      using (BypassPropertyChecks)
      {
        AnyString = idx.ToString() + "Any";
        Max5Chars = idx.ToString() + "Max5";
      }
    }
  }
}