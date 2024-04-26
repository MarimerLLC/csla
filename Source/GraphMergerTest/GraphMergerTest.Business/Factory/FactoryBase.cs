using Csla;

namespace GraphMergerTest.Business
{
  public abstract class FactoryBase<T>
  {
    protected IDataPortal<T> DataPortal { get; set; }

    public FactoryBase(IDataPortal<T> dataPortal)
    {
      DataPortal = dataPortal;
    }

    public T Get(Guid id)
    {
      return DataPortal.Fetch(id);
    }

    public async Task<T> GetAsync(Guid id)
    {
      return await DataPortal.FetchAsync(id);
    }

    public void Delete(Guid id)
    {
      DataPortal.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
      await DataPortal.DeleteAsync(id);
    }
  }
}
