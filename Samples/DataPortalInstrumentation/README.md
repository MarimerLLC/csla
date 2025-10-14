# Data Portal Instrumentation Sample

This sample demonstrates how to use the CSLA data portal dashboard features for monitoring and observability.

## Dashboard Options

CSLA provides multiple dashboard implementations:

### 1. Default Dashboard (`Csla.Server.Dashboard.Dashboard`)

The default dashboard maintains in-memory metrics and a recent activity queue.

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .DataPortal(dp => dp
    .AddServerSideDataPortal(ss => ss
      .RegisterDashboard<Csla.Server.Dashboard.Dashboard>())));
```

Access dashboard data via the `IDashboard` interface:
- `TotalCalls` - Total number of data portal calls
- `CompletedCalls` - Number of successful calls
- `FailedCalls` - Number of failed calls
- `GetRecentActivity()` - Returns a list of recent data portal operations

### 2. OpenTelemetry Dashboard (`Csla.Server.Dashboard.OpenTelemetryDashboard`)

The OpenTelemetry dashboard exports metrics to OpenTelemetry collectors for integration with modern observability platforms like Prometheus, Grafana, Azure Monitor, and .NET Aspire.

```csharp
using OpenTelemetry.Metrics;

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
  .WithMetrics(metrics => metrics
    .AddMeter("Csla.DataPortal")
    .AddPrometheusExporter()  // or other exporters
  );

// Configure CSLA with OpenTelemetry Dashboard
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .DataPortal(dp => dp
    .AddServerSideDataPortal(ss => ss
      .RegisterDashboard<Csla.Server.Dashboard.OpenTelemetryDashboard>())));
```

Metrics available:
- `csla.dataportal.calls.total` - Counter for total calls
- `csla.dataportal.calls.completed` - Counter for successful calls
- `csla.dataportal.calls.failed` - Counter for failed calls
- `csla.dataportal.call.duration` - Histogram of call durations in milliseconds

All metrics include tags for `object.type` and `operation` to enable filtering and aggregation.

### 3. Null Dashboard (`Csla.Server.Dashboard.NullDashboard`)

A no-op dashboard that records nothing, for production environments where monitoring is not needed.

```csharp
builder.Services.AddCsla(o => o
  .AddAspNetCore()
  .DataPortal(dp => dp
    .AddServerSideDataPortal(ss => ss
      .RegisterDashboard<Csla.Server.Dashboard.NullDashboard>())));
```

## Running the Sample

1. Update `Program.cs` to use the desired dashboard implementation
2. Run the application
3. Navigate to the app and trigger some data portal operations
4. For the default dashboard, access the dashboard via the API controller at `/api/DataPortal`
5. For OpenTelemetry dashboard, use your configured metrics backend to query the metrics

## See Also

- [OpenTelemetry Dashboard Documentation](../../../docs/OpenTelemetry-Dashboard.md)
- [CSLA Data Portal](https://cslanet.com/docs/DataPortal)
