# Quick Reference Card

## 🚀 Quick Start Commands

### Backend
```bash
# Build
dotnet build

# Run API (default: http://localhost:5000)
cd DashboardPro.API
dotnet run

# Run with watch (auto-reload)
dotnet watch run
```

### Frontend
```bash
# Install dependencies
cd DashboardPro.Web
npm install

# Run dev server (default: http://localhost:5173)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## 📡 API Quick Reference

### Base URL
```
http://localhost:5000/api
```

### Key Endpoints
```bash
# Get providers
GET /metrics/providers

# Get metrics for provider
GET /metrics/providers/iis-w3c-logs/metrics

# Query metric
POST /metrics/providers/iis-w3c-logs/query
{
  "metricId": "request-count",
  "aggregation": "sum",
  "groupBy": "uri-stem"
}

# Upload log file
POST /data/providers/iis-w3c-logs/upload
[multipart/form-data]

# List dashboards
GET /dashboards

# Create dashboard
POST /dashboards
{
  "name": "My Dashboard",
  "providerId": "iis-w3c-logs",
  "widgets": [...]
}
```

## 🎨 Widget Types

### Categories & Types
```javascript
// Conversion
'funnel', 'sankey'

// Distribution
'heatmap', 'histogram', 'box-plot', 'violin-plot'

// Indicators
'gauge', 'bullet'

// Trends
'line', 'area', 'sparkline'

// Comparison
'bar', 'stacked-bar', 'grouped-bar', 'radial-bar'

// Composition
'pie', 'donut'

// Correlation
'scatter', 'bubble'

// Hierarchy
'treemap', 'sunburst'

// Relationship
'network', 'chord'

// Time
'calendar'

// Change
'waterfall'
```

## 🔧 Common Tasks

### Add a New Data Provider

1. Create new class library:
```bash
dotnet new classlib -n DashboardPro.Modules.YourSource -f net8.0
```

2. Implement IDataProvider:
```csharp
public class YourDataProvider : IDataProvider
{
    public string ProviderId => "your-source";
    public string ProviderName => "Your Data Source";
    
    public Task<IEnumerable<DataRecord>> ParseDataAsync(Stream dataStream, ...)
    {
        // Parse your data
    }
    
    public Task<IEnumerable<MetricDefinition>> GetAvailableMetricsAsync()
    {
        // Define your metrics
    }
    
    public Task<MetricResult> GetMetricAsync(MetricQuery query, ...)
    {
        // Calculate metric
    }
}
```

3. Register in Program.cs:
```csharp
builder.Services.AddSingleton<IDataProvider, YourDataProvider>();
```

### Add a New Widget

1. Create Vue component:
```bash
# DashboardPro.Web/src/components/widgets/YourWidget.vue
```

2. Component template:
```vue
<template>
  <svg :width="width" :height="height" ref="svgRef"></svg>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as d3 from 'd3'

const props = defineProps({
  data: { type: Array, default: () => [] },
  width: { type: Number, default: 400 },
  height: { type: Number, default: 300 },
  options: { type: Object, default: () => ({}) }
})

const svgRef = ref(null)

function render() {
  // D3.js visualization code
}

onMounted(render)
watch(() => [props.data, props.width, props.height], render, { deep: true })
</script>
```

3. Register in widgets/index.js:
```javascript
import YourWidget from './YourWidget.vue'

export const widgetTypes = {
  'your-widget': { 
    component: YourWidget, 
    name: 'Your Widget', 
    category: 'Category' 
  }
}
```

### Create a Dashboard via JSON

```json
{
  "name": "My Dashboard",
  "description": "Dashboard description",
  "providerId": "iis-w3c-logs",
  "widgets": [
    {
      "type": "gauge",
      "title": "Total Requests",
      "query": {
        "metricId": "request-count",
        "aggregation": "sum"
      },
      "options": {
        "color": "#4CAF50",
        "min": 0,
        "max": 10000
      },
      "position": {
        "x": 0,
        "y": 0,
        "width": 3,
        "height": 3
      }
    }
  ],
  "filters": {
    "parameters": [
      {
        "name": "Date Range",
        "type": "date-range",
        "field": "timestamp"
      }
    ],
    "realTimeEnabled": true,
    "refreshInterval": 30
  }
}
```

Save via API:
```bash
curl -X POST http://localhost:5000/api/dashboards \
  -H "Content-Type: application/json" \
  -d @my-dashboard.json
