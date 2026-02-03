<template>
  <section class="builder">
    <header>
      <h2>Dashboard Builder</h2>
      <p>Select a metric and dimension to preview a widget.</p>
    </header>

    <div class="controls">
      <label>
        Widget Type
        <select v-model="selectedType">
          <option v-for="type in widgetTypes" :key="type" :value="type">
            {{ type }}
          </option>
        </select>
      </label>
      <label>
        Metric
        <select v-model="selectedMetric">
          <option v-for="metric in availableMetrics" :key="metric" :value="metric">
            {{ metric }}
          </option>
        </select>
      </label>
      <label>
        Dimension
        <select v-model="selectedDimension">
          <option v-for="dimension in availableDimensions" :key="dimension" :value="dimension">
            {{ dimension }}
          </option>
        </select>
      </label>
      <button type="button" @click="applyFilter">Apply Filter</button>
    </div>

    <div class="preview">
      <WidgetRenderer :widget="previewWidget" :data="previewData" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import WidgetRenderer from '@/components/WidgetRenderer.vue';
import { widgetRegistry } from '@/registry/widgetRegistry';
import type { DataPoint, WidgetConfiguration, WidgetType } from '@/types/dashboard';
import { useDashboardFilters } from '@/stores/dashboardFilters';

const filters = useDashboardFilters();

const widgetTypes = Object.keys(widgetRegistry) as WidgetType[];
const availableMetrics = [
  'count(*)',
  'avg(time_taken)',
  'count(sc_status=500)',
  'count(sc_status=404)',
];
const availableDimensions = ['hour', 'sc_status', 'cs_uri_stem', 'c_ip'];

const selectedType = ref<WidgetType>('LINE_CHART');
const selectedMetric = ref('avg(time_taken)');
const selectedDimension = ref('hour');

const previewWidget = computed<WidgetConfiguration>(() => ({
  id: 'preview',
  type: selectedType.value,
  metric: selectedMetric.value,
  dimension: selectedDimension.value,
  label: 'Preview',
  min: 0,
  max: 1000,
}));

const previewData = computed<Record<string, DataPoint[]>>(() => ({
  preview: [
    { key: '00:00', value: 220 },
    { key: '06:00', value: 340 },
    { key: '12:00', value: 410 },
    { key: '18:00', value: 360 },
  ],
}));

const applyFilter = () => {
  filters.setFilter({ field: selectedDimension.value, value: 'preview' });
};
</script>

<style scoped>
.builder {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.controls {
  display: grid;
  gap: 16px;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  align-items: end;
}

.controls label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 0.85rem;
  color: #475569;
}

.preview {
  height: 320px;
  padding: 16px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
}
</style>
