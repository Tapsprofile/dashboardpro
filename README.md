# DashboardPro - Pluggable Dashboard Portal

A modern, pluggable dashboard portal built with ASP.NET Core 8 and Vue3 + D3.js for creating dynamic, data-driven dashboards with domain-agnostic architecture.

## 🎯 Features

### Backend (ASP.NET Core 8)
- **Domain-Agnostic Architecture**: `IDataProvider` interface for pluggable data sources
- **W3C IIS Log Parser**: First module implementation for IIS log analysis
- **Bulk Parsing**: Support for processing multiple log files simultaneously
- **Generic Metrics API**: RESTful API for querying metrics from any data provider
- **Extensible Design**: Easy to add new data providers and metrics

### Frontend (Vue3 + D3.js)
- **Admin Builder**: Visual dashboard creation interface
- **20+ D3 Widgets**: 
  - Conversion: Funnel Charts, Sankey Diagrams
  - Distribution: Heatmaps, Histograms, Box Plots, Violin Plots
  - Indicators: Gauges, Bullet Charts
  - Trends: Line Charts, Area Charts, Sparklines
  - Comparison: Bar Charts, Stacked Bars, Grouped Bars, Radial Bars
  - Composition: Pie Charts, Donut Charts
  - And more: Scatter Plots, Bubble Charts, Treemaps, Sunbursts, Network Graphs, Chord Diagrams, Calendar Heatmaps, Waterfall Charts
- **Pinia State Management**: Real-time filter synchronization across widgets
- **Parameter Selection**: Dynamic widget configuration with metric selection
- **JSON-Based Configuration**: Dashboard definitions stored as JSON
- **Real-Time Updates**: Auto-refresh capabilities with configurable intervals

## 📁 Project Structure

```
DashboardPro/
├── DashboardPro.sln
├── DashboardPro.API/              # ASP.NET Core 8 Web API
│   ├── Controllers/
│   │   ├── MetricsController.cs  # Generic metrics API
│   │   ├── DataController.cs     # Data upload and parsing
│   │   └── DashboardsController.cs # Dashboard CRUD operations
│   └── Program.cs
├── DashboardPro.Core/             # Domain-agnostic core library
│   ├── Interfaces/
│   │   └── IDataProvider.cs      # Pluggable data provider interface
│   └── Models/
│       ├── DataRecord.cs
│       ├── MetricDefinition.cs
│       ├── MetricQuery.cs
│       ├── MetricResult.cs
│       └── Dashboard/
│           └── DashboardConfig.cs
├── DashboardPro.Modules.IISLogs/  # IIS Log Analysis Module
│   └── Parsers/
│       └── W3CLogParser.cs       # W3C log format parser
├── DashboardPro.Web/              # Vue3 Frontend
│   ├── src/
│   │   ├── components/
│   │   │   ├── admin/
│   │   │   │   └── DashboardBuilder.vue # Dashboard builder UI
│   │   │   └── widgets/          # D3.js widget library
│   │   │       ├── FunnelChart.vue
│   │   │       ├── Heatmap.vue
│   │   │       ├── GaugeChart.vue
│   │   │       ├── LineChart.vue
│   │   │       ├── BarChart.vue
│   │   │       ├── PieChart.vue
│   │   │       └── index.js      # Widget registry (20+ widgets)
│   │   ├── stores/
│   │   │   └── dashboard.js      # Pinia store for state management
│   │   └── utils/
│   │       └── api.js            # API client
│   └── package.json
└── SampleDashboards/              # JSON dashboard samples
    ├── summary-dashboard.json
    ├── performance-dashboard.json
    └── traffic-dashboard.json
```

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js 20+
- npm or yarn

### Backend Setup

1. Build the solution:
```bash
cd /home/runner/work/dashboardpro/dashboardpro
dotnet build
```

2. Run the API:
```bash
cd DashboardPro.API
dotnet run
```

The API will be available at `http://localhost:5000` (or `https://localhost:5001`)

### Frontend Setup

1. Install dependencies:
```bash
cd DashboardPro.Web
npm install
```

2. Run the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

## 📊 Sample Dashboards

Three pre-configured dashboard JSON files are provided:

### 1. Summary Dashboard (`summary-dashboard.json`)
High-level overview with:
- Total Requests (Gauge)
- Error Rate (Gauge)
- Average Response Time (Gauge)
- Total Bandwidth (Gauge)
- Requests by Status Code (Pie Chart)
- Top 10 Endpoints (Bar Chart)