```

## 📊 Available Metrics (IIS Logs)

```javascript
{
  "request-count": {
    aggregations: ["count", "sum"],
    dimensions: ["method", "uri-stem", "status"]
  },
  "response-time": {
    aggregations: ["avg", "min", "max", "p95", "p99"],
    dimensions: ["method", "uri-stem"]
  },
  "bytes-sent": {
    aggregations: ["sum", "avg"],
    dimensions: ["uri-stem", "status"]
  },
  "error-rate": {
    aggregations: ["rate", "count"],
    dimensions: ["status", "uri-stem"]
  }
}
```

## 🔍 Troubleshooting

### Backend won't start
```bash
# Check if port 5000 is in use
netstat -an | grep 5000

# Change port in launchSettings.json
# DashboardPro.API/Properties/launchSettings.json
```

### Frontend build errors
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Check Node version (requires 20+)
node --version
```

### CORS issues
Update Program.cs:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

app.UseCors("AllowFrontend");
```

## 🧪 Testing

### Test API with curl
```bash
# Get providers
curl http://localhost:5000/api/metrics/providers

# Upload sample log
curl -X POST http://localhost:5000/api/data/providers/iis-w3c-logs/upload \
  -F "file=@SampleData/sample-iis-log.log"

# Query metrics
curl -X POST http://localhost:5000/api/metrics/providers/iis-w3c-logs/query \
  -H "Content-Type: application/json" \
  -d '{"metricId":"request-count","aggregation":"sum"}'
```

### Test Frontend
1. Open browser: http://localhost:5173
2. Open browser console (F12)
3. Check for errors
4. Verify API calls in Network tab

## 📁 Project Structure

```
DashboardPro/
├── DashboardPro.Core/          # Domain models & interfaces
├── DashboardPro.Modules.IISLogs/  # IIS log parser
├── DashboardPro.API/           # REST API
├── DashboardPro.Web/           # Vue frontend
├── SampleDashboards/           # JSON examples
├── SampleData/                 # Test data
└── *.md                        # Documentation
```

## 🌐 Environment Variables

### Backend
```bash
# appsettings.json or environment
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000
```

### Frontend
```bash
# .env
VITE_API_URL=http://localhost:5000/api
```

## 📚 Documentation Links

- **README.md** - Project overview
- **GETTING_STARTED.md** - Setup guide
- **API_DOCUMENTATION.md** - API reference
- **ARCHITECTURE.md** - System design
- **PROJECT_SUMMARY.md** - Complete summary

## 💡 Tips

1. **Use TypeScript**: Consider migrating frontend to TypeScript for better type safety
2. **Add Tests**: Implement unit and integration tests
3. **Use Docker**: Create Dockerfile for easy deployment
4. **Add Auth**: Implement JWT/OAuth for production
5. **Cache Results**: Add Redis for query caching
6. **Monitor**: Integrate Application Insights or similar
7. **Version API**: Add API versioning for breaking changes

## 🎯 Quick Examples

### Load Sample Dashboard
```bash
curl -X POST http://localhost:5000/api/dashboards \
  -H "Content-Type: application/json" \
  -d @SampleDashboards/summary-dashboard.json
```

### Query with Filters
```bash
curl -X POST http://localhost:5000/api/metrics/providers/iis-w3c-logs/query \
  -H "Content-Type: application/json" \
  -d '{
    "metricId": "request-count",
    "aggregation": "count",
    "groupBy": "sc-status",
    "filters": {
      "cs-method": "GET"
    }
  }'
```

---

**Need help?** Check the full documentation or open an issue on GitHub.
