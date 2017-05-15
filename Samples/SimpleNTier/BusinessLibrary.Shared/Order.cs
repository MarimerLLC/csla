using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Csla;
using Csla.Rules.CommonRules;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  public class Order : BusinessBase<Order>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    public static PropertyInfo<string> CustomerNameProperty = 
      RegisterProperty<string>(p => p.CustomerName);
    [Display(Name = "Customer name")]
    [Required]
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      set { SetProperty(CustomerNameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Required(CustomerNameProperty));
    }

    public class MyExpensiveRule : Csla.Rules.BusinessRule
    {
      public MyExpensiveRule()
      {
        if (ApplicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server)
          IsAsync = true;
      }

      protected override async void Execute(Csla.Rules.RuleContext context)
      {
        MyExpensiveCommand result = null;
        if (IsAsync)
        {
          result = await MyExpensiveCommand.DoCommandAsync();
        }
        else
        {
          result = MyExpensiveCommand.DoCommandAsync().Result;
        }
        if (result == null)
          context.AddErrorResult("Command failed to run");
        else if (result.Result)
          context.AddInformationResult(result.ResultText);
        else
          context.AddErrorResult(result.ResultText);
        context.Complete();
      }
    }

    public static PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(p => p.LineItems, RelationshipTypes.LazyLoad);
    public LineItems LineItems
    {
      get
      {
        if (!FieldManager.FieldExists(LineItemsProperty))
        {
          LoadProperty(LineItemsProperty, DataPortal.CreateChild<LineItems>());
          OnPropertyChanged(LineItemsProperty);
        }
        return GetProperty(LineItemsProperty);
      }
    }
    public static async Task<Order> NewOrderAsync()
    {
      return await DataPortal.CreateAsync<Order>();
    }

    public static async Task<Order> GetOrderAsync(int id)
    {
      return await DataPortal.FetchAsync<Order>(id);
    }
  }
}