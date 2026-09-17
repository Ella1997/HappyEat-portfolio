<script setup>
import { computed, ref, watch } from "vue";

// =========================
// Props / Emits
// =========================
const props = defineProps({
  records: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(["view"]);

// =========================
// Filter
// =========================
const selectedMealType = ref("all");
const selectedDateRange = ref("all");

const mealTypes = ["早餐", "早午餐", "午餐", "點心", "晚餐", "宵夜"];

const filteredRecords = computed(() => {
  let result = [...props.records];

  // 餐別篩選
  if (selectedMealType.value !== "all") {
    result = result.filter(
      (record) => record.mealType === selectedMealType.value,
    );
  }

  // 日期篩選
  if (selectedDateRange.value !== "all") {
    const today = getToday();

    let startDate = null;

    switch (selectedDateRange.value) {
      case "today":
        startDate = today;
        break;

      case "7days":
        startDate = new Date(today);
        startDate.setDate(startDate.getDate() - 6);
        break;

      case "30days":
        startDate = new Date(today);
        startDate.setDate(startDate.getDate() - 29);
        break;
    }

    result = result.filter((record) => {
      const recordDate = parseLocalDate(record.recordDate);

      return recordDate >= startDate && recordDate <= today;
    });
  }

  // 最新紀錄放最前面
  return result.sort(
    (a, b) =>
      parseLocalDate(b.recordDate) - parseLocalDate(a.recordDate) ||
      b.recordId - a.recordId,
  );
});

// =========================
// Pagination
// =========================
const currentPage = ref(1);
const pageSize = 10;

const totalPages = computed(() => {
  return Math.ceil(filteredRecords.value.length / pageSize);
});

const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize;

  return filteredRecords.value.slice(start, start + pageSize);
});

const startItem = computed(() => {
  if (filteredRecords.value.length === 0) {
    return 0;
  }

  return (currentPage.value - 1) * pageSize + 1;
});

const endItem = computed(() => {
  return Math.min(currentPage.value * pageSize, filteredRecords.value.length);
});

// 篩選條件改變時回到第一頁
watch([selectedMealType, selectedDateRange], () => {
  currentPage.value = 1;
});

// 若刪除資料後目前頁數超出範圍
// 自動回到最後一頁
watch(totalPages, (newTotalPages) => {
  if (newTotalPages === 0) {
    currentPage.value = 1;
    return;
  }

  if (currentPage.value > newTotalPages) {
    currentPage.value = newTotalPages;
  }
});

// =========================
// Page Numbers
// =========================
const visiblePages = computed(() => {
  const total = totalPages.value;
  const current = currentPage.value;

  if (total <= 5) {
    return Array.from({ length: total }, (_, index) => index + 1);
  }

  let start = Math.max(1, current - 2);
  let end = Math.min(total, start + 4);

  if (end - start < 4) {
    start = Math.max(1, end - 4);
  }

  return Array.from({ length: end - start + 1 }, (_, index) => start + index);
});

const goToPage = (page) => {
  if (page < 1 || page > totalPages.value) {
    return;
  }

  currentPage.value = page;
};

// =========================
// Record
// =========================
const handleView = (record) => {
  emit("view", record);
};

const getRecordTitle = (record) => {
  const items = record.items ?? [];

  if (items.length === 0) {
    return record.description || "未命名餐點";
  }

  if (items.length === 1) {
    return items[0].itemName || "未命名餐點";
  }

  const firstItemName = items[0].itemName || "餐點";

  return `${firstItemName} 等 ${items.length} 項`;
};

// =========================
// Date
// =========================
function parseLocalDate(dateString) {
  if (!dateString) {
    return new Date(0);
  }

  const [year, month, day] = dateString.split("-").map(Number);

  return new Date(year, month - 1, day);
}

function getToday() {
  const today = new Date();

  return new Date(today.getFullYear(), today.getMonth(), today.getDate());
}

const formatDate = (dateString) => {
  if (!dateString) return "-";

  const [datePart] = String(dateString).split("T");
  const [, month, day] = datePart.split("-");

  return `${Number(month)}/${Number(day)}`;
};

