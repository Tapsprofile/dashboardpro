<template>
  <svg :width="width" :height="height" ref="svgRef"></svg>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import * as d3 from 'd3'

const props = defineProps({
  value: { type: Number, default: 0 },
  min: { type: Number, default: 0 },
  max: { type: Number, default: 100 },
  width: { type: Number, default: 200 },
  height: { type: Number, default: 200 },
  options: { type: Object, default: () => ({}) }
})

const svgRef = ref(null)

function renderGauge() {
  if (!svgRef.value) return

  const svg = d3.select(svgRef.value)
  svg.selectAll('*').remove()

  const radius = Math.min(props.width, props.height) / 2 - 10
  const angleRange = Math.PI * 1.5
  const startAngle = -Math.PI * 0.75

  const g = svg.append('g')
    .attr('transform', `translate(${props.width / 2},${props.height / 2})`)

  // Background arc
  const backgroundArc = d3.arc()
    .innerRadius(radius * 0.7)
    .outerRadius(radius)
    .startAngle(startAngle)
    .endAngle(startAngle + angleRange)

  g.append('path')
    .attr('d', backgroundArc)
    .attr('fill', '#e0e0e0')

  // Value arc
  const valueAngle = startAngle + (props.value - props.min) / (props.max - props.min) * angleRange
  const valueArc = d3.arc()
    .innerRadius(radius * 0.7)
    .outerRadius(radius)
    .startAngle(startAngle)
    .endAngle(valueAngle)

  g.append('path')
    .attr('d', valueArc)
    .attr('fill', props.options.color || '#4CAF50')

  // Center text
  g.append('text')
    .attr('text-anchor', 'middle')
    .attr('dy', '0.35em')
    .attr('font-size', '24px')
    .attr('font-weight', 'bold')
    .text(Math.round(props.value))

  // Min/Max labels
  g.append('text')
    .attr('x', -radius * 0.8)
    .attr('y', radius * 0.3)
    .attr('text-anchor', 'start')
    .attr('font-size', '12px')
    .text(props.min)

  g.append('text')
    .attr('x', radius * 0.8)
    .attr('y', radius * 0.3)
    .attr('text-anchor', 'end')
    .attr('font-size', '12px')
    .text(props.max)
}

onMounted(() => {
  renderGauge()
})

watch(() => [props.value, props.min, props.max, props.width, props.height], () => {
  renderGauge()
}, { deep: true })
</script>
