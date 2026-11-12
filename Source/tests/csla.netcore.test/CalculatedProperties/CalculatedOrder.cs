//-----------------------------------------------------------------------
// <copyright file="CalculatedOrder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Parent and child business objects with calculated properties</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Test.CalculatedProperties
{
  // A parent whose calculated Total sums the calculated Amount of its child lines
  [Serializable]
  public partial class CalculatedOrder : BusinessBase<CalculatedOrder>
  {
    public static readonly PropertyInfo<CalculatedOrderLines> LinesProperty = RegisterProperty<CalculatedOrderLines>(nameof(Lines));
    public CalculatedOrderLines Lines
    {
      get => GetProperty(LinesProperty);
      private set => LoadProperty(LinesProperty, value);
    }

    public static readonly PropertyInfo<decimal> TotalProperty = RegisterProperty<decimal>(nameof(Total));
    [CalculatedProperty]
    public decimal Total => GetProperty(TotalProperty);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new OrderTotalRule(TotalProperty));
    }

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<CalculatedOrderLines> linesPortal)
    {
      Lines = linesPortal.FetchChild();
      BusinessRules.CheckRules();
    }

    private class OrderTotalRule : BusinessRule
    {
      public OrderTotalRule(IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        var order = (CalculatedOrder)context.Target;
        context.AddOutValue(PrimaryProperty, order.Lines.Sum(l => l.Amount));
      }
    }
  }

  [Serializable]
  public partial class CalculatedOrderLines : BusinessListBase<CalculatedOrderLines, CalculatedOrderLine>
  {
    [FetchChild]
    private void Fetch([Inject] IChildDataPortal<CalculatedOrderLine> linePortal)
    {
      using (LoadListMode)
      {
        Add(linePortal.FetchChild(2m, 10m));
        Add(linePortal.FetchChild(3m, 5m));
      }
    }
  }

  [Serializable]
  public partial class CalculatedOrderLine : BusinessBase<CalculatedOrderLine>
  {
    public static readonly PropertyInfo<decimal> QuantityProperty = RegisterProperty<decimal>(nameof(Quantity));
    public decimal Quantity
    {
      get => GetProperty(QuantityProperty);
      set => SetProperty(QuantityProperty, value);
    }

    public static readonly PropertyInfo<decimal> PriceProperty = RegisterProperty<decimal>(nameof(Price));
    public decimal Price
    {
      get => GetProperty(PriceProperty);
      set => SetProperty(PriceProperty, value);
    }

    public static readonly PropertyInfo<decimal> AmountProperty = RegisterProperty<decimal>(nameof(Amount));
    [CalculatedProperty]
    public decimal Amount => GetProperty(AmountProperty);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new AmountRule(AmountProperty, QuantityProperty, PriceProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Dependency(QuantityProperty, AmountProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Dependency(PriceProperty, AmountProperty));
    }

    [FetchChild]
    private void Fetch(decimal quantity, decimal price)
    {
      LoadProperty(QuantityProperty, quantity);
      LoadProperty(PriceProperty, price);
      BusinessRules.CheckRules();
    }

    private class AmountRule : BusinessRule
    {
      private readonly IPropertyInfo _quantity;
      private readonly IPropertyInfo _price;

      public AmountRule(IPropertyInfo primaryProperty, IPropertyInfo quantity, IPropertyInfo price)
        : base(primaryProperty)
      {
        _quantity = quantity;
        _price = price;
        InputProperties.Add(quantity);
        InputProperties.Add(price);
      }

      protected override void Execute(IRuleContext context)
      {
        var amount = (decimal)context.InputPropertyValues[_quantity] * (decimal)context.InputPropertyValues[_price];
        context.AddOutValue(PrimaryProperty, amount);
      }
    }
  }
}