// =========================
// Nutrition Format
// =========================
const formatNumber = (value) => {
  if (value == null) {
    return "-";
  }

  const number = Number(value);

  return Number.isInteger(number)
    ? number.toLocaleString()
    : number.toLocaleString(undefined, {
        maximumFractionDigits: 1,
      });
};

// =========================
// Meal Type Style
// =========================
const getMealTypeClass = (mealType) => {
  switch (mealType) {
    case "早餐":
      return "bg-primary-soft text-text-primary";

    case "早午餐":
      return "bg-info-soft text-text-primary";

    case "午餐":
      return "bg-info-strong text-text-primary";

    case "點心":
      return "bg-accent/20 text-text-primary";

    case "晚餐":
      return "bg-accent/35 text-text-primary";

    case "宵夜":
      return "bg-border text-text-secondary";

    default:
      return "bg-surface-soft text-text-secondary";
  }
};
</script>

<template>
  <section class="rounded-3xl border border-border bg-card p-6 shadow-sm">
    <!-- =========================
     Header
    ========================== -->
    <div
      class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between"
    >
      <!-- Title -->
      <h2 class="text-xl font-semibold text-text-primary">飲食紀錄</h2>

      <!-- Filters -->
      <div
        v-if="props.records.length > 0"
        class="flex flex-wrap items-center gap-3"
      >
        <!-- Meal Type -->
        <select
          id="meal-type-filter"
          v-model="selectedMealType"
          aria-label="篩選餐別"
          class="min-w-32 rounded-xl border border-border bg-white px-3 py-2 text-sm text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
        >
          <option value="all">全部餐別</option>

          <option
            v-for="mealType in mealTypes"
            :key="mealType"
            :value="mealType"
          >
            {{ mealType }}
          </option>
        </select>

        <!-- Date Range -->
        <select
          id="date-range-filter"
          v-model="selectedDateRange"
          aria-label="篩選時間"
          class="min-w-36 rounded-xl border border-border bg-white px-3 py-2 text-sm text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
        >
          <option value="all">全部時間</option>

          <option value="today">今天</option>

          <option value="7days">最近 7 天</option>

          <option value="30days">最近 30 天</option>
        </select>
      </div>
    </div>

    <!-- =========================
         Empty State
    ========================== -->
    <div
      v-if="props.records.length === 0"
      class="mt-6 rounded-2xl border border-dashed border-border bg-surface-soft px-6 py-12 text-center"
    >
      <p class="text-sm font-medium text-text-secondary">目前沒有飲食紀錄</p>

      <p class="mt-1 text-sm text-text-muted">
        使用 AI 辨識或手動新增第一筆飲食紀錄
      </p>
    </div>

    <!-- =========================
         Records
    ========================== -->
    <template v-else>
      <!-- Filter Empty -->
      <div
        v-if="filteredRecords.length === 0"
        class="mt-6 rounded-2xl bg-surface-soft px-6 py-10 text-center"
      >
        <p class="text-sm text-text-secondary">找不到符合條件的飲食紀錄</p>
      </div>

      <template v-else>
        <!-- =========================
             Table
        ========================== -->
        <div class="mt-6 overflow-x-auto">
          <table class="w-full min-w-220 border-collapse">
            <!-- Table Header -->
            <thead>
              <tr class="border-b border-border">
                <th
                  class="px-4 py-3 text-left text-sm font-medium text-text-secondary"
                >
                  餐點
                </th>

                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  餐別
                </th>

                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  熱量
                  <span class="font-normal text-text-muted"> (kcal) </span>
                </th>

                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  碳水
                  <span class="font-normal text-text-muted"> (g) </span>
                </th>

                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  蛋白質
                  <span class="font-normal text-text-muted"> (g) </span>
                </th>

                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  脂肪
                  <span class="font-normal text-text-muted"> (g) </span>
                </th>
              </tr>
            </thead>

            <!-- Table Body -->
            <tbody>
              <tr
                v-for="record in paginatedRecords"
                :key="record.recordId"
                class="cursor-pointer border-b border-border transition last:border-b-0 hover:bg-surface-soft"
                tabindex="0"
                @click="handleView(record)"
                @keydown.enter="handleView(record)"
              >
                <!-- =========================
                     Meal
                ========================== -->
                <td class="px-4 py-4">
                  <div class="flex min-w-55 items-center gap-4">
                    <!-- Image -->
                    <div
                      class="flex h-14 w-14 shrink-0 items-center justify-center overflow-hidden rounded-xl bg-info-soft"
                    >
                      <img
                        v-if="record.imagePath"
                        :src="record.imagePath"
                        :alt="record.description || '餐點照片'"
                        class="h-full w-full object-cover"
                      />

                      <!-- No Image -->
                      <svg
                        v-else
                        xmlns="http://www.w3.org/2000/svg"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="1.8"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="h-6 w-6 text-text-muted"
                      >
                        <rect width="18" height="18" x="3" y="3" rx="2" />

                        <circle cx="9" cy="9" r="2" />

                        <path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21" />
                      </svg>
                    </div>

                    <!-- Meal Information -->
                    <div class="min-w-0">
                      <p
                        class="max-w-55 truncate text-base font-medium text-text-primary"
                      >
                        {{ getRecordTitle(record) || "未命名餐點" }}
                      </p>

                      <p class="mt-1 text-sm text-text-muted">
                        {{ formatDate(record.recordDate) }}
                      </p>
                    </div>
                  </div>
                </td>

                <!-- =========================
                     Meal Type
                ========================== -->
                <td class="px-4 py-4 text-center">
                  <span
                    class="inline-flex rounded-lg px-3 py-1.5 text-sm font-medium"
                    :class="getMealTypeClass(record.mealType)"
                  >
                    {{ record.mealType || "-" }}
                  </span>
                </td>

                <!-- =========================
                     Calories
                ========================== -->
                <td
                  class="px-4 py-4 text-center text-base font-semibold text-text-primary"
                >
                  {{ formatNumber(record.totalCalories) }}
                </td>

                <!-- =========================
                     Carbs
                ========================== -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ formatNumber(record.totalCarbs) }}
                </td>

                <!-- =========================
                     Protein
                ========================== -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ formatNumber(record.totalProtein) }}
                </td>

                <!-- =========================
                     Fat
                ========================== -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ formatNumber(record.totalFat) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- =========================
             Pagination
        ========================== -->
        <div
          class="mt-5 flex flex-col gap-4 border-t border-border pt-5 sm:flex-row sm:items-center sm:justify-between"
        >
          <!-- Record Count -->
          <p class="text-sm text-text-secondary">
            顯示
            <span class="font-medium text-text-primary">
              {{ startItem }}–{{ endItem }}
            </span>
            筆，共
            <span class="font-medium text-text-primary">
              {{ filteredRecords.length }}
            </span>
            筆紀錄
          </p>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="flex items-center gap-1">
            <!-- Previous -->
            <button
              type="button"
              class="flex h-9 w-9 items-center justify-center rounded-lg border border-border bg-white text-text-secondary transition hover:bg-info-soft hover:text-text-primary disabled:cursor-not-allowed disabled:opacity-40"
              :disabled="currentPage === 1"
              aria-label="上一頁"
              @click="goToPage(currentPage - 1)"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="h-4 w-4"
              >
                <path d="m15 18-6-6 6-6" />
              </svg>
            </button>

            <!-- Page Numbers -->
            <button
              v-for="page in visiblePages"
              :key="page"
              type="button"
              class="h-9 min-w-9 rounded-lg px-2 text-sm font-medium transition"
              :class="
                currentPage === page
                  ? 'bg-primary text-white'
                  : 'border border-border bg-white text-text-secondary hover:bg-info-soft hover:text-text-primary'
              "
              @click="goToPage(page)"
            >
              {{ page }}
            </button>

            <!-- Next -->
            <button
              type="button"
              class="flex h-9 w-9 items-center justify-center rounded-lg border border-border bg-white text-text-secondary transition hover:bg-info-soft hover:text-text-primary disabled:cursor-not-allowed disabled:opacity-40"
              :disabled="currentPage === totalPages"
              aria-label="下一頁"
              @click="goToPage(currentPage + 1)"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="h-4 w-4"
              >
                <path d="m9 18 6-6-6-6" />
              </svg>
            </button>
          </div>
        </div>
      </template>
    </template>
  </section>
</template>
