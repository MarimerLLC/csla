namespace Csla.Test.DataPortal
{
  [Serializable]
  public class MultipleDataAccessBase<T>
    : BusinessBase<T>
    where T : MultipleDataAccessBase<T>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    [Fetch]
    protected void Fetch()
    {
      using (BypassPropertyChecks)
      {
        Id = int.MaxValue;
      }
    }

    [Fetch]
    protected void Fetch(int id)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
      }
    }
  }
}
