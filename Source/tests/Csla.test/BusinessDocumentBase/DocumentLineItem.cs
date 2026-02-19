//-----------------------------------------------------------------------
// <copyright file="DocumentLineItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test child object for BusinessDocumentBase tests</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Test.BusinessDocumentBase
{
  [Serializable]
  public class DocumentLineItem : BusinessBase<DocumentLineItem>
  {
    public static readonly PropertyInfo<string> DescriptionProperty = RegisterProperty<string>(nameof(Description));
    public string Description
    {
      get => GetProperty(DescriptionProperty);
      set => SetProperty(DescriptionProperty, value);
    }

    public static readonly PropertyInfo<decimal> AmountProperty = RegisterProperty<decimal>(nameof(Amount));
    public decimal Amount
    {
      get => GetProperty(AmountProperty);
      set => SetProperty(AmountProperty, value);
    }

    public static readonly PropertyInfo<string> AsyncRuleTextProperty = RegisterProperty<string>(nameof(AsyncRuleText));
    public string AsyncRuleText
    {
      get => GetProperty(AsyncRuleTextProperty);
      set => SetProperty(AsyncRuleTextProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new OneSecondAsyncRule(AsyncRuleTextProperty));
    }

    [CreateChild]
    private void CreateChild() { }

    [FetchChild]
    private void Child_Fetch(string description, decimal amount)
    {
      Description = description;
      Amount = amount;
      MarkOld();
    }

    [InsertChild]
    private void Child_Insert() { }

    [UpdateChild]
    private void Child_Update() { }

    [DeleteSelfChild]
    private void Child_DeleteSelf() { }

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
