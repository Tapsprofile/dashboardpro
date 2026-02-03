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

function renderHeatmap() {
  if (!svgRef.value || !props.data.length) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const margin = { top: 30, right: 30, bottom: 30, left: 60 }
  const innerWidth = props.width - margin.left - margin.right
  const innerHeight = props.height - margin.top - margin.bottom

  const g = svg.append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`)

  // Extract unique x and y values
  const xValues = [...new Set(props.data.map(d => d.x))]
  const yValues = [...new Set(props.data.map(d => d.y))]

  const xScale = d3.scaleBand()
    .domain(xValues)
    .range([0, innerWidth])
    .padding(0.05)

  const yScale = d3.scaleBand()
    .domain(yValues)
    .range([0, innerHeight])
    .padding(0.05)

  const colorScale = d3.scaleSequential(d3.interpolateYlOrRd)
    .domain([0, d3.max(props.data, d => d.value) || 1])

  // Draw cells
  g.selectAll('rect')
    .data(props.data)
    .enter()
    .append('rect')
    .attr('x', d => xScale(d.x) || 0)
    .attr('y', d => yScale(d.y) || 0)
    .attr('width', xScale.bandwidth())
    .attr('height', yScale.bandwidth())
    .attr('fill', d => colorScale(d.value))
    .attr('stroke', '#fff')
    .attr('stroke-width', 1)

  // X axis
  g.append('g')
    .attr('transform', `translate(0,${innerHeight})`)
    .call(d3.axisBottom(xScale))
    .selectAll('text')
    .attr('transform', 'rotate(-45)')
    .style('text-anchor', 'end')

  // Y axis
  g.append('g')
    .call(d3.axisLeft(yScale))
}

onMounted(() => {
  renderHeatmap()
})

watch(() => [props.data, props.width, props.height], () => {
  renderHeatmap()
}, { deep: true })
</script>
