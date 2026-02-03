export type WidgetType =
  | 'KPI_CARD'
  | 'GAUGE'
  | 'LINE_CHART'
  | 'AREA_CHART'
  | 'BAR_CHART'
  | 'PIE_CHART'
  | 'DONUT_CHART'
  | 'SCATTER_PLOT'
  | 'HEATMAP'
  | 'FUNNEL'
  | 'DATA_TABLE'
  | 'GEO_MAP'
  | 'HISTOGRAM'
  | 'BOX_PLOT'
  | 'RADAR'
  | 'TREE_MAP'
  | 'SANKEY'
  | 'SPARKLINE'
  | 'BULLET'
  | 'CALENDAR_HEATMAP'
  | 'STACKED_BAR';

export interface DashboardConfiguration {
  dashboardTitle: string;
  widgets: WidgetConfiguration[];
}

export interface WidgetConfiguration {
  id: string;
  type: WidgetType;
  metric?: string;
  dimension?: string;
  label?: string;
  x?: string;
  y?: string;
  granularity?: string;
  min?: number;
  max?: number;
  steps?: string[];
  columns?: string[];
  sort?: string;
  options?: Record<string, string>;
}

export interface DataPoint {
  key: string;
  value: number;
}

export interface WidgetResult {
  widgetId: string;
  points: DataPoint[];
}
