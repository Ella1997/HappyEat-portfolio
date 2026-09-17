<script setup>
import { computed, ref, watch, onMounted, onUnmounted } from "vue";
import { calculateBMI, calculateWaistHipRatio } from "@/utils/calculation";

// =========================
// Props / Emits
// =========================
const props = defineProps({
  records: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(["edit", "delete"]);

// =========================
// Month Filter
// =========================
const selectedMonth = ref("all");

const monthOptions = computed(() => {
  const months = [
    ...new Set(
      props.records
        .map((record) => record.recordDate?.slice(0, 7))
        .filter(Boolean),
    ),
  ];

  return months.sort((a, b) => b.localeCompare(a));
});

const formatMonth = (month) => {
  const [year, monthNumber] = month.split("-");

  return `${year} 年 ${Number(monthNumber)} 月`;
};

// =========================
// Filtered Records
// =========================
const filteredRecords = computed(() => {
  if (selectedMonth.value === "all") {
    return props.records;
  }

  return props.records.filter((record) =>
    record.recordDate?.startsWith(selectedMonth.value),
  );
});

// =========================
// Pagination
// =========================
const currentPage = ref(1);
const pageSize = 5;

const totalPages = computed(() => {
  return Math.ceil(filteredRecords.value.length / pageSize);
});

const paginatedRecords = computed(() => {
  const start = (currentPage.value - 1) * pageSize;
  const end = start + pageSize;

  return filteredRecords.value.slice(start, end);
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

watch(selectedMonth, () => {
  currentPage.value = 1;
});

watch(totalPages, (newTotalPages) => {
  if (newTotalPages > 0 && currentPage.value > newTotalPages) {
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
// Action Menu
// =========================
const openedMenuId = ref(null);

const toggleMenu = (recordId) => {
  openedMenuId.value = openedMenuId.value === recordId ? null : recordId;
};

const closeMenu = () => {
  openedMenuId.value = null;
};

const handleEdit = (record) => {
  closeMenu();
  emit("edit", record);
};

const handleDelete = (record) => {
  closeMenu();
  emit("delete", record);
};

const handleDocumentClick = () => {
  closeMenu();
};

onMounted(() => {
  document.addEventListener("click", handleDocumentClick);
});

onUnmounted(() => {
  document.removeEventListener("click", handleDocumentClick);
});
</script>

<template>
  <section class="rounded-3xl border border-border bg-card p-6 shadow-sm">
    <!-- =========================
         Header
    ========================== -->
    <div
      class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between"
    >
      <div>
        <h2 class="text-xl font-semibold text-text-primary">體態紀錄</h2>

        <p class="mt-1 text-sm text-text-secondary">查看你的歷史體態數據</p>
      </div>

      <!-- =========================
           月份篩選
      ========================== -->
      <div v-if="props.records.length > 0">
        <label
          for="body-record-month"
          class="mb-1.5 block text-sm font-medium text-text-secondary"
        >
          紀錄月份
        </label>

        <select
          id="body-record-month"
          v-model="selectedMonth"
          class="min-w-36 rounded-xl border border-border bg-white px-3 py-2 text-sm text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
        >
          <option value="all">全部月份</option>

          <option v-for="month in monthOptions" :key="month" :value="month">
            {{ formatMonth(month) }}
          </option>
        </select>
      </div>
    </div>

    <!-- =========================
         完全沒有紀錄
    ========================== -->
    <div
      v-if="props.records.length === 0"
      class="mt-6 rounded-2xl border border-dashed border-border bg-surface-soft px-6 py-12 text-center"
    >
      <p class="text-sm font-medium text-text-secondary">目前沒有體態紀錄</p>

      <p class="mt-1 text-sm text-text-muted">
        新增第一筆紀錄後即可查看歷史數據
      </p>
    </div>

    <!-- =========================
         有紀錄
    ========================== -->
    <template v-else>
      <!-- 篩選後沒有紀錄 -->
      <div
        v-if="filteredRecords.length === 0"
        class="mt-6 rounded-2xl bg-surface-soft px-6 py-10 text-center"
      >
        <p class="text-sm text-text-secondary">此月份沒有體態紀錄</p>
      </div>

      <template v-else>
        <!-- =========================
             Table
        ========================== -->
        <div class="mt-6 overflow-x-auto">
          <table class="w-full min-w-245 border-collapse">
            <thead>
              <tr class="border-b border-border">
                <!-- 日期 -->
                <th
                  class="px-4 py-3 text-left text-sm font-medium text-text-secondary"
                >
                  日期
                </th>

                <!-- 體重 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  體重
                  <span class="font-normal text-text-muted"> (kg) </span>
                </th>

                <!-- BMI -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  BMI
                </th>

                <!-- 體脂 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  體脂
                  <span class="font-normal text-text-muted"> (%) </span>
                </th>

                <!-- 肌肉量 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  肌肉量
                  <span class="font-normal text-text-muted"> (kg) </span>
                </th>

                <!-- 內臟脂肪 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  內臟脂肪
                </th>

                <!-- 腰圍 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  腰圍
                  <span class="font-normal text-text-muted"> (cm) </span>
                </th>

                <!-- 腰臀比 -->
                <th
                  class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                >
                  腰臀比
                </th>

                <!-- 操作 -->
                <th class="w-14 px-3 py-3">
                  <span class="sr-only"> 操作 </span>
                </th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="record in paginatedRecords"
                :key="record.bodyRecordId"
                class="border-b border-border transition last:border-b-0 hover:bg-surface-soft"
              >
                <!-- 日期 -->
                <td
                  class="whitespace-nowrap px-4 py-4 text-base font-medium text-text-primary"
                >
                  {{ record.recordDate }}
                </td>

                <!-- 體重 -->
                <td
                  class="px-4 py-4 text-center text-base font-semibold text-text-primary"
                >
                  {{ record.weight ?? "-" }}
                </td>

                <!-- BMI -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ calculateBMI(record.weight, record.height) ?? "-" }}
                </td>

                <!-- 體脂 -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ record.bodyFat ?? "-" }}
                </td>

                <!-- 肌肉量 -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ record.muscleMass ?? "-" }}
                </td>

                <!-- 內臟脂肪 -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ record.visceralFat ?? "-" }}
                </td>

                <!-- 腰圍 -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{ record.waistSize ?? "-" }}
                </td>

                <!-- 腰臀比 -->
                <td class="px-4 py-4 text-center text-base text-text-primary">
                  {{
                    calculateWaistHipRatio(record.waistSize, record.hipSize) ??
                    "-"
                  }}
                </td>

                <!-- =========================
                     Action Menu
                ========================== -->
                <td class="relative px-3 py-4 text-right">
                  <button
                    type="button"
                    class="inline-flex h-8 w-8 items-center justify-center rounded-lg text-text-muted transition hover:bg-info-soft hover:text-text-primary"
                    aria-label="開啟操作選單"
                    @click.stop="toggleMenu(record.bodyRecordId)"
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      class="h-5 w-5"
                    >
                      <circle cx="12" cy="5" r="1" />
                      <circle cx="12" cy="12" r="1" />
                      <circle cx="12" cy="19" r="1" />
                    </svg>
                  </button>

                  <!-- Dropdown -->
                  <div
                    v-if="openedMenuId === record.bodyRecordId"
                    class="absolute right-3 top-12 z-20 w-28 rounded-xl border border-border bg-white p-1.5 text-left shadow-lg"
                    @click.stop
                  >
                    <!-- 編輯 -->
                    <button
                      type="button"
                      class="flex w-full items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-text-secondary transition hover:bg-info-soft hover:text-text-primary"
                      @click="handleEdit(record)"
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
                        <path d="M12 20h9" />

                        <path
                          d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4Z"
                        />
                      </svg>

                      編輯
                    </button>

                    <!-- 刪除 -->
                    <button
                      type="button"
                      class="flex w-full items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-danger transition hover:bg-danger/10"
                      @click="handleDelete(record)"
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
                        <path d="M3 6h18" />
                        <path d="M8 6V4h8v2" />
                        <path d="M19 6l-1 14H6L5 6" />
                        <path d="M10 11v5" />
                        <path d="M14 11v5" />
                      </svg>

                      刪除
                    </button>
                  </div>
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
          <!-- 筆數 -->
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

          <!-- 分頁 -->
          <div v-if="totalPages > 1" class="flex items-center gap-1">
            <!-- 上一頁 -->
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

            <!-- 頁碼 -->
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

            <!-- 下一頁 -->
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
