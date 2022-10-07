using Csla.Configuration;

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

    builder.Services.AddTransient<DataAccess.IPersonDal, DataAccess.PersonDal>();
    builder.Services.AddCsla(options => options
      .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Xaml));
    //builder.Services.AddCsla(options => options
    //  .AddXaml());

    return builder.Build();
	}
}
