using Csla;

namespace DataPortalCacheExample
{
  [Serializable]
  public class ReferenceData : BusinessBase<ReferenceData>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }
    public static readonly PropertyInfo<string> ValueProperty = RegisterProperty<string>(nameof(Value));
    public string Value
    {
      get => GetProperty(ValueProperty);
      set => SetProperty(ValueProperty, value);
    }

    public override string ToString() => $"{Id}:{Value}";

    [Create]
    private void Create(int id)
    {
      using (BypassPropertyChecks)
      Id = id;
      // return a unique value on each create
      Value = Guid.NewGuid().ToString();
      Console.WriteLine($"{this} !!! new item created");
    }
  }
}
