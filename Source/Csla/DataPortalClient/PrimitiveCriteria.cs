//-----------------------------------------------------------------------
// <copyright file="PrimitiveCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class used as a wrapper for criteria based requests that use primitives</summary>
//-----------------------------------------------------------------------

namespace Csla.DataPortalClient
{

  public class PrimitiveCriteria : ReadOnlyBase<PrimitiveCriteria>
  {
    public static readonly PropertyInfo<object> ValueProperty = RegisterProperty<object>(nameof(Value));

    public object Value
    {
      get { return GetProperty(ValueProperty)!; }
      set { LoadProperty(ValueProperty, value); }
    }
  
    [Create]
    [RunLocal]
    private void Create(object value)
    {
      Value = value;
    }
  }

}