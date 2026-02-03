import { defineStore } from 'pinia';

export interface DashboardFilter {
  field: string;
  value: string | number;
}

export const useDashboardFilters = defineStore('dashboardFilters', {
  state: () => ({
    activeFilters: [] as DashboardFilter[],
  }),
  actions: {
    setFilter(filter: DashboardFilter) {
      const index = this.activeFilters.findIndex((item) => item.field === filter.field);
      if (index >= 0) {
        this.activeFilters.splice(index, 1, filter);
      } else {
        this.activeFilters.push(filter);
      }
    },
    clearFilter(field: string) {
      this.activeFilters = this.activeFilters.filter((item) => item.field !== field);
    },
    reset() {
      this.activeFilters = [];
    },
  },
});
