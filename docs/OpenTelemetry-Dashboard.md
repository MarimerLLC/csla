# OpenTelemetry Dashboard for CSLA Data Portal

The `OpenTelemetryDashboard` class provides OpenTelemetry instrumentation for the CSLA data portal, enabling metrics collection for monitoring and observability.

## Overview

The OpenTelemetry Dashboard implements the `IDashboard` interface and reports metrics using the standard OpenTelemetry Metrics API. This makes it easy to integrate CSLA applications with modern observability platforms like Prometheus, Grafana, Azure Monitor, and .NET Aspire dashboards.

## Metrics

The dashboard reports the following metrics:

### Counters

- **`csla.dataportal.calls.total`**: Total number of data portal calls
  - Unit: `{call}`
  - Tags: `object.type`, `operation`

- **`csla.dataportal.calls.completed`**: Number of successfully completed data portal calls
  - Unit: `{call}`
  - Tags: `object.type`, `operation`

- **`csla.dataportal.calls.failed`**: Number of failed data portal calls
  - Unit: `{call}`
  - Tags: `object.type`, `operation`, `exception.type`

### Histograms

- **`csla.dataportal.call.duration`**: Duration of data portal calls
  - Unit: `ms` (milliseconds)
  - Tags: `object.type`, `operation`

## Usage

### Registration

To use the OpenTelemetry Dashboard, register it during application startup:

```csharp
services.AddCsla(options => options
  .DataPortal(dpo => dpo
    .AddServerSideDataPortal(config => config
      .RegisterDashboard<OpenTelemetryDashboard>()
    )
  )
);
```

### OpenTelemetry Configuration

Configure OpenTelemetry to collect the metrics:

```csharp
services.AddOpenTelemetry()
  .WithMetrics(metrics => metrics
    .AddMeter("Csla.DataPortal")
    // Add exporters as needed
    .AddPrometheusExporter()
    .AddOtlpExporter()
  );
```

### .NET Aspire Integration

For .NET Aspire applications, the dashboard automatically integrates with Aspire's built-in observability:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add service defaults which includes OpenTelemetry
builder.AddServiceDefaults();

// Configure CSLA with OpenTelemetry Dashboard
builder.Services.AddCsla(options => options
  .DataPortal(dpo => dpo
    .AddServerSideDataPortal(config => config
      .RegisterDashboard<OpenTelemetryDashboard>()
    )
  )
);
```

## Example Queries

### Prometheus Queries

```promql
# Total data portal calls per second
rate(csla_dataportal_calls_total[5m])

# Failed call rate
rate(csla_dataportal_calls_failed[5m])

# Success rate
rate(csla_dataportal_calls_completed[5m]) / rate(csla_dataportal_calls_total[5m])

# Average call duration (P50, P95, P99)
histogram_quantile(0.50, rate(csla_dataportal_call_duration_bucket[5m]))
histogram_quantile(0.95, rate(csla_dataportal_call_duration_bucket[5m]))
histogram_quantile(0.99, rate(csla_dataportal_call_duration_bucket[5m]))

# Calls by operation type
sum by (operation) (rate(csla_dataportal_calls_total[5m]))
```

## Comparison with Default Dashboard

| Feature | Default Dashboard | OpenTelemetry Dashboard |
|---------|------------------|------------------------|
| Metrics Export | No | Yes (via OpenTelemetry) |
| Recent Activity | Yes | No |
| Memory Usage | Low | Very Low |
| Integration | Built-in | OpenTelemetry ecosystem |
| Query Support | Direct properties | Metrics queries (PromQL, etc.) |

## Notes

- The OpenTelemetry Dashboard does not maintain a recent activity queue to minimize memory overhead
- Metrics are reported in real-time as operations occur
- Tags are added to metrics to enable filtering and aggregation by object type, operation, and exception type
- The dashboard is compatible with all .NET platforms supported by CSLA, including .NET Framework 4.6.2+, .NET Standard 2.0, and .NET 8.0+

## See Also

- [OpenTelemetry Metrics](https://opentelemetry.io/docs/instrumentation/net/getting-started/)
- [.NET Aspire Observability](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/telemetry)
- [CSLA Data Portal](https://cslanet.com/docs/DataPortal)
