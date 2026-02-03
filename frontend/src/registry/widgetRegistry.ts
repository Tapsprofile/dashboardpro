import type { Component } from 'vue';
import GaugeChart from '@/components/widgets/GaugeChart.vue';
import LineChart from '@/components/widgets/LineChart.vue';
import PlaceholderChart from '@/components/widgets/PlaceholderChart.vue';
import type { DataPoint, WidgetConfiguration, WidgetType } from '@/types/dashboard';

export interface WidgetDefinition {
  type: WidgetType;
  component: Component;
  description: string;
  mapProps?: (widget: WidgetConfiguration, points: DataPoint[]) => Record<string, unknown>;
}

export const widgetRegistry: Record<WidgetType, WidgetDefinition> = {
  KPI_CARD: { type: 'KPI_CARD', component: PlaceholderChart, description: 'Headline metric card' },
  GAUGE: {
    type: 'GAUGE',
    component: GaugeChart,
    description: 'Radial gauge',
    mapProps: (widget, points) => ({ widget, value: points[0]?.value ?? 0 }),
  },
  LINE_CHART: {
    type: 'LINE_CHART',
    component: LineChart,
    description: 'Time series line chart',
    mapProps: (widget, points) => ({ widget, points }),
  },
  AREA_CHART: { type: 'AREA_CHART', component: PlaceholderChart, description: 'Area chart' },
  BAR_CHART: { type: 'BAR_CHART', component: PlaceholderChart, description: 'Bar chart' },
  PIE_CHART: { type: 'PIE_CHART', component: PlaceholderChart, description: 'Pie chart' },
  DONUT_CHART: { type: 'DONUT_CHART', component: PlaceholderChart, description: 'Donut chart' },
  SCATTER_PLOT: { type: 'SCATTER_PLOT', component: PlaceholderChart, description: 'Scatter plot' },
  HEATMAP: { type: 'HEATMAP', component: PlaceholderChart, description: 'Heatmap' },
  FUNNEL: { type: 'FUNNEL', component: PlaceholderChart, description: 'Funnel chart' },
  DATA_TABLE: { type: 'DATA_TABLE', component: PlaceholderChart, description: 'Data table' },
  GEO_MAP: { type: 'GEO_MAP', component: PlaceholderChart, description: 'Geographic map' },
  HISTOGRAM: { type: 'HISTOGRAM', component: PlaceholderChart, description: 'Histogram' },
  BOX_PLOT: { type: 'BOX_PLOT', component: PlaceholderChart, description: 'Box plot' },
  RADAR: { type: 'RADAR', component: PlaceholderChart, description: 'Radar chart' },
  TREE_MAP: { type: 'TREE_MAP', component: PlaceholderChart, description: 'Tree map' },
  SANKEY: { type: 'SANKEY', component: PlaceholderChart, description: 'Sankey diagram' },
  SPARKLINE: { type: 'SPARKLINE', component: PlaceholderChart, description: 'Sparkline' },
  BULLET: { type: 'BULLET', component: PlaceholderChart, description: 'Bullet chart' },
  CALENDAR_HEATMAP: {
    type: 'CALENDAR_HEATMAP',
    component: PlaceholderChart,
    description: 'Calendar heatmap',
  },
  STACKED_BAR: { type: 'STACKED_BAR', component: PlaceholderChart, description: 'Stacked bar' },
};
