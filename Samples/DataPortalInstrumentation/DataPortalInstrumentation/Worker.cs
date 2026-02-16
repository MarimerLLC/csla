using Csla;
using System;
using System.Threading.Tasks;

namespace DataPortalInstrumentation
{
  [CslaImplementProperties]
  public partial class Worker : BusinessBase<Worker>
  {
    public partial int Id { get; set; }

    [Fetch]
    private async Task Fetch(int id)
    {
      // TODO: load values into object
      await Task.Delay(300);
      LoadProperty(IdProperty, id);
    }

    [Insert]
    private async Task Insert()
    {
      // TODO: insert object's data
      await Task.CompletedTask;
    }

    [Update]
    private async Task Update()
    {
      // TODO: update object's data
      await Task.CompletedTask;
    }

    [DeleteSelf]
    private async Task DeleteSelf()
    {
      await Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private async Task Delete(int id)
    {
      // TODO: delete object's data
      await Task.CompletedTask;
    }
  }
}
