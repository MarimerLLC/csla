using Csla.Configuration;
using MauiExample.ViewModels;

namespace MauiExample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

    builder.Services.AddCsla(options => options
      .AddXaml());

    AddDependancyInjection(builder);

    return builder.Build();
	}

  private static void AddDependancyInjection(MauiAppBuilder builder)
  {
    builder.Services.AddTransient<DataAccess.IPersonDal, DataAccess.PersonDal>();

    builder.Services.AddScoped<PersonEditViewModel>();
    builder.Services.AddScoped<PersonListViewModel>();
  }
}
