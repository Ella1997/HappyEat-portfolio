<script setup>
import { computed, onMounted, ref } from "vue";

import NutritionSummary from "@/components/food/NutritionSummary.vue";
import FoodRecordList from "@/components/food/FoodRecordList.vue";
import FoodRecordDetailModal from "@/components/food/FoodRecordDetailModal.vue";
import FoodRecordForm from "@/components/food/FoodRecordForm.vue";
import FoodAiRecognitionModal from "@/components/food/FoodAiRecognitionModal.vue";
import { useFoodRecord } from "@/composables/useFoodRecord";
import { foodImageApi } from "@/services/foodImageApi";

const serverBaseUrl = import.meta.env.VITE_SERVER_BASE_URL;

import { useBodyRecord } from "@/composables/useBodyRecord";
import { useUser } from "@/composables/useUser";
import { useUserGoal } from "@/composables/useUserGoal";

import {
  calculateAge,
  calculateBMR,
  calculateTDEE,
  calculateRecommendedCal,
} from "@/utils/calculation";

// =========================
// User
// 目前先使用測試 UserID = 2
// =========================
const userId = 2;

// =========================
// Food Record
// =========================
const {
  records,
  loading,
  error,
  getRecords,
  createRecord,
  updateRecord,
  deleteRecord,
} = useFoodRecord();

// =========================
// Food Images
// =========================
const images = ref([]);

const getImages = async () => {
  try {
    const response = await foodImageApi.getByUserId(userId);
    images.value = response.data;
  } catch (err) {
    console.error("取得飲食圖片失敗:", err);
    throw err;
  }
};

// =========================
// Recommended Calories Data
// =========================

const { records: bodyRecords, getRecords: getBodyRecords } = useBodyRecord();

const { user, getUser } = useUser();

const { activeGoal, getGoals } = useUserGoal();

const imageMap = computed(() => {
  return new Map(images.value.map((image) => [image.imageId, image.imagePath]));
});

const getImageUrl = (imageId) => {
  if (!imageId) return null;

  const imagePath = imageMap.value.get(imageId);

  if (!imagePath) return null;

  return `${serverBaseUrl}${imagePath}`;
};

// =========================
// Modal State
// =========================

const selectedRecord = ref(null); // Detail Modal 目前顯示的飲食紀錄

const isFormOpen = ref(false); //新增/編輯表單開關

const editingRecord = ref(null); //是否正在編輯

const isAiOpen = ref(false); //AI辨識開關

const aiImageId = ref(null);

const aiRecord = ref(null);

// =========================
// Recommended Calories
// =========================
// 最新一筆體態紀錄
const latestBodyRecord = computed(() => {
  return bodyRecords.value[0] ?? null;
});

// 年齡
const age = computed(() => {
  return calculateAge(user.value?.birthDate);
});

// BMR
const bmr = computed(() => {
  if (!latestBodyRecord.value || !user.value) return null;

  return calculateBMR({
    weight: latestBodyRecord.value.weight,
    height: latestBodyRecord.value.height,
    age: age.value,
    gender: user.value.gender,
  });
});

// TDEE
const tdee = computed(() => {
  if (!latestBodyRecord.value || !bmr.value) return null;

  return calculateTDEE(bmr.value, latestBodyRecord.value.activityLevel);
});

// 每日建議熱量
const recommendedCalories = computed(() => {
  if (!tdee.value) return null;

  return calculateRecommendedCal(tdee.value, activeGoal.value?.goalType);
});

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

      imagePath: getImageUrl(record.imageId),
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

// AI 辨識新增
const handleAiCreate = () => {
  isAiOpen.value = true;
};

const closeAiRecognition = () => {
  isAiOpen.value = false;
};

const handleAiConfirm = ({ imageId, result }) => {
  aiImageId.value = imageId;

  // 將GeminiRResponseDto轉成FoodRecordForm可使用的格式
  aiRecord.value = {
    recordId: null,
    userId,
    imageId,
    recordSource: "AI",
    recordDate: getLocalDateKey(),
    mealType: "",
    description: result.description ?? "",
    items: (result.items ?? []).map((item) => ({
      itemName: item.itemName,
      foodId: null,
      drinkId: null,
      quantity: Number(item.quantity) || 1,
      unit: item.unit ?? "",
      calories: Number(item.calories ?? 0),
      carbs: Number(item.carbs ?? 0),
      protein: Number(item.protein ?? 0),
      fat: Number(item.fat ?? 0),
    })),
  };
  isAiOpen.value = false;
  isFormOpen.value = true;
};

// 開啟手動新增表單
const handleManualCreate = () => {
  editingRecord.value = null;
  isFormOpen.value = true;
};

// 開啟飲食紀錄明細
const handleViewRecord = (record) => {
  selectedRecord.value = record;
};

// 關閉飲食紀錄明細
const closeDetailModal = () => {
  selectedRecord.value = null;
};

// 從 Detail Modal 進入編輯模式
const handleEditRecord = (record) => {
  // 先關閉 Detail Modal
  selectedRecord.value = null;

  // 將目前 record 傳給 FoodRecordForm
  editingRecord.value = record;

  // 開啟表單 Modal
  isFormOpen.value = true;
};

