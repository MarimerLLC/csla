using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Fluent;

[assembly: AvaloniaTestApplication(typeof(CslaAvaloniaExample.Tests.TestAppBuilder))]

namespace CslaAvaloniaExample.Tests;

public sealed class TestApplication : Application
{
  public override void Initialize()
  {
    Styles.Add(new FluentTheme());
  }
}

public static class TestAppBuilder
{
  public static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<TestApplication>()
      .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
