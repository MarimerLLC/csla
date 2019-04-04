//-----------------------------------------------------------------------
// <copyright file="SerializationRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Serialization
{
  [Serializable()]
  public class SerializationRoot : BusinessBase<SerializationRoot>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data, RelationshipTypes.PrivateField);
    private string _data = DataProperty.DefaultValue;
    public string Data
    {
      get { return GetProperty(DataProperty, _data); }
      set { SetProperty(DataProperty, ref _data, value); }
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);
      Csla.ApplicationContext.GlobalContext.Add("Deserialized", true);
      Console.WriteLine("OnDeserialized");
    }
  }
}