// 關閉新增 / 編輯表單
const closeForm = async (shouldCleanupAiImage = true) => {
  // AI 辨識已建立圖片，但使用者尚未建立 FoodRecord 就取消則要刪除DB的圖片
  if (shouldCleanupAiImage && aiImageId.value && !editingRecord.value) {
    try {
      await foodImageApi.delete(aiImageId.value);
    } catch (err) {
      console.error("清除未使用的 AI 圖片失敗：", err);
    }
  }
  isFormOpen.value = false;
  editingRecord.value = null;
  aiImageId.value = null;
  aiRecord.value = null;
};

// 新增 / 編輯飲食紀錄
const handleFormSubmit = async (formPayload) => {
  try {
    // FoodRecordForm 只負責表單內容，
    // UserId、ImageId、RecordSource 由 FoodDiaryView 補上
    const payload = {
      userId,
      imageId: editingRecord.value?.imageId ?? aiImageId.value ?? null,
      recordSource:
        editingRecord.value?.recordSource ?? (aiRecord.value ? "AI" : "Manual"),
      ...formPayload,
    };

    if (editingRecord.value) {
      // =========================
      // Edit
      // =========================
      await updateRecord(editingRecord.value.recordId, userId, payload);
    } else {
      // =========================
      // Create
      // =========================
      await createRecord(payload);
      if (aiImageId.value) {
        await getImages();
      }
    }
    closeForm(false);
  } catch (err) {
    console.error(
      editingRecord.value ? "更新飲食紀錄失敗：" : "新增飲食紀錄失敗：",
      err,
    );

    window.alert(
      err.response?.data?.detail ??
        (editingRecord.value
          ? "更新飲食紀錄失敗，請稍後再試。"
          : "新增飲食紀錄失敗，請稍後再試。"),
    );
  }
};

// 刪除飲食紀錄
const handleDeleteRecord = async (record) => {
  const confirmed = window.confirm(
    `確定要刪除「${getDateKey(record.recordDate)}」這筆飲食紀錄嗎？`,
  );

  if (!confirmed) return;

  try {
    await deleteRecord(record.recordId, userId);

    selectedRecord.value = null;
  } catch (err) {
    console.error("刪除飲食紀錄失敗：", err);

    window.alert(
      err.response?.data?.detail ?? "刪除飲食紀錄失敗，請稍後再試。",
    );
  }
};

// =========================
// Init
// =========================
onMounted(async () => {
  try {
    await Promise.all([
      // Food Diary
      getRecords(userId),
      getImages(),

      // Recommended Calories
      getBodyRecords(userId),
      getUser(userId),
      getGoals(userId),
    ]);
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
            class="inline-flex items-center gap-1 rounded-xl bg-accent px-5 py-3 text-sm font-semibold text-text-primary shadow-sm transition hover:bg-accent-hover"
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

      <!-- =========================
           Main Content
      ========================== -->
      <template v-else>
        <!-- Nutrition Summary -->
        <div class="mt-8">
          <NutritionSummary
            :nutrition="todayNutrition"
            :recommended-calories="recommendedCalories"
          />
        </div>

        <!-- Food Records -->
        <div class="mt-6">
          <FoodRecordList :records="displayRecords" @view="handleViewRecord" />
        </div>
      </template>

      <!-- =====================================================
           Detail Modal
           獨立 Modal
      ====================================================== -->
      <FoodRecordDetailModal
        v-if="selectedRecord"
        :record="selectedRecord"
        @close="closeDetailModal"
        @edit="handleEditRecord"
        @delete="handleDeleteRecord"
      />

      <!-- =====================================================
           Food Record Form Modal
           手動新增 / 編輯共用
      ====================================================== -->
      <div
        v-if="isFormOpen"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/35 p-4"
        @click.self="closeForm"
      >
        <div
          class="scrollbar-none max-h-[90vh] w-full max-w-3xl overflow-y-auto rounded-3xl border border-border bg-modal-content shadow-xl"
        >
          <!-- Modal Header -->
          <div
            class="sticky top-0 z-20 flex items-center justify-between border-b border-border bg-modal-header px-6 py-5"
          >
            <div>
              <h2 class="text-lg font-semibold text-text-primary">
                {{
                  editingRecord
                    ? "編輯飲食紀錄"
                    : aiRecord
                      ? "確認 AI 辨識結果"
                      : "新增飲食紀錄"
                }}
              </h2>

              <p class="mt-1 text-sm text-text-muted">
                {{
                  editingRecord
                    ? "修改餐點與營養資訊"
                    : aiRecord
                      ? "確認餐點與營養資訊，必要時可手動調整"
                      : "新增本餐的食物、飲料與營養資訊"
                }}
              </p>
            </div>

            <!-- Close -->
            <button
              type="button"
              class="flex h-9 w-9 items-center justify-center rounded-xl text-text-muted transition hover:bg-white/60 hover:text-text-primary"
              aria-label="關閉"
              @click="closeForm"
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
                <path d="M18 6 6 18" />
                <path d="m6 6 12 12" />
              </svg>
            </button>
          </div>

          <!-- Form Content -->
          <div class="p-6">
            <FoodRecordForm
              :key="
                editingRecord?.recordId ?? (aiRecord ? 'ai-create' : 'create')
              "
              :record="editingRecord ?? aiRecord"
              @submit="handleFormSubmit"
              @cancel="closeForm"
            />
          </div>
        </div>
      </div>
      <!-- =========================
          AI Recognition Modal
      ========================== -->
      <FoodAiRecognitionModal
        v-if="isAiOpen"
        :user-id="userId"
        @confirm="handleAiConfirm"
        @close="closeAiRecognition"
      />
    </div>
  </main>
</template>
