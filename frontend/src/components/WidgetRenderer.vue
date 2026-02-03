<template>
  <component :is="definition.component" v-bind="boundProps" />
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { DataPoint, WidgetConfiguration } from '@/types/dashboard';
import { widgetRegistry } from '@/registry/widgetRegistry';

const props = defineProps<{
  widget: WidgetConfiguration;
  data: Record<string, DataPoint[]>;
}>();

const definition = computed(() => widgetRegistry[props.widget.type]);

const boundProps = computed(() => {
  const points = props.data[props.widget.id] ?? [];
  if (definition.value.mapProps) {
    return definition.value.mapProps(props.widget, points);
  }

  return { widget: props.widget };
});
</script>
