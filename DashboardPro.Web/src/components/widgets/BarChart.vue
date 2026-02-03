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

function renderBarChart() {
  if (!svgRef.value || !props.data.length) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const margin = { top: 20, right: 20, bottom: 50, left: 50 }
  const innerWidth = props.width - margin.left - margin.right
  const innerHeight = props.height - margin.top - margin.bottom

  const g = svg.append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`)

  const xScale = d3.scaleBand()
    .domain(props.data.map(d => d.label))
    .range([0, innerWidth])
    .padding(0.1)

  const yScale = d3.scaleLinear()
    .domain([0, d3.max(props.data, d => d.value) || 1])
    .range([innerHeight, 0])

  g.selectAll('rect')
    .data(props.data)
    .enter()
    .append('rect')
    .attr('x', d => xScale(d.label) || 0)
    .attr('y', d => yScale(d.value))
    .attr('width', xScale.bandwidth())
    .attr('height', d => innerHeight - yScale(d.value))
    .attr('fill', props.options.color || '#2196F3')

  g.append('g')
    .attr('transform', `translate(0,${innerHeight})`)
    .call(d3.axisBottom(xScale))
    .selectAll('text')
    .attr('transform', 'rotate(-45)')
    .style('text-anchor', 'end')

  g.append('g')
    .call(d3.axisLeft(yScale))
}

onMounted(() => {
  renderBarChart()
})

watch(() => [props.data, props.width, props.height], () => {
  renderBarChart()
}, { deep: true })
</script>
