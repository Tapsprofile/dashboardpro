<template>
  <BaseChart :data="points" :render="renderLine" />
</template>

<script setup lang="ts">
import * as d3 from 'd3';
import BaseChart from '@/components/BaseChart.vue';
import type { DataPoint, WidgetConfiguration } from '@/types/dashboard';

const props = defineProps<{
  widget: WidgetConfiguration;
  points: DataPoint[];
}>();

const renderLine = ({
  svg,
  width,
  height,
  data,
}: {
  svg: d3.Selection<SVGSVGElement, unknown, null, undefined>;
  width: number;
  height: number;
  data: DataPoint[];
}) => {
  const margin = { top: 20, right: 16, bottom: 32, left: 40 };
  const innerWidth = width - margin.left - margin.right;
  const innerHeight = height - margin.top - margin.bottom;

  const x = d3
    .scalePoint()
    .domain(data.map((point) => point.key))
    .range([0, innerWidth]);

  const y = d3
    .scaleLinear()
    .domain([0, d3.max(data, (point) => point.value) ?? 0])
    .nice()
    .range([innerHeight, 0]);

  const chart = svg
    .append('g')
    .attr('transform', `translate(${margin.left}, ${margin.top})`);

  const line = d3
    .line<DataPoint>()
    .x((point) => x(point.key) ?? 0)
    .y((point) => y(point.value))
    .curve(d3.curveMonotoneX);

  chart
    .append('path')
    .datum(data)
    .attr('fill', 'none')
    .attr('stroke', '#3b82f6')
    .attr('stroke-width', 2)
    .attr('d', line);

  chart.append('g').attr('transform', `translate(0, ${innerHeight})`).call(d3.axisBottom(x));
  chart.append('g').call(d3.axisLeft(y).ticks(4));

  if (props.widget.label) {
    svg
      .append('text')
      .attr('x', margin.left)
      .attr('y', 16)
      .attr('font-size', '12px')
      .attr('fill', '#64748b')
      .text(props.widget.label);
  }
};
</script>
