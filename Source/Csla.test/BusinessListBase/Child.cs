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
using System.Threading.Tasks;
using Csla;
using Csla.Rules;
using Csla.Serialization;

namespace Csla.Test.BusinessListBase
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }


    private static PropertyInfo<string> AsyncRuleTextProperty = RegisterProperty<string>(nameof(AsyncRuleText));
    public string AsyncRuleText 
    {
      get { return GetProperty(AsyncRuleTextProperty); }
      set { SetProperty<string>(AsyncRuleTextProperty, value);}
    }

    protected override void AddBusinessRules() 
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new OneSecondAsyncRule(AsyncRuleTextProperty));
    }

    [CreateChild]
    private void CreateChild() 
    { }

    [InsertChild]
    private void Child_Insert()
    { }

    [UpdateChild]
    private void Child_Update()
    { }

    [DeleteSelfChild]
    private void Child_DeleteSelf()
    { }


    private sealed class OneSecondAsyncRule : BusinessRuleAsync 
    {
      public OneSecondAsyncRule(Core.IPropertyInfo primaryProperty) : base(primaryProperty)
      {
        InputProperties.Add(primaryProperty);
      }

      protected override async Task ExecuteAsync(IRuleContext context) 
      {
        await Task.Delay(TimeSpan.FromSeconds(1));
      }
    }
  }
}