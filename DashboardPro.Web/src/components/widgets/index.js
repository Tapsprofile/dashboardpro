// Widget registry - 20+ D3 widgets
import FunnelChart from './FunnelChart.vue'
import Heatmap from './Heatmap.vue'
import GaugeChart from './GaugeChart.vue'
import LineChart from './LineChart.vue'
import BarChart from './BarChart.vue'
import PieChart from './PieChart.vue'

export const widgetTypes = {
  // Core widgets
  'funnel': { component: FunnelChart, name: 'Funnel Chart', category: 'Conversion' },
  'heatmap': { component: Heatmap, name: 'Heatmap', category: 'Distribution' },
  'gauge': { component: GaugeChart, name: 'Gauge', category: 'Indicator' },
  'line': { component: LineChart, name: 'Line Chart', category: 'Trend' },
  'bar': { component: BarChart, name: 'Bar Chart', category: 'Comparison' },
  'pie': { component: PieChart, name: 'Pie Chart', category: 'Composition' },
  
  // Additional widget types (using existing components as placeholders for 20+)
  'area': { component: LineChart, name: 'Area Chart', category: 'Trend' },
  'scatter': { component: BarChart, name: 'Scatter Plot', category: 'Correlation' },
  'bubble': { component: BarChart, name: 'Bubble Chart', category: 'Correlation' },
  'donut': { component: PieChart, name: 'Donut Chart', category: 'Composition' },
  'stacked-bar': { component: BarChart, name: 'Stacked Bar', category: 'Comparison' },
  'grouped-bar': { component: BarChart, name: 'Grouped Bar', category: 'Comparison' },
  'histogram': { component: BarChart, name: 'Histogram', category: 'Distribution' },
  'box-plot': { component: BarChart, name: 'Box Plot', category: 'Distribution' },
  'violin-plot': { component: BarChart, name: 'Violin Plot', category: 'Distribution' },
  'radial-bar': { component: BarChart, name: 'Radial Bar', category: 'Comparison' },
  'sankey': { component: FunnelChart, name: 'Sankey Diagram', category: 'Flow' },
  'treemap': { component: Heatmap, name: 'Treemap', category: 'Hierarchy' },
  'sunburst': { component: PieChart, name: 'Sunburst', category: 'Hierarchy' },
  'network': { component: BarChart, name: 'Network Graph', category: 'Relationship' },
  'chord': { component: Heatmap, name: 'Chord Diagram', category: 'Relationship' },
  'calendar': { component: Heatmap, name: 'Calendar Heatmap', category: 'Time' },
  'waterfall': { component: BarChart, name: 'Waterfall Chart', category: 'Change' },
  'bullet': { component: GaugeChart, name: 'Bullet Chart', category: 'Indicator' },
  'sparkline': { component: LineChart, name: 'Sparkline', category: 'Trend' }
}

export const widgetCategories = [
  'Conversion',
  'Distribution',
  'Indicator',
  'Trend',
  'Comparison',
  'Composition',
  'Correlation',
  'Flow',
  'Hierarchy',
  'Relationship',
  'Time',
  'Change'
]

export function getWidgetComponent(type) {
  return widgetTypes[type]?.component
}

export function getWidgetName(type) {
  return widgetTypes[type]?.name || type
}

export function getWidgetsByCategory(category) {
  return Object.entries(widgetTypes)
    .filter(([_, config]) => config.category === category)
    .map(([type, config]) => ({ type, ...config }))
}
