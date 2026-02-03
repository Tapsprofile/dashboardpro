# API Documentation

Complete API reference for DashboardPro backend services.

## Base URL

```
http://localhost:5000/api
```

## Authentication

Currently, the API does not require authentication. In production, you should implement proper authentication and authorization.

---

## Data Providers

### List All Providers

Get all registered data providers.

**Endpoint:** `GET /metrics/providers`

**Response:**
```json
[
  {
    "providerId": "iis-w3c-logs",
    "providerName": "IIS W3C Log Parser"
  }
]
```

---

### Get Provider Metrics

Get available metrics for a specific provider.

**Endpoint:** `GET /metrics/providers/{providerId}/metrics`

**Parameters:**
- `providerId` (path): Provider identifier (e.g., "iis-w3c-logs")

**Response:**
```json
[
  {
    "metricId": "request-count",
    "metricName": "Request Count",
    "description": "Total number of requests",
    "type": "Counter",
    "availableDimensions": ["method", "uri-stem", "status"],
    "availableAggregations": ["count", "sum"]
  },
  {
    "metricId": "response-time",
    "metricName": "Response Time",
    "description": "Time taken to process requests",
    "type": "Histogram",
    "availableDimensions": ["method", "uri-stem"],
    "availableAggregations": ["avg", "min", "max", "p95", "p99"]
  }
]
```

---

### Query Metric Data

Execute a metric query and retrieve results.

**Endpoint:** `POST /metrics/providers/{providerId}/query`

**Parameters:**
- `providerId` (path): Provider identifier

**Request Body:**
```json
{
  "metricId": "request-count",
  "startTime": "2024-01-01T00:00:00Z",
  "endTime": "2024-01-31T23:59:59Z",
  "dimensions": ["method", "status"],
  "aggregation": "sum",
  "filters": {
    "method": "GET"
  },
  "limit": 100,
  "groupBy": "uri-stem"
}
```

**Request Fields:**
- `metricId` (required): Metric identifier
- `startTime` (optional): Start of time range
- `endTime` (optional): End of time range
- `dimensions` (optional): Dimensions to include
- `aggregation` (optional): Aggregation method (default: "sum")
- `filters` (optional): Filter criteria
- `limit` (optional): Maximum number of results
- `groupBy` (optional): Field to group results by

**Response:**
```json
{
  "metricId": "request-count",
  "dataPoints": [
    {
      "timestamp": "2024-01-15T08:15:00Z",
      "value": 150,
      "labels": {
        "uri-stem": "/api/users"
      }
    },
    {
      "timestamp": "2024-01-15T08:15:00Z",
      "value": 89,
      "labels": {
        "uri-stem": "/api/products"
      }
    }
  ],
  "metadata": {}
}
```

---

## Data Upload

### Upload Single File

Upload and parse a single data file.

**Endpoint:** `POST /data/providers/{providerId}/upload`

**Parameters:**
- `providerId` (path): Provider identifier

**Request:**
- Content-Type: `multipart/form-data`
- Field: `file` (file upload)

**Response:**
```json
{
  "recordCount": 1523,
  "message": "Data parsed successfully"
}
```

**Example:**
```bash
curl -X POST http://localhost:5000/api/data/providers/iis-w3c-logs/upload \
  -F "file=@/path/to/logfile.log"
```

---

### Bulk Upload Files

Upload and parse multiple data files at once.

**Endpoint:** `POST /data/providers/{providerId}/bulk-upload`

**Parameters:**
- `providerId` (path): Provider identifier

**Request:**
- Content-Type: `multipart/form-data`
- Field: `files` (multiple file uploads)

**Response:**
```json
{
  "fileCount": 5,
  "recordCount": 7815,
  "message": "Data parsed successfully"
}
```

**Example:**
```bash
curl -X POST http://localhost:5000/api/data/providers/iis-w3c-logs/bulk-upload \
  -F "files=@log1.log" \
  -F "files=@log2.log" \
  -F "files=@log3.log"
```

---

## Dashboards

### List All Dashboards

Get all saved dashboards.

**Endpoint:** `GET /dashboards`

**Response:**
```json
[
  {
    "id": "dashboard-123",
    "name": "Summary Dashboard",
    "description": "High-level overview",
    "providerId": "iis-w3c-logs",
    "widgets": [...],
    "filters": {...},
    "layout": {...},
    "createdAt": "2024-01-15T08:00:00Z",
    "updatedAt": "2024-01-15T09:30:00Z"
  }
]
```

