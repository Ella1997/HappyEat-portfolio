<script setup>
import { computed, ref } from "vue";
import { Line } from "vue-chartjs";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Filler,
} from "chart.js";

// =========================
// Chart.js Register
// =========================
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Filler,
);

// =========================
// Props
// =========================
const props = defineProps({
  records: {
    type: Array,
    default: () => [],
  },
});

// =========================
// Metric
// =========================
const selectedMetric = ref("weight");

const metrics = [
  {
    key: "weight",
    label: "體重",
    unit: "kg",
  },
  {
    key: "bodyFat",
    label: "體脂",
    unit: "%",
  },
  {
    key: "muscleMass",
    label: "肌肉量",
    unit: "kg",
  },
];

const currentMetric = computed(() => {
  return metrics.find((metric) => metric.key === selectedMetric.value);
});

// =========================
// Weekly Data
// 最近 28 天 → 依週分組 → 計算平均
// =========================
const weeklyData = computed(() => {
  const metric = selectedMetric.value;

  // 今天
  const today = new Date();
  today.setHours(23, 59, 59, 999);

  // 往前抓 28 天
  const startDate = new Date(today);
  startDate.setDate(startDate.getDate() - 27);
  startDate.setHours(0, 0, 0, 0);

  // 篩選最近 28 天且有該指標的資料
  const recentRecords = props.records.filter((record) => {
    if (record[metric] == null) {
      return false;
    }

    const recordDate = parseLocalDate(record.recordDate);

    return recordDate >= startDate && recordDate <= today;
  });

  // 依「星期一」作為每週起始日分組
  const groups = {};

  recentRecords.forEach((record) => {
    const recordDate = parseLocalDate(record.recordDate);

    // JavaScript:
    // Sunday = 0
    // Monday = 1
    // ...
    // Saturday = 6
    const day = recordDate.getDay();

    // 算出距離該週星期一幾天
    const diffToMonday = day === 0 ? -6 : 1 - day;

    const monday = new Date(recordDate);

    monday.setDate(recordDate.getDate() + diffToMonday);

    const weekKey = formatDateKey(monday);

    if (!groups[weekKey]) {
      groups[weekKey] = [];
    }

    groups[weekKey].push(Number(record[metric]));
  });

  // 計算每週平均
  return Object.entries(groups)
    .map(([weekStart, values]) => {
      const average =
        values.reduce((sum, value) => sum + value, 0) / values.length;

      return {
        weekStart,
        value: Number(average.toFixed(1)),
        count: values.length,
      };
    })
    .sort((a, b) => parseLocalDate(a.weekStart) - parseLocalDate(b.weekStart));
});

// =========================
// Chart State
// =========================
const hasChartData = computed(() => {
  return weeklyData.value.length > 0;
});

// =========================
// Chart Data
// =========================
const chartData = computed(() => ({
  labels: weeklyData.value.map((item) => formatDisplayDate(item.weekStart)),

  datasets: [
    {
      data: weeklyData.value.map((item) => item.value),

      // 暫時使用 Design System 對應色
      borderColor: "#b7d84b",
      backgroundColor: "rgba(183, 216, 75, 0.12)",

      borderWidth: 2.5,

      pointRadius: 4,
      pointHoverRadius: 6,

      pointBackgroundColor: "#ffffff",
      pointBorderColor: "#8eaf2f",
      pointBorderWidth: 2,

      tension: 0.35,
      fill: true,
    },
  ],
}));

// =========================
// Chart Options
// =========================
const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,

  interaction: {
    intersect: false,
    mode: "index",
  },

  plugins: {
    legend: {
      display: false,
    },

    tooltip: {
      displayColors: false,

      callbacks: {
        title(context) {
          if (!context.length) {
            return "";
          }

          return `週起始 ${context[0].label}`;
        },

        label(context) {
          return `週平均 ${context.parsed.y} ${currentMetric.value.unit}`;
        },

        afterLabel(context) {
          const item = weeklyData.value[context.dataIndex];

          return `本週 ${item.count} 筆紀錄`;
        },
      },
    },
  },

  scales: {
    x: {
      grid: {
        display: false,
      },

      border: {
        display: false,
      },

      ticks: {
        color: "#94a3b8",
        maxRotation: 0,
        autoSkip: false,
      },
    },

    y: {
      beginAtZero: false,

      border: {
        display: false,
      },

      grid: {
        color: "rgba(148, 163, 184, 0.15)",
      },

      ticks: {
        color: "#94a3b8",
      },
    },
  },
}));

// =========================
// Date Helpers
// =========================

// 將 YYYY-MM-DD 轉成 local Date
// 避免直接 new Date("YYYY-MM-DD") 的 UTC 問題
function parseLocalDate(dateString) {
  const [year, month, day] = dateString.split("-").map(Number);

  return new Date(year, month - 1, day);
}

// Date → YYYY-MM-DD
function formatDateKey(date) {
  const year = date.getFullYear();

  const month = String(date.getMonth() + 1).padStart(2, "0");

  const day = String(date.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

// YYYY-MM-DD → M/D
function formatDisplayDate(dateString) {
  const [, month, day] = dateString.split("-");

  return `${Number(month)}/${Number(day)}`;
}
</script>

<template>
  <div class="flex h-full min-h-0 flex-col">
    <!-- =========================
         Header
    ========================== -->
    <div
      class="flex shrink-0 flex-col gap-4 sm:flex-row sm:items-center sm:justify-between"
    >
      <div>
        <h2 class="text-xl font-semibold text-text-primary">體態趨勢</h2>

        <p class="mt-1 text-sm text-text-secondary">近一個月・每週平均</p>
      </div>

      <!-- =========================
           Metric Tabs
      ========================== -->
      <div class="flex rounded-xl bg-surface-soft p-1">
        <button
          v-for="metric in metrics"
          :key="metric.key"
          type="button"
          class="rounded-lg px-4 py-2 text-sm transition"
          :class="
            selectedMetric === metric.key
              ? 'bg-white font-semibold text-text-primary shadow-sm'
              : 'font-medium text-text-secondary hover:text-text-primary'
          "
          @click="selectedMetric = metric.key"
        >
          {{ metric.label }}
        </button>
      </div>
    </div>

    <!-- =========================
         Chart
    ========================== -->
    <div v-if="hasChartData" class="mt-5 min-h-0 flex-1">
      <Line :data="chartData" :options="chartOptions" />
    </div>

    <!-- =========================
         Empty State
    ========================== -->
    <div
      v-else
      class="mt-5 flex min-h-0 flex-1 items-center justify-center rounded-2xl bg-surface-soft"
    >
      <div class="text-center">
        <p class="text-sm font-medium text-text-secondary">
          最近 4 週尚無{{ currentMetric.label }}資料
        </p>

        <p class="mt-1 text-xs text-text-muted">新增體態紀錄後即可查看趨勢</p>
      </div>
    </div>
  </div>
</template>