### 2. Performance Dashboard (`performance-dashboard.json`)
Detailed performance analysis:
- Response Time Trend (Line Chart)
- P95 Response Time (Gauge)
- Response Time Heatmap (Hour x Day)
- Slowest Endpoints (Bar Chart)
- Bandwidth Usage Over Time (Area Chart)
- Request Processing Funnel
- Response Time Percentiles

### 3. Traffic Dashboard (`traffic-dashboard.json`)
Traffic patterns and user behavior:
- Requests Over Time (Line Chart)
- Traffic by HTTP Method (Pie Chart)
- Unique IP Addresses (Gauge)
- Top User Agents (Bar Chart)
- Hourly Traffic Pattern Heatmap
- User Journey Funnel
- Top Referring Sites
- Status Code Distribution
- Bandwidth by Content Type
- Peak Traffic Hours

## 🔌 Architecture

### Domain-Agnostic Design

The `IDataProvider` interface allows plugging in any data source:

```csharp
public interface IDataProvider
{
    string ProviderId { get; }
    string ProviderName { get; }
    Task<IEnumerable<DataRecord>> ParseDataAsync(Stream dataStream, CancellationToken cancellationToken = default);
    Task<IEnumerable<DataRecord>> BulkParseAsync(IEnumerable<Stream> dataStreams, CancellationToken cancellationToken = default);
    Task<IEnumerable<MetricDefinition>> GetAvailableMetricsAsync();
    Task<MetricResult> GetMetricAsync(MetricQuery query, CancellationToken cancellationToken = default);
}
```

### Adding New Data Providers

1. Create a new class library project (e.g., `DashboardPro.Modules.YourDataSource`)
2. Implement the `IDataProvider` interface
3. Register in `Program.cs`:
```csharp
builder.Services.AddSingleton<IDataProvider, YourDataProvider>();
```

## 📡 API Endpoints

### Data Providers
- `GET /api/metrics/providers` - List all available data providers
- `GET /api/metrics/providers/{providerId}/metrics` - Get metrics for a provider
- `POST /api/metrics/providers/{providerId}/query` - Query metric data

### Data Upload
- `POST /api/data/providers/{providerId}/upload` - Upload single data file
- `POST /api/data/providers/{providerId}/bulk-upload` - Upload multiple files

### Dashboards
- `GET /api/dashboards` - List all dashboards
- `GET /api/dashboards/{id}` - Get dashboard by ID
- `POST /api/dashboards` - Create new dashboard
- `PUT /api/dashboards/{id}` - Update dashboard
- `DELETE /api/dashboards/{id}` - Delete dashboard

## 🎨 Using the Admin Builder

1. **Select Data Provider**: Choose the data source (e.g., IIS W3C Logs)
2. **Add Widgets**: Select widget type from 20+ available D3 visualizations
3. **Configure Widget**: 
   - Set widget title
   - Choose metric (request-count, response-time, bytes-sent, error-rate)
   - Select aggregation (sum, avg, count, p95, p99)
   - Optionally group by dimensions
4. **Add Filters**: Create dynamic filter parameters
5. **Preview**: See real-time preview of your dashboard
6. **Save or Export**: Save to database or export as JSON

## 🔄 Real-Time Filtering with Pinia

Filters are managed through Pinia store and automatically synchronized across all widgets:

```javascript
import { useDashboardStore } from '@/stores/dashboard'

const store = useDashboardStore()
store.updateFilter('status', '200')  // Updates all widgets
store.setRealTimeEnabled(true)       // Enable auto-refresh
store.setRefreshInterval(30)         // Set refresh interval (seconds)
```

## 📈 IIS Log Analysis Module

The W3C Log Parser supports:
- **W3C Extended Log Format**: Standard IIS log format
- **Bulk Processing**: Parse multiple log files concurrently
- **Metrics**:
  - Request Count: Total number of requests with grouping by method, URI, status
  - Response Time: Average, min, max, P95, P99 percentiles
  - Bytes Sent: Total bandwidth usage
  - Error Rate: Percentage of 4xx/5xx errors

### Sample W3C Log Format
```
#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs-user-agent sc-status sc-bytes time-taken
2024-01-01 10:00:00 192.168.1.1 GET /api/users - 80 - 203.0.113.0 Mozilla/5.0 200 1024 250
```

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8, C# 12
- **Frontend**: Vue 3 (Composition API), Vite
- **State Management**: Pinia
- **Visualization**: D3.js
- **HTTP Client**: Axios
- **Grid Layout**: vue-grid-layout

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

Contributions are welcome! To add new widgets or data providers:

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Submit a pull request

## 📧 Support

For issues and questions, please open an issue on the GitHub repository.
