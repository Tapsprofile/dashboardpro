<template>
  <svg :width="width" :height="height" ref="svgRef"></svg>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as d3 from 'd3'

const props = defineProps({
  data: { type: Array, default: () => [] },
  width: { type: Number, default: 300 },
  height: { type: Number, default: 300 },
  options: { type: Object, default: () => ({}) }
})

const svgRef = ref(null)

function renderPieChart() {
  if (!svgRef.value || !props.data.length) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const radius = Math.min(props.width, props.height) / 2 - 20

  const g = svg.append('g')
    .attr('transform', `translate(${props.width / 2},${props.height / 2})`)

  const color = d3.scaleOrdinal(d3.schemeCategory10)

  const pie = d3.pie()
    .value(d => d.value)

  const arc = d3.arc()
    .innerRadius(0)
    .outerRadius(radius)

  const arcs = g.selectAll('arc')
    .data(pie(props.data))
    .enter()
    .append('g')

  arcs.append('path')
    .attr('d', arc)
    .attr('fill', (d, i) => color(i))
    .attr('stroke', '#fff')
    .attr('stroke-width', 2)

  arcs.append('text')
    .attr('transform', d => `translate(${arc.centroid(d)})`)
    .attr('text-anchor', 'middle')
    .attr('font-size', '12px')
    .attr('fill', '#fff')
    .text(d => d.data.label)
}

onMounted(() => {
  renderPieChart()
})

watch(() => [props.data, props.width, props.height], () => {
  renderPieChart()
}, { deep: true })
</script>
