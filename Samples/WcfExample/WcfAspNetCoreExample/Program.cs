using CoreWCF.Configuration;
using CoreWCF.Description;
using Csla.Configuration;
using Csla.Channels.Wcf.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services
    .AddCsla(csla => csla
    .DataPortal(dp => dp
    .AddServerSideDataPortal(p => p
    .UseWcfPortal())));

var app = builder.Build();

var wcfPortalOptions = app.Services.GetRequiredService<WcfPortalOptions>();

app.UseServiceModel(builder =>
{
  builder.AddService<WcfPortal>()
  .AddServiceEndpoint<WcfPortal, IWcfPortalServer>(wcfPortalOptions.Binding, wcfPortalOptions.DataPortalUrl);
});

app.Run();
