<template>
  <BaseChart :data="dataPoints" :render="renderPlaceholder" />
</template>

<script setup lang="ts">
import * as d3 from 'd3';
import BaseChart from '@/components/BaseChart.vue';
import type { DataPoint, WidgetConfiguration } from '@/types/dashboard';

const props = defineProps<{
  widget: WidgetConfiguration;
}>();

const dataPoints: DataPoint[] = [];

const renderPlaceholder = ({
  svg,
  width,
  height,
}: {
  svg: d3.Selection<SVGSVGElement, unknown, null, undefined>;
  width: number;
  height: number;
}) => {
  svg
    .append('rect')
    .attr('x', 8)
    .attr('y', 8)
    .attr('width', width - 16)
    .attr('height', height - 16)
    .attr('fill', '#f8fafc')
    .attr('stroke', '#e2e8f0')
    .attr('stroke-dasharray', '4 4');

  svg
    .append('text')
    .attr('x', width / 2)
    .attr('y', height / 2 - 6)
    .attr('text-anchor', 'middle')
    .attr('font-size', '14px')
    .attr('fill', '#64748b')
    .text(props.widget.type);

  svg
    .append('text')
    .attr('x', width / 2)
    .attr('y', height / 2 + 16)
    .attr('text-anchor', 'middle')
    .attr('font-size', '12px')
    .attr('fill', '#94a3b8')
    .text('Renderer pending');
};
</script>
