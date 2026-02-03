<template>
  <BaseChart :data="dataPoints" :options="chartOptions" :render="renderGauge" />
</template>

<script setup lang="ts">
import * as d3 from 'd3';
import BaseChart from '@/components/BaseChart.vue';
import type { DataPoint, WidgetConfiguration } from '@/types/dashboard';

const props = defineProps<{
  widget: WidgetConfiguration;
  value: number;
}>();

const dataPoints: DataPoint[] = [{ key: props.widget.label ?? 'value', value: props.value }];
const chartOptions = {
  min: props.widget.min ?? 0,
  max: props.widget.max ?? 100,
  label: props.widget.label ?? '',
};

const renderGauge = ({
  svg,
  width,
  height,
  options,
}: {
  svg: d3.Selection<SVGSVGElement, unknown, null, undefined>;
  width: number;
  height: number;
  options: { min?: number; max?: number; label?: string };
}) => {
  const radius = Math.min(width, height) / 2 - 16;
  const centerX = width / 2;
  const centerY = height / 2;
  const min = options.min ?? 0;
  const max = options.max ?? 100;
  const value = Math.min(Math.max(props.value, min), max);
  const percent = (value - min) / (max - min || 1);

  const arc = d3
    .arc()
    .innerRadius(radius * 0.65)
    .outerRadius(radius)
    .startAngle(-Math.PI / 2);

  svg
    .append('path')
    .attr('d', arc({ endAngle: Math.PI / 2 }) as string)
    .attr('fill', '#e2e8f0')
    .attr('transform', `translate(${centerX}, ${centerY})`);

  svg
    .append('path')
    .attr('d', arc({ endAngle: -Math.PI / 2 + Math.PI * percent }) as string)
    .attr('fill', '#3b82f6')
    .attr('transform', `translate(${centerX}, ${centerY})`);

  svg
    .append('text')
    .attr('x', centerX)
    .attr('y', centerY + 8)
    .attr('text-anchor', 'middle')
    .attr('font-size', '18px')
    .attr('font-weight', '600')
    .text(`${Math.round(value)} ms`);

  if (options.label) {
    svg
      .append('text')
      .attr('x', centerX)
      .attr('y', centerY + 32)
      .attr('text-anchor', 'middle')
      .attr('font-size', '12px')
      .attr('fill', '#475569')
      .text(options.label);
  }
};
</script>
