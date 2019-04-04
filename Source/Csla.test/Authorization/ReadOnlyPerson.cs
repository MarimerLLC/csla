//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPerson.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Diagnostics;

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  [Serializable()]
  public class ReadOnlyPerson : ReadOnlyBase<ReadOnlyPerson>
  {
    private ReadOnlyPerson() 
    {
      LoadProperty(FirstNameProperty, "John");
      LoadProperty(LastNameProperty, "Doe");
      LoadProperty(MiddleNameProperty, "A");
      LoadProperty(PlaceOfBirthProperty, "New York");
    }
    public static ReadOnlyPerson GetReadOnlyPerson()
    {
      return new ReadOnlyPerson();
    }
    private static PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(new PropertyInfo<string>("FirstName"));
    public string FirstName
    {
      get
      {
        return GetProperty(FirstNameProperty, Csla.Security.NoAccessBehavior.ThrowException);
      }
    }

   
    private static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(new PropertyInfo<string>("LastName"));

    public string LastName
    {
      get
      {
        return GetProperty(LastNameProperty, Csla.Security.NoAccessBehavior.ThrowException);
      }
    }

    private static PropertyInfo<string> MiddleNameProperty = RegisterProperty<string>(new PropertyInfo<string>("MiddleName"));
    public string MiddleName
    {
      get
      {
        return GetProperty(MiddleNameProperty, Csla.Security.NoAccessBehavior.ThrowException);
      }
    }


    private static PropertyInfo<string> PlaceOfBirthProperty = RegisterProperty<string>(new PropertyInfo<string>("PlaceOfBirth"));

    public string PlaceOfBirth
    {
      get
      {
        return GetProperty(PlaceOfBirthProperty, Csla.Security.NoAccessBehavior.ThrowException);
      }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, FirstNameProperty, new List<string> { "Admin" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.ReadProperty, MiddleNameProperty, new List<string> { "Admin" }));
    }
    //protected override void AddInstanceAuthorizationRules()
    //{
    //  base.AddInstanceAuthorizationRules();
    //  AuthorizationRules.InstanceAllowRead("LastName", "Admin");
    //  AuthorizationRules.InstanceDenyRead("PlaceOfBirth", "Admin");
    //}

    //protected override object GetIdValue()
    //{
    //  return null;
    //}
  }
}