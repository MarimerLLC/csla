using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules;

namespace Csla.Blazor.Test
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new TwoSecondDelayAsyncRule(NameProperty));
    }

    [Create]
    private void Create()
    {
      base.Child_Create();
    }

    [Insert]
    private void Insert()
    {
    }

    private sealed class TwoSecondDelayAsyncRule : BusinessRuleAsync
    {
      public TwoSecondDelayAsyncRule(Csla.Core.IPropertyInfo primaryProperty) : base(primaryProperty)
      {
      }

      /// <inheritdoc />
      protected override async Task ExecuteAsync(IRuleContext context)
      {
        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
      }
    }
  }
}
