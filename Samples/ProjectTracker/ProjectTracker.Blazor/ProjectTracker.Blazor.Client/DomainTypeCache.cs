using Csla;
using Csla.Server;
using Csla.DataPortalClient;
using ProjectTracker.Library;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace ProjectTracker.Blazor.Client
{
  public class DataPortalCache(IMemoryCache cache) : IDataPortalCache
  {
    private readonly IMemoryCache _cache = cache;

    public async Task<DataPortalResult> GetDataPortalResultAsync(Type objectType, object criteria, DataPortalOperations operation, Func<Task<DataPortalResult>> portal)
    {
      if (operation == DataPortalOperations.Fetch && objectType == typeof(RoleList))
      {
        // this operation + type is cached
        return await GetResultAsync(objectType, criteria, operation, portal);
      }
      else
      {
        // the result isn't cached
        return await portal();
      }
    }

    private async Task<DataPortalResult> GetResultAsync(Type objectType, object criteria, DataPortalOperations operation, Func<Task<DataPortalResult>> portal)
    {
      var key = GetKey(objectType, criteria, operation);
      var result = await _cache.GetOrCreateAsync(key, async (v) =>
      {
        var obj = await portal();
        v.AbsoluteExpiration = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        return obj;
      });
      return result ?? await portal();
    }

    private static string GetKey(Type objectType, object criteria, DataPortalOperations operation)
    {
      var builder = new StringBuilder();
      // requested type
      builder.Append(objectType.FullName ?? objectType.Name);
      builder.Append('|');

      // criteria values (each criteria has 'valid' ToString)
      var criteriaList = Csla.Server.DataPortal.GetCriteriaArray(criteria);
      if (criteriaList is not null)
      {
        foreach (var item in criteriaList)
        {
          builder.Append(item?.ToString() ?? string.Empty);
          builder.Append('|');
        }
      }

      // operation
      builder.Append(operation.ToString());
      return builder.ToString();
    }
  }
}
