<template>
  <svg :width="width" :height="height" ref="svgRef"></svg>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as d3 from 'd3'

const props = defineProps({
  data: { type: Array, default: () => [] },
  width: { type: Number, default: 400 },
  height: { type: Number, default: 300 },
  options: { type: Object, default: () => ({}) }
})

const svgRef = ref(null)

function renderLineChart() {
  if (!svgRef.value || !props.data.length) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const margin = { top: 20, right: 30, bottom: 30, left: 50 }
  const innerWidth = props.width - margin.left - margin.right
  const innerHeight = props.height - margin.top - margin.bottom

  const g = svg.append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`)

  const xScale = d3.scaleTime()
    .domain(d3.extent(props.data, d => new Date(d.timestamp)))
    .range([0, innerWidth])

  const yScale = d3.scaleLinear()
    .domain([0, d3.max(props.data, d => d.value) || 1])
    .range([innerHeight, 0])

  const line = d3.line()
    .x(d => xScale(new Date(d.timestamp)))
    .y(d => yScale(d.value))
    .curve(d3.curveMonotoneX)

  // Draw line
  g.append('path')
    .datum(props.data)
    .attr('fill', 'none')
    .attr('stroke', props.options.color || '#2196F3')
    .attr('stroke-width', 2)
    .attr('d', line)

  // X axis
  g.append('g')
    .attr('transform', `translate(0,${innerHeight})`)
    .call(d3.axisBottom(xScale))

  // Y axis
  g.append('g')
    .call(d3.axisLeft(yScale))
}

onMounted(() => {
  renderLineChart()
})

watch(() => [props.data, props.width, props.height], () => {
  renderLineChart()
}, { deep: true })
</script>
