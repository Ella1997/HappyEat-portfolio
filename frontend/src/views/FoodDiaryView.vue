<script setup>
import { computed, onMounted, ref } from "vue";

import NutritionSummary from "@/components/food/NutritionSummary.vue";
import FoodRecordList from "@/components/food/FoodRecordList.vue";
import FoodRecordDetailModal from "@/components/food/FoodRecordDetailModal.vue";

import { useFoodRecord } from "@/composables/useFoodRecord";

// =========================
// User
// 目前先使用測試 UserID = 2
// 之後再接 userStore
// =========================
const userId = 2;

// =========================
// Food Record
// =========================
const { records, loading, error, getRecords, deleteRecord } = useFoodRecord();

// =========================
// Modal State
// =========================
const selectedRecord = ref(null);

// =========================
// Recommended Calories
// 之後直接接 Body Management 的 calculation 結果
// =========================
const recommendedCalories = ref(1850);

// =========================
// Display Records
// 將後端 FoodRecord 轉成畫面需要的資料
// =========================
const displayRecords = computed(() => {
  return records.value.map((record) => {
    const items = record.items ?? [];

    return {
      ...record,

      totalCalories: sumNutrition(items, "calories"),

      totalCarbs: sumNutrition(items, "carbs"),

      totalProtein: sumNutrition(items, "protein"),

      totalFat: sumNutrition(items, "fat"),

      // 目前測試資料沒有 ImageID
      // 之後接 FoodImage API
      imagePath: null,
    };
  });
});

// =========================
// Today Summary
// =========================
const todayRecords = computed(() => {
  const today = getLocalDateKey();

  return displayRecords.value.filter(
    (record) => getDateKey(record.recordDate) === today,
  );
});

const todayNutrition = computed(() => {
  return todayRecords.value.reduce(
    (total, record) => {
      total.calories += Number(record.totalCalories ?? 0);

      total.carbs += Number(record.totalCarbs ?? 0);

      total.protein += Number(record.totalProtein ?? 0);

      total.fat += Number(record.totalFat ?? 0);

      return total;
    },
    {
      calories: 0,
      carbs: 0,
      protein: 0,
      fat: 0,
    },
  );
});

// =========================
// Nutrition Helper
// =========================
const sumNutrition = (items, property) => {
  return items.reduce((total, item) => total + Number(item[property] ?? 0), 0);
};

// =========================
// Date Helpers
// =========================
const getLocalDateKey = () => {
  const today = new Date();

  const year = today.getFullYear();

  const month = String(today.getMonth() + 1).padStart(2, "0");

  const day = String(today.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
};

const getDateKey = (dateValue) => {
  if (!dateValue) {
    return "";
  }

  return String(dateValue).slice(0, 10);
};

// =========================
// Actions
// =========================
const handleAiCreate = () => {
  console.log("open AI recognition");
};

const handleManualCreate = () => {
  console.log("open manual create");
};

const handleViewRecord = (record) => {
  selectedRecord.value = record;
};

const closeDetailModal = () => {
  selectedRecord.value = null;
};

const handleEditRecord = (record) => {
  console.log("edit", record);
};

const handleDeleteRecord = async (record) => {
  console.log("delete", record);
};

// =========================
// Init
// =========================
onMounted(async () => {
  try {
    await getRecords(userId);
  } catch (err) {
    console.error("初始化飲食紀錄失敗：", err);
  }
});
</script>

<template>
  <main class="min-h-screen bg-page px-6 py-8 lg:px-10">
    <div class="mx-auto max-w-7xl">
      <!-- =========================
           Page Header
      ========================== -->
      <div
        class="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between"
      >
        <div>
          <h1 class="text-3xl font-bold text-text-primary">飲食日誌</h1>

          <p class="mt-2 text-sm text-text-secondary">
            記錄每日飲食，掌握熱量與營養攝取
          </p>
        </div>

        <div class="flex items-center gap-3">
          <!-- AI 辨識 -->
          <button
            type="button"
            class="inline-flex items-center gap-2 rounded-xl bg-primary px-5 py-3 text-sm font-semibold text-text-primary shadow-sm transition hover:bg-primary-dark hover:text-white"
            @click="handleAiCreate"
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
              <path d="M12 8V4H8" />
              <rect width="16" height="12" x="4" y="8" rx="2" />
              <path d="M2 14h2" />
              <path d="M20 14h2" />
              <path d="M15 13v2" />
              <path d="M9 13v2" />
            </svg>

            AI 辨識
          </button>

          <!-- 手動新增 -->
          <button
            type="button"
            class="inline-flex items-center gap-2 rounded-xl bg-accent px-5 py-3 text-sm font-semibold text-text-primary shadow-sm transition hover:bg-accent-hover"
            @click="handleManualCreate"
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
              <path d="M5 12h14" />
              <path d="M12 5v14" />
            </svg>

            手動新增
          </button>
        </div>
      </div>

      <!-- =========================
           Loading
      ========================== -->
      <div
        v-if="loading && records.length === 0"
        class="mt-8 rounded-3xl border border-border bg-card p-8 text-center text-sm text-text-secondary shadow-sm"
      >
        正在載入飲食紀錄...
      </div>

      <!-- =========================
           Error
      ========================== -->
      <div
        v-else-if="error && records.length === 0"
        class="mt-8 rounded-3xl border border-danger/30 bg-card p-8 text-center"
      >
        <p class="text-sm text-danger">飲食紀錄載入失敗，請稍後再試。</p>
      </div>

      <template v-else>
        <!-- =========================
             Nutrition Summary
        ========================== -->
        <div class="mt-8">
          <NutritionSummary
            :nutrition="todayNutrition"
            :recommended-calories="recommendedCalories"
          />
        </div>

        <!-- =========================
             Food Records
        ========================== -->
        <div class="mt-6">
          <FoodRecordList :records="displayRecords" @view="handleViewRecord" />
        </div>
      </template>

      <!-- =========================
           Detail Modal
      ========================== -->
      <FoodRecordDetailModal
        v-if="selectedRecord"
        :record="selectedRecord"
        @close="closeDetailModal"
        @edit="handleEditRecord"
        @delete="handleDeleteRecord"
      />
    </div>
  </main>
</template>
