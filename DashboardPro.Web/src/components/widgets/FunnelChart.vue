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

function renderFunnel() {
  if (!svgRef.value || !props.data.length) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const margin = { top: 20, right: 20, bottom: 30, left: 100 }
  const innerWidth = props.width - margin.left - margin.right
  const innerHeight = props.height - margin.top - margin.bottom

  const g = svg.append('g')
    .attr('transform', `translate(${margin.left},${margin.top})`)

  const maxValue = d3.max(props.data, d => d.value) || 1
  const widthScale = d3.scaleLinear()
    .domain([0, maxValue])
    .range([0, innerWidth])

  const stepHeight = innerHeight / props.data.length

  props.data.forEach((d, i) => {
    const barWidth = widthScale(d.value)
    const x = (innerWidth - barWidth) / 2

    const group = g.append('g')
      .attr('transform', `translate(0, ${i * stepHeight})`)

    // Trapezoid shape
    const points = i < props.data.length - 1
      ? `${x},0 ${x + barWidth},0 ${x + barWidth * 0.9},${stepHeight} ${x + barWidth * 0.1},${stepHeight}`
      : `${x},0 ${x + barWidth},0 ${x + barWidth},${stepHeight} ${x},${stepHeight}`

    group.append('polygon')
      .attr('points', points)
      .attr('fill', props.options.color || '#4CAF50')
      .attr('opacity', 0.7)
      .attr('stroke', '#fff')
      .attr('stroke-width', 2)

    // Label
    group.append('text')
      .attr('x', -10)
      .attr('y', stepHeight / 2)
      .attr('dy', '0.35em')
      .attr('text-anchor', 'end')
      .attr('fill', '#333')
      .text(d.label)

    // Value
    group.append('text')
      .attr('x', innerWidth / 2)
      .attr('y', stepHeight / 2)
      .attr('dy', '0.35em')
      .attr('text-anchor', 'middle')
      .attr('fill', '#fff')
      .attr('font-weight', 'bold')
      .text(d.value)
  })
}

onMounted(() => {
  renderFunnel()
})

watch(() => [props.data, props.width, props.height], () => {
  renderFunnel()
}, { deep: true })
</script>
