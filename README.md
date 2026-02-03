# Dashboard Portal Product

Metadata-driven, domain-agnostic analytics platform that turns raw data into
pluggable dashboards. The first module is an IIS W3C log analyzer, but the core
engine is provider-based so other sources (ERP, custom app DB, etc.) can be
swapped without rewriting the frontend.

## Clean Architecture Map

```
backend/
  src/
    DashboardPortal.Api/               # ASP.NET Core 8 web API + RBAC
      Controllers/
      Program.cs
    DashboardPortal.Application/       # Use cases, contracts, aggregation
      Abstractions/
      Analytics/
      Services/
    DashboardPortal.Domain/            # Entities + dashboard metadata
      Entities/
      Configurations/
    DashboardPortal.Infrastructure/    # Providers, parsing, background jobs
      Background/
      Parsing/
      Persistence/
      Providers/
      Security/
frontend/
  src/
    components/
      BaseChart.vue                    # D3 container + resize logic
      WidgetRenderer.vue
      widgets/
    registry/
    stores/
    types/
    views/
samples/
  executive-health.json
  performance-deep-dive.json
  traffic-security.json
```

## Backend Highlights (ASP.NET Core 8)

- **IDataProvider** abstraction in `Application/Abstractions/Data`.
- **IIS W3C parser** uses `IAsyncEnumerable<LogEntry>` for streaming imports.
- **DashboardAggregationService** translates dashboard JSON into grouped
  queries (metric + dimension) and returns widget results.
- **RBAC** uses Admin/User roles with API policies.
- **Bulk Imports** are queued with `ImportQueue` and processed by
  `LogImportWorker` (background service).
- **Index Plan** in `Infrastructure/Persistence` documents recommended indexes
  for large log datasets.

## Frontend Highlights (Vue 3 + D3.js)

- **BaseChart.vue** owns the SVG container, responsive sizing, and re-render.
- **Widget Registry** maps widget types to renderers (20+ types defined).
- **Pinia store** drives cross-widget filtering.
- **Dashboard Builder** shows a real-time preview of a selected metric and
  dimension.

## Sample Dashboard JSON

See `samples/` for the provided admin-configured dashboards:

- `executive-health.json` (KPIs + Gauges)
- `performance-deep-dive.json` (Line + Heatmap)
- `traffic-security.json` (Tables + Funnels)
