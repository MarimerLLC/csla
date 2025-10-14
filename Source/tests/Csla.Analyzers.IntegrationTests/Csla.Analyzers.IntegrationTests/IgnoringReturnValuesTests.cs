using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class UsedByCallingMethods
    : BusinessBase<UsedByCallingMethods>
  { }

  public class CallingMethods
  {
    private UsedByCallingMethods value;

    public CallingMethods Save() { return null; }

    public async Task<CallingMethods> SaveAsync() { return await Task.FromResult<CallingMethods>(null); }

    public async Task UsedByCallingMethodsSaveAsync()
    {
      var x = DataPortal.Fetch<UsedByCallingMethods>();

      // This should have an error because it doesn't set the return value
      await x.SaveAsync();

      await this.SaveAsync();

      // This should have an error because it doesn't set the return value
      await x.SaveAsync(true).ConfigureAwait(false);

      x = await x.SaveAsync();

      var a = await x.SaveAsync();

      this.value = await x.SaveAsync();

      await DataPortal.Fetch<UsedByCallingMethods>().SaveAsync();
    }

    public void UsedByCallingMethodsSave()
    {
      var x = DataPortal.Fetch<UsedByCallingMethods>();

      // This should have an error because it doesn't set the return value
      x.Save();

      this.Save();

      // This should have an error because it doesn't set the return value
      x.Save(true);

      x = x.Save();

      var a = x.Save();

      // This should have an error because it doesn't set the return value
      x.Save(true);

      this.value = x.Save();

      this.DoThis(() => { this.value = x.Save(); });

      this.DoThis(() => this.value = x.Save());

      // This should have an error because it doesn't set the return value
      this.DoThis(() => { x.Save(); });

      this.ReturnThis(() => x.Save());

      this.ReturnThis(() => { return x.Save(); });

      this.ReturnThis(() =>
      {
        var q = DataPortal.Fetch<UsedByCallingMethods>();
        // This should have an error because it doesn't set the return value
        q.Save();
        return null;
      });

      DataPortal.Fetch<UsedByCallingMethods>().Save();
    }

    public UsedByCallingMethods ReturnsUser()
    {
      var x = DataPortal.Fetch<UsedByCallingMethods>();
      return x.Save();
    }

    public async Task<UsedByCallingMethods> ReturnsUserAsync()
    {
      var x = DataPortal.Fetch<UsedByCallingMethods>();
      return await x.SaveAsync();
    }

    private void DoThis(Action a)
    {
      a();
    }

    private UsedByCallingMethods ReturnThis(Func<UsedByCallingMethods> a)
    {
      return a();
    }
  }
}