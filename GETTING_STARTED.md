# Getting Started with DashboardPro

This guide will help you get up and running with DashboardPro in just a few minutes.

## Prerequisites

- .NET 8.0 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- Node.js 20+ ([Download](https://nodejs.org/))
- A code editor (VS Code, Visual Studio, or your preferred IDE)

## Step 1: Clone and Build the Backend

1. Navigate to the project directory:
```bash
cd /home/runner/work/dashboardpro/dashboardpro
```

2. Build the solution:
```bash
dotnet build
```

3. Run the API:
```bash
cd DashboardPro.API
dotnet run
```

The API will start at `http://localhost:5000` or `https://localhost:5001`.

## Step 2: Set Up the Frontend

1. Open a new terminal and navigate to the web project:
```bash
cd DashboardPro.Web
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`.

## Step 3: Upload Sample Data

You can test the system with the provided sample IIS log file:

1. Open the frontend in your browser: `http://localhost:5173`
2. The Dashboard Builder will be displayed
3. Click "Select Data Provider" and choose "IIS W3C Log Parser"
4. Upload the sample log file from `SampleData/sample-iis-log.log`

Alternatively, use the API directly:

```bash
curl -X POST http://localhost:5000/api/data/providers/iis-w3c-logs/upload \
  -F "file=@SampleData/sample-iis-log.log"
```

## Step 4: Create Your First Dashboard

### Using the Admin Builder (Recommended)

1. In the Dashboard Builder interface:
   - Enter a dashboard name (e.g., "My First Dashboard")
   - Select "IIS W3C Log Parser" as the data provider
   - Click "Add Widget"
   - Choose a widget type (e.g., "Gauge Chart")
   - Configure the widget:
     - Title: "Total Requests"
     - Metric: "Request Count"
     - Aggregation: "sum"
   - Click "Save Dashboard"

2. Add more widgets:
   - Bar Chart for "Top Endpoints"
   - Line Chart for "Response Time Trend"
   - Pie Chart for "Requests by Status"

### Using Pre-built Dashboard Templates

Load one of the sample dashboards:

1. **Summary Dashboard**:
```bash
curl -X POST http://localhost:5000/api/dashboards \
  -H "Content-Type: application/json" \
  -d @SampleDashboards/summary-dashboard.json
```

2. **Performance Dashboard**:
```bash
curl -X POST http://localhost:5000/api/dashboards \
  -H "Content-Type: application/json" \
  -d @SampleDashboards/performance-dashboard.json
```

3. **Traffic Dashboard**:
```bash
curl -X POST http://localhost:5000/api/dashboards \
  -H "Content-Type: application/json" \
  -d @SampleDashboards/traffic-dashboard.json
```

## Step 5: Query Metrics

### Get Available Providers
```bash
curl http://localhost:5000/api/metrics/providers
```

### Get Available Metrics
```bash
curl http://localhost:5000/api/metrics/providers/iis-w3c-logs/metrics
```

### Query Specific Metric
```bash
curl -X POST http://localhost:5000/api/metrics/providers/iis-w3c-logs/query \
  -H "Content-Type: application/json" \
  -d '{
    "metricId": "request-count",
    "aggregation": "sum",
    "groupBy": "sc-status"
  }'
```

## Step 6: Explore Widget Types

The system includes 20+ D3.js widget types organized by category:

### Conversion Analysis
- **Funnel Chart**: Visualize conversion funnels
- **Sankey Diagram**: Show flow between states

### Distribution
- **Heatmap**: Show patterns across two dimensions
- **Histogram**: Display distribution of values
- **Box Plot**: Statistical distribution visualization

### Performance Indicators
- **Gauge**: Display single metric values
- **Bullet Chart**: Compare actual vs target

### Trends
- **Line Chart**: Time series data
- **Area Chart**: Cumulative trends
- **Sparkline**: Compact trend visualization

### Comparisons
- **Bar Chart**: Compare categories
- **Stacked Bar**: Compare components
- **Radial Bar**: Circular comparisons

### And many more...

## Step 7: Add Real-Time Filtering

1. In the Dashboard Builder, scroll to the "Filters" section
2. Click "Add Filter Parameter"
3. Configure the filter:
   - Name: "Date Range"
   - Type: "date-range"
   - Field: "timestamp"
4. Enable real-time updates:
   - Check "Real-Time Enabled"
   - Set refresh interval (e.g., 30 seconds)

Filters will automatically apply to all widgets in the dashboard!

## Next Steps

### Create a Custom Data Provider

1. Create a new class library:
```bash
dotnet new classlib -n DashboardPro.Modules.YourDataSource
```

2. Implement `IDataProvider`:
```csharp
public class YourDataProvider : IDataProvider
{
    public string ProviderId => "your-data-source";
    public string ProviderName => "Your Data Source";
    
    // Implement interface methods...
}
```

3. Register in `Program.cs`:
```csharp
builder.Services.AddSingleton<IDataProvider, YourDataProvider>();
```

### Extend Widget Library

Add new D3.js visualizations by creating Vue components in:
`DashboardPro.Web/src/components/widgets/`

Follow the existing widget patterns for consistency.

### Deploy to Production

#### Backend (IIS/Azure):
```bash
dotnet publish -c Release
```

#### Frontend:
```bash
npm run build
```

Deploy the `dist` folder to your web server.

## Troubleshooting

### Port Already in Use
If port 5000 or 5173 is already in use:

Backend: Edit `DashboardPro.API/Properties/launchSettings.json`
Frontend: The dev server will automatically use the next available port

### CORS Issues
The backend is configured to allow all origins in development. For production, update the CORS policy in `Program.cs`.

### Build Errors
Make sure you have the correct SDK versions:
```bash
dotnet --version  # Should be 8.0 or higher
node --version    # Should be 20.0 or higher
```

## Need Help?

- Check the main [README.md](README.md) for detailed documentation
- Review the sample dashboards in `SampleDashboards/`
- Examine the sample log file in `SampleData/`
- Open an issue on GitHub for support

## What's Next?

- Explore advanced widget configurations
- Create complex dashboard layouts
- Build custom data providers for your data sources
- Integrate with your existing systems
- Deploy to production

Happy dashboard building! 🚀