---

### Get Dashboard by ID

Retrieve a specific dashboard.

**Endpoint:** `GET /dashboards/{id}`

**Parameters:**
- `id` (path): Dashboard identifier

**Response:**
```json
{
  "id": "dashboard-123",
  "name": "Summary Dashboard",
  "description": "High-level overview of IIS performance",
  "providerId": "iis-w3c-logs",
  "widgets": [
    {
      "id": "widget-1",
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
        "field": "timestamp",
        "defaultValue": null
      }
    ],
    "realTimeEnabled": true,
    "refreshInterval": 30
  },
  "layout": {
    "columns": 12,
    "rowHeight": 100,
    "theme": "light"
  },
  "createdAt": "2024-01-15T08:00:00Z",
  "updatedAt": "2024-01-15T09:30:00Z"
}
```

---

### Create Dashboard

Create a new dashboard.

**Endpoint:** `POST /dashboards`

**Request Body:**
```json
{
  "name": "My Dashboard",
  "description": "Custom dashboard",
  "providerId": "iis-w3c-logs",
  "widgets": [],
  "filters": {
    "parameters": [],
    "realTimeEnabled": false,
    "refreshInterval": 30
  },
  "layout": {
    "columns": 12,
    "rowHeight": 100,
    "theme": "light"
  }
}
```

**Response:**
```json
{
  "id": "dashboard-456",
  "name": "My Dashboard",
  ...
}
```

---

### Update Dashboard

Update an existing dashboard.

**Endpoint:** `PUT /dashboards/{id}`

**Parameters:**
- `id` (path): Dashboard identifier

**Request Body:**
Same as Create Dashboard

**Response:**
Updated dashboard object

---

### Delete Dashboard

Delete a dashboard.

**Endpoint:** `DELETE /dashboards/{id}`

**Parameters:**
- `id` (path): Dashboard identifier

**Response:**
- Status: 204 No Content

---

## Metric Types

### Counter
Cumulative metric that only increases (e.g., request count, bytes sent).

**Aggregations:** sum, count

### Gauge  
Point-in-time measurement that can go up or down (e.g., active connections).

**Aggregations:** avg, min, max, last

### Histogram
Distribution of values (e.g., response times).

**Aggregations:** avg, min, max, p50, p95, p99, count

### Rate
Change over time (e.g., requests per second, error rate).

**Aggregations:** rate, count

---

## Widget Types Reference

### Available Widget Types

| Type | Category | Description |
|------|----------|-------------|
| `funnel` | Conversion | Funnel chart for conversion analysis |
| `heatmap` | Distribution | Heatmap for pattern visualization |
| `gauge` | Indicator | Gauge chart for single metrics |
| `line` | Trend | Line chart for time series |
| `bar` | Comparison | Bar chart for comparisons |
| `pie` | Composition | Pie chart for proportions |
| `area` | Trend | Area chart for cumulative trends |
| `scatter` | Correlation | Scatter plot for correlations |
| `bubble` | Correlation | Bubble chart with 3 dimensions |
| `donut` | Composition | Donut chart (hollow pie) |
| `stacked-bar` | Comparison | Stacked bar chart |
| `grouped-bar` | Comparison | Grouped bar chart |
| `histogram` | Distribution | Histogram for distributions |
| `box-plot` | Distribution | Box plot for statistics |
| `violin-plot` | Distribution | Violin plot for distributions |
| `radial-bar` | Comparison | Radial/circular bar chart |
| `sankey` | Flow | Sankey diagram for flows |
| `treemap` | Hierarchy | Treemap for hierarchies |
| `sunburst` | Hierarchy | Sunburst for hierarchies |
| `network` | Relationship | Network graph |
| `chord` | Relationship | Chord diagram |
| `calendar` | Time | Calendar heatmap |
| `waterfall` | Change | Waterfall chart |
| `bullet` | Indicator | Bullet chart |
| `sparkline` | Trend | Compact sparkline |

---

## Error Responses

### 400 Bad Request
```json
{
  "error": "Invalid request parameters"
}
```

### 404 Not Found
```json
{
  "error": "Provider 'invalid-id' not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "An error occurred while processing the request"
}
```

---

## Rate Limiting

Currently, there are no rate limits. In production, implement rate limiting to prevent abuse.

## CORS

The API allows all origins in development. Configure appropriate CORS policies for production:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Versioning

API versioning is not currently implemented. Consider adding versioning for production use.
