using Avalonia.Headless.XUnit;
using BusinessLibrary;
using Csla;
using Csla.Configuration;
using Csla.Xaml;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CslaAvaloniaExample.Tests;

public sealed class PropertyInfoTests
{
  [AvaloniaFact]
  public async Task BrokenRules_follow_Name_required_rule()
  {
    using var services = CreateServices();

    var portal = services.GetRequiredService<IDataPortal<PersonEdit>>();
    var person = await portal.CreateAsync();

    var propertyInfo = new PropertyInfo
    {
      Path = nameof(PersonEdit.Name),
      DataContext = person
    };

    Assert.False(propertyInfo.IsValid);
    Assert.NotEmpty(propertyInfo.BrokenRules);
    Assert.Contains(
      propertyInfo.BrokenRules,
      rule => rule.Property == nameof(PersonEdit.Name));
    Assert.False(string.IsNullOrWhiteSpace(propertyInfo.ErrorText));

    person.Name = "Ada Lovelace";

    Assert.True(propertyInfo.IsValid);
    Assert.Empty(propertyInfo.BrokenRules);
    Assert.True(string.IsNullOrWhiteSpace(propertyInfo.ErrorText));

    person.Name = string.Empty;

    Assert.False(propertyInfo.IsValid);
    Assert.NotEmpty(propertyInfo.BrokenRules);
    Assert.Contains(
      propertyInfo.BrokenRules,
      rule => rule.Property == nameof(PersonEdit.Name));
  }

  private static ServiceProvider CreateServices()
  {
    var services = new ServiceCollection();

    services.AddCsla();
    services.AddSingleton<IPersonDal, PersonDal>();

    return services.BuildServiceProvider();
  }
}
