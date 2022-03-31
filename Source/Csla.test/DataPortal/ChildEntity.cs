//-----------------------------------------------------------------------
// <copyright file="ChildEntity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataBinding
{
  [Serializable()]
  public class ChildEntity : BusinessBase<ChildEntity>
  {
    public static PropertyInfo<int> IDProperty = RegisterProperty<int>(c => c.ID);
    public static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName);
    public static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);

    public int ID
    {
      get { return GetProperty(IDProperty); }
    }

    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    #region "Object ID value"

    protected override object GetIdValue()
    {
      return ReadProperty(IDProperty);
    }

    #endregion

    [CreateChild]
    private void Create(int id, string firstName, string lastName)
    {
      LoadProperty(IDProperty, id);
      LoadProperty(FirstNameProperty, firstName);
      LoadProperty(LastNameProperty, lastName);
    }
  }
}