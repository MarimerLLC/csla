//-----------------------------------------------------------------------
// <copyright file="DCRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Csla;

namespace Csla.Test.Serialization
{
  [DataContract]
  public class DCRoot : BusinessBase<DCRoot>
  {
    [DataMember]
    int _data;
    public static PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data, RelationshipTypes.PrivateField);
    public int Data
    {
      get { return GetProperty(DataProperty, _data); }
      set { SetProperty(DataProperty, ref _data, value); }
    }

    public NonCslaChild Child { get; } = new NonCslaChild();

    public static DCRoot NewDCRoot(IDataPortal<DCRoot> dataPortal)
    {
      return dataPortal.Create();
    }

    [Create]
    private void Create()
    {
    }
  }

  [DataContract]
  public class NonCslaChild
  {
    [DataMember]
    private int _value;

    public int TheValue
    {
      get { return _value; }
      set { _value = value; }
    }
  }
}