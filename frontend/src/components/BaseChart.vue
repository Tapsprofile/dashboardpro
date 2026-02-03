<template>
  <div ref="container" class="base-chart"></div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from 'vue';
import * as d3 from 'd3';
import type { DataPoint } from '@/types/dashboard';

export interface ChartOptions {
  min?: number;
  max?: number;
  label?: string;
  unit?: string;
}

export interface RenderContext {
  svg: d3.Selection<SVGSVGElement, unknown, null, undefined>;
  width: number;
  height: number;
  data: DataPoint[];
  options: ChartOptions;
}

const props = defineProps<{
  data: DataPoint[];
  options?: ChartOptions;
  render: (context: RenderContext) => void;
}>();

const container = ref<HTMLDivElement | null>(null);
let svg: d3.Selection<SVGSVGElement, unknown, null, undefined> | null = null;
let resizeObserver: ResizeObserver | null = null;

const renderChart = () => {
  if (!container.value) {
    return;
  }

  const { width, height } = container.value.getBoundingClientRect();
  const resolvedWidth = Math.max(width, 200);
  const resolvedHeight = Math.max(height, 120);

  if (!svg) {
    svg = d3
      .select(container.value)
      .append('svg')
      .attr('preserveAspectRatio', 'xMidYMid meet');
  }

  svg.attr('width', resolvedWidth).attr('height', resolvedHeight);
  svg.selectAll('*').remove();

  props.render({
    svg,
    width: resolvedWidth,
    height: resolvedHeight,
    data: props.data,
    options: props.options ?? {},
  });
};

onMounted(() => {
  renderChart();
  resizeObserver = new ResizeObserver(renderChart);
  if (container.value) {
    resizeObserver.observe(container.value);
  }
});

onBeforeUnmount(() => {
  resizeObserver?.disconnect();
});

watch(
  () => [props.data, props.options, props.render],
  () => renderChart(),
  { deep: true }
);
</script>

<style scoped>
.base-chart {
  width: 100%;
  height: 100%;
  min-height: 220px;
}
</style>
