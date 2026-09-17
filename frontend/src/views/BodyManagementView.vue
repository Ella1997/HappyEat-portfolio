<script setup>
import { ref, onMounted, computed } from "vue";

import { useBodyRecord } from "@/composables/useBodyRecord";
import { useUser } from "@/composables/useUser";
import { useUserGoal } from "@/composables/useUserGoal";

import BodyRecordForm from "@/components/body/BodyRecordForm.vue";
import BodyRecordList from "@/components/body/BodyRecordList.vue";
import UserGoalForm from "@/components/body/UserGoalForm.vue";
import BodyVisualization from "@/components/body/BodyVisualization.vue";
import BodyTrendChart from "@/components/body/BodyTrendChart.vue";

import {
  calculateBMI,
  calculateAge,
  calculateBMR,
  calculateTDEE,
  calculateRecommendedCal,
  calculateGoalWeightStatus,
  calculateGoalProgress,
  calculateWaistHipRatio,
} from "@/utils/calculation";

import { formatActivityLevel } from "@/utils/format";

// =========================
// Composables
// =========================

const {
  records,
  loading,
  error,
  getRecords,
  createRecord,
  updateRecord,
  deleteRecord,
} = useBodyRecord();

const { user, getUser } = useUser();

const { activeGoal, getGoals, createGoal, updateGoal, endGoal } = useUserGoal();

// 先寫死 userId 測試
const userId = 2;

// =========================
// Body Record State
// =========================

const showBodyRecordModal = ref(false);
const editingBodyRecord = ref(null);

// =========================
// User Goal State
// =========================

const showGoalModal = ref(false);
const editingGoal = ref(null);

// =========================
// Computed - Body Record
// =========================

const latestRecord = computed(() => {
  return records.value[0] ?? null;
});

const bmi = computed(() => {
  if (!latestRecord.value) return null;

  return calculateBMI(latestRecord.value.weight, latestRecord.value.height);
});

const waistHipRatio = computed(() => {
  if (!latestRecord.value) return null;

  return calculateWaistHipRatio(
    latestRecord.value.waistSize,
    latestRecord.value.hipSize,
  );
});

// =========================
// Computed - User
// =========================

const age = computed(() => {
  return calculateAge(user.value?.birthDate);
});

// =========================
// Computed - Calories
// =========================

const bmr = computed(() => {
  if (!latestRecord.value || !user.value) return null;

  return calculateBMR({
    weight: latestRecord.value.weight,
    height: latestRecord.value.height,
    age: age.value,
    gender: user.value.gender,
  });
});

const tdee = computed(() => {
  if (!latestRecord.value) return null;

  return calculateTDEE(bmr.value, latestRecord.value.activityLevel);
});

const recommendedCalories = computed(() => {
  if (!tdee.value) return null;

  return calculateRecommendedCal(tdee.value, activeGoal.value?.goalType);
});

// =========================
// Computed - User Goal
// =========================

const goalWeightStatus = computed(() => {
  if (!latestRecord.value || activeGoal.value?.targetWeight == null) {
    return null;
  }

  return calculateGoalWeightStatus(
    latestRecord.value.weight,
    activeGoal.value.targetWeight,
    activeGoal.value.goalType,
  );
});

const goalProgress = computed(() => {
  if (
    !activeGoal.value ||
    activeGoal.value?.startWeight == null ||
    activeGoal.value?.targetWeight == null
  ) {
    return null;
  }

  const today = new Intl.DateTimeFormat("en-CA", {
    timeZone: "Asia/Taipei",
  }).format(new Date());

  //目標尚未開始
  if (activeGoal.value.startDate > today) return { status: "notStarted" };

  //目標已開始，但尚未有開始日期後的體態紀錄
  if (
    !latestRecord.value ||
    latestRecord.value.recordDate < activeGoal.value.startDate
  )
    return { status: "noRecord" };

  const progress = calculateGoalProgress(
    activeGoal.value.startWeight,
    latestRecord.value.weight,
    activeGoal.value.targetWeight,
    activeGoal.value.goalType,
  );

  if (!progress) {
    return {
      status: "invalid",
    };
  }

  return { status: "tracking", ...progress };
});

// =========================
// Body Record Handlers
// =========================

const openCreateBodyRecord = () => {
  editingBodyRecord.value = null;
  showBodyRecordModal.value = true;
};

const handleEditBodyRecord = (record) => {
  editingBodyRecord.value = record;
  showBodyRecordModal.value = true;
};

const closeBodyRecordModal = () => {
  showBodyRecordModal.value = false;
  editingBodyRecord.value = null;
};

const handleBodyRecordSubmit = async (data) => {
  if (editingBodyRecord.value) {
    await updateRecord(editingBodyRecord.value.bodyRecordId, userId, data);

    closeBodyRecordModal();
    return;
  }

  await createRecord({
    userId,
    ...data,
  });

  closeBodyRecordModal();
};

const handleDeleteBodyRecord = async (record) => {
  const confirmed = window.confirm(
    `確定要刪除 ${record.recordDate} 的體態紀錄嗎？`,
  );

  if (!confirmed) return;

  await deleteRecord(record.bodyRecordId, userId);
};

// =========================
// User Goal Handlers
// =========================

const openCreateGoal = () => {
  editingGoal.value = null;
  showGoalModal.value = true;
};

const openEditGoal = () => {
  editingGoal.value = activeGoal.value;
  showGoalModal.value = true;
};

const closeGoalModal = () => {
  showGoalModal.value = false;
  editingGoal.value = null;
};

const handleGoalSubmit = async (data) => {
  try {
    if (editingGoal.value) {
      await updateGoal(userId, editingGoal.value.goalId, data);
      window.alert("目標修改成功!");
      closeGoalModal();
      return;
    }
    await createGoal({
      userId,
      ...data,
    });
    window.alert("目標建立成功！");
    closeGoalModal();
  } catch (err) {
    window.alert(err.response?.data.detail ?? "目標操作失敗，請稍後再試。");
  }
};

// =========================
// Lifecycle
// =========================

onMounted(() => {
  getRecords(userId);
  getUser(userId);
  getGoals(userId);
});
</script>

<template>
  <main class="min-h-screen bg-page">
    <div class="mx-auto max-w-7xl px-6 py-8">
      <!-- =========================
           Page Header
      ========================== -->
      <div
        class="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between"
      >
        <div>
          <h1 class="text-3xl font-bold tracking-tight text-text-primary">
            體態管理
          </h1>

          <p class="mt-2 text-sm text-text-secondary">
            掌握身體變化，朝你的目標持續前進
          </p>
        </div>

        <button
          type="button"
          class="inline-flex items-center gap-1 rounded-xl bg-accent px-5 py-3 text-sm font-semibold text-text-primary shadow-sm transition hover:bg-accent-hover"
          @click="openCreateBodyRecord"
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
          新增紀錄
        </button>
      </div>

      <!-- Loading -->
      <p v-if="loading">載入中...</p>

      <!-- Error -->
      <p v-else-if="error" class="text-danger">取得資料失敗</p>

      <div v-else>
        <!-- ==================================================
     第一層：體態總覽

     Desktop:
     ┌──────────┬──────────────┬──────────┐
     │ 基本體態 │ 身體數據     │ 我的目標 │
     │ 人體     ├──────────────┤          │
     │          │ 體態趨勢     │          │
     └──────────┴──────────────┴──────────┘
=================================================== -->
        <section class="grid gap-6 lg:h-160 lg:grid-cols-[0.95fr_1.1fr_1fr]">
          <!-- =========================
       左欄：基本體態 / 人體視覺化
  ========================== -->
          <div
            class="rounded-3xl border border-border bg-card p-6 shadow-sm lg:h-full"
          >
            <!-- Header -->
            <div class="flex items-start justify-between gap-3">
              <div>
                <h2 class="text-xl font-semibold text-text-primary">
                  基本體態
                </h2>

                <p class="mt-1 text-sm text-text-secondary">最近一次身體紀錄</p>
              </div>

              <span
                v-if="latestRecord"
                class="shrink-0 rounded-full bg-primary-soft px-3 py-1 text-xs font-medium text-primary-dark"
              >
                {{ latestRecord.recordDate }}
              </span>
            </div>

            <!-- 有體態資料 -->
            <div v-if="latestRecord" class="mt-4">
              <BodyVisualization
                :neck-size="latestRecord.neckSize"
                :waist-size="latestRecord.waistSize"
                :hip-size="latestRecord.hipSize"
              />
            </div>

            <!-- 無體態資料 -->
            <div
              v-else
              class="mt-4 rounded-2xl bg-surface-soft px-6 py-12 text-center"
            >
              <p class="text-sm text-text-secondary">尚未建立體態紀錄</p>

              <button
                type="button"
                class="mt-4 rounded-xl bg-accent px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-accent-hover"
                @click="openCreateBodyRecord"
              >
                ＋ 建立第一筆紀錄
              </button>
            </div>
          </div>

          <!-- =========================
       中欄：身體數據 + 體態趨勢
  ========================== -->
          <div class="flex min-h-0 flex-col gap-6">
            <!-- =========================
         身體數據 KPI
    ========================== -->
            <div
              class="shrink-0 rounded-3xl border border-border bg-card p-5 shadow-sm"
            >
              <div>
                <h2 class="text-xl font-semibold text-text-primary">
                  身體數據
                </h2>

                <p class="mt-1 text-sm text-text-secondary">最新體態指標</p>
              </div>

              <!-- 有體態資料 -->
              <div v-if="latestRecord" class="mt-4 grid grid-cols-2 gap-3">
                <!-- 體重 -->
                <div class="rounded-2xl bg-info-soft px-4 py-3">
                  <p class="text-xs font-medium text-text-secondary">體重</p>

                  <p class="mt-1.5 text-2xl font-bold text-text-primary">
                    {{ latestRecord.weight }}

                    <span class="text-sm font-normal text-text-muted">
                      kg
                    </span>
                  </p>
                </div>

                <!-- BMI -->
                <div class="rounded-2xl bg-info-soft px-4 py-3">
                  <p class="text-xs font-medium text-text-secondary">BMI</p>

                  <p class="mt-1.5 text-2xl font-bold text-text-primary">
                    {{ bmi ?? "-" }}
                  </p>
                </div>

                <!-- 體脂率 -->
                <div class="rounded-2xl bg-info-soft px-4 py-3">
                  <p class="text-xs font-medium text-text-secondary">體脂率</p>

                  <p class="mt-1.5 text-2xl font-bold text-text-primary">
                    {{ latestRecord.bodyFat ?? "-" }}

                    <span class="text-sm font-normal text-text-muted"> % </span>
                  </p>
                </div>

                <!-- 肌肉量 -->
                <div class="rounded-2xl bg-info-soft px-4 py-3">
                  <p class="text-xs font-medium text-text-secondary">肌肉量</p>

                  <p class="mt-1.5 text-2xl font-bold text-text-primary">
                    {{ latestRecord.muscleMass ?? "-" }}

                    <span class="text-sm font-normal text-text-muted">
                      kg
                    </span>
                  </p>
                </div>

                <!-- 內臟脂肪 -->
                <div class="rounded-2xl bg-info-soft p-4">
                  <p class="text-xs font-medium text-text-secondary">
                    內臟脂肪
                  </p>

                  <p class="mt-2 text-2xl font-bold text-text-primary">
                    {{ latestRecord.visceralFat ?? "-" }}
                  </p>
                </div>

                <!-- 腰臀比 -->
                <div class="rounded-2xl bg-info-soft p-4">
                  <p class="text-xs font-medium text-text-secondary">腰臀比</p>

                  <p class="mt-2 text-2xl font-bold text-text-primary">
                    {{ waistHipRatio ?? "-" }}
                  </p>
                </div>
              </div>

              <!-- 無體態資料 -->
              <div
                v-else
                class="mt-6 rounded-2xl bg-surface-soft px-5 py-8 text-center"
              >
                <p class="text-sm text-text-secondary">
                  建立體態紀錄後即可查看身體數據
                </p>
              </div>
            </div>

            <!-- =========================
         體態趨勢
    ========================== -->
            <div
              class="min-h-0 flex-1 rounded-3xl border border-border bg-card p-6 shadow-sm"
            >
              <BodyTrendChart :records="records" />
            </div>
          </div>

          <!-- =========================
       右欄：我的目標
  ========================== -->
          <div
            class="rounded-3xl border border-border bg-card p-6 shadow-sm lg:h-full lg:overflow-y-auto"
          >
            <!-- Header -->
            <div class="flex items-center justify-between">
              <div>
                <h2 class="text-xl font-semibold text-text-primary">
                  我的目標
                </h2>

                <p class="mt-1 text-sm text-text-secondary">追蹤目前體態目標</p>
              </div>

              <button
                v-if="activeGoal"
                type="button"
                class="flex items-center gap-1.5 rounded-lg px-3 py-2 text-sm font-semibold text-accent transition-colors hover:bg-accent/10 hover:text-accent-hover"
                @click="openEditGoal"
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
                  <path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4Z" />
                </svg>

                編輯
              </button>
            </div>

            <!-- =========================
         有進行中目標
    ========================== -->
            <div v-if="activeGoal">
              <!-- 目標核心資訊 -->
              <div class="mt-4 grid grid-cols-2 gap-3">
                <!-- 目標類型 -->
                <div class="rounded-2xl bg-info-soft p-4">
                  <p class="text-xs font-medium text-text-secondary">目標</p>

                  <div class="mt-2 flex items-center gap-3">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      class="h-6 w-6 shrink-0 text-primary-dark"
                    >
                      <path d="M12 13V2l8 4-8 4" />
                      <path d="M20.561 10.222a9 9 0 1 1-12.55-5.29" />
                      <path d="M8.002 9.997a5 5 0 1 0 8.9 2.02" />
                    </svg>

                    <p class="text-xl font-bold text-text-primary">
                      {{ activeGoal.goalType }}
                    </p>
                  </div>
                </div>

                <!-- 目標體重 -->
                <div class="rounded-2xl bg-info-soft p-4">
                  <p class="text-xs font-medium text-text-secondary">
                    目標體重
                  </p>

                  <p class="mt-2 text-2xl font-bold text-text-primary">
                    {{ activeGoal.targetWeight ?? "-" }}

                    <span class="text-sm font-normal text-text-muted">
                      kg
                    </span>
                  </p>
                </div>
              </div>

              <!-- =========================
           有設定目標體重
      ========================== -->
              <div v-if="activeGoal.targetWeight != null">
                <!-- Goal 尚未開始 -->
                <div
                  v-if="goalProgress?.status === 'notStarted'"
                  class="mt-4 rounded-2xl bg-info-soft p-4"
                >
                  <p class="text-sm text-text-secondary">
                    目標將於
                    <span class="font-semibold text-text-primary">
                      {{ activeGoal.startDate }}
                    </span>
                    開始。
                  </p>
                </div>

                <!-- Goal 已開始，但沒有有效 BodyRecord -->
                <div
                  v-else-if="goalProgress?.status === 'noRecord'"
                  class="mt-4 rounded-2xl bg-info-soft p-4"
                >
                  <p class="text-sm text-text-secondary">
                    新增體態紀錄後，即可開始追蹤目標進度。
                  </p>
                </div>

                <!-- Goal 正常追蹤 -->
                <div v-else-if="goalProgress?.status === 'tracking'">
                  <!-- 距離目標 -->
                  <div
                    v-if="goalWeightStatus"
                    class="mt-5 rounded-2xl bg-info-soft p-4"
                  >
                    <p
                      v-if="goalWeightStatus.achieved"
                      class="text-sm font-semibold text-success"
                    >
                      已達成目標！
                    </p>

                    <p v-else class="text-sm text-text-secondary">
                      距離目標還差
                      <span class="font-semibold text-text-primary">
                        {{ goalWeightStatus.difference }}
                      </span>
                      kg
                    </p>
                  </div>

                  <!-- 目標進度 -->
                  <div class="mt-6">
                    <div class="mb-2 flex items-center justify-between text-sm">
                      <span class="text-text-secondary"> 目標進度 </span>

                      <span class="font-semibold text-primary-dark">
                        {{ goalProgress.progress }}%
                      </span>
                    </div>

                    <!-- Progress Bar -->
                    <div
                      class="h-2.5 overflow-hidden rounded-full bg-primary-soft"
                    >
                      <div
                        class="h-full rounded-full bg-primary transition-all duration-500"
                        :style="{
                          width: `${goalProgress.progress}%`,
                        }"
                      ></div>
                    </div>

                    <!-- 體重進度資訊 -->
                    <div class="mt-4 grid grid-cols-3 text-xs">
                      <!-- Start -->
                      <div>
                        <p class="text-text-muted">開始</p>

                        <p class="mt-1 font-semibold text-text-primary">
                          {{ goalProgress.startWeight }} kg
                        </p>
                      </div>

                      <!-- Current -->
                      <div class="text-center">
                        <p class="text-text-muted">目前</p>

                        <p class="mt-1 font-semibold text-text-primary">
                          {{ goalProgress.currentWeight }} kg
                        </p>
                      </div>

                      <!-- Target -->
                      <div class="text-right">
                        <p class="text-text-muted">目標</p>

                        <p class="mt-1 font-semibold text-text-primary">
                          {{ goalProgress.targetWeight }} kg
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- =========================
           其他目標資訊
      ========================== -->
              <div class="mt-6 space-y-4 border-t border-border pt-5">
                <!-- 開始日期 -->
                <div class="flex items-center justify-between text-sm">
                  <span class="text-text-secondary"> 開始日期 </span>

                  <span class="font-medium text-text-primary">
                    {{ activeGoal.startDate }}
                  </span>
                </div>

                <!-- 目標體脂率 -->
                <div
                  v-if="activeGoal.targetBodyFat != null"
                  class="flex items-center justify-between text-sm"
                >
                  <span class="text-text-secondary"> 目標體脂率 </span>

                  <span class="font-medium text-text-primary">
                    {{ activeGoal.targetBodyFat }} %
                  </span>
                </div>

                <!-- 預計完成日期 -->
                <div
                  v-if="activeGoal.targetDate"
                  class="flex items-center justify-between text-sm"
                >
                  <span class="text-text-secondary"> 預計完成 </span>

                  <span class="font-medium text-text-primary">
                    {{ activeGoal.targetDate }}
                  </span>
                </div>
              </div>
            </div>

            <!-- =========================
         沒有進行中目標
    ========================== -->
            <div
              v-else
              class="mt-4 rounded-2xl bg-info-soft px-5 py-8 text-center"
            >
              <div
                class="mx-auto flex h-11 w-11 items-center justify-center rounded-full bg-white text-primary-dark"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="h-6 w-6"
                >
                  <circle cx="12" cy="12" r="10" />
                  <circle cx="12" cy="12" r="6" />
                  <circle cx="12" cy="12" r="2" />
                </svg>
              </div>

              <p class="mt-3 text-sm font-medium text-text-primary">
                還沒有設定體態目標
              </p>

              <p class="mt-1 text-xs text-text-secondary">
                設定目標後即可追蹤你的進度
              </p>

              <button
                type="button"
                class="mt-4 rounded-xl bg-accent px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-accent-hover"
                @click="openCreateGoal"
              >
                ＋ 設定目標
              </button>
            </div>
          </div>
        </section>

        <!-- =========================
             第二層：每日熱量需求
        ========================== -->
        <section
          class="mt-6 rounded-3xl border border-border bg-card p-6 shadow-sm"
        >
          <div>
            <h2 class="text-xl font-semibold text-text-primary">
              每日熱量需求
            </h2>

            <p class="mt-1 text-sm text-text-secondary">
              根據目前體態與活動程度估算每日熱量需求
            </p>
          </div>

          <div class="mt-6 grid gap-4 md:grid-cols-3">
            <!-- BMR -->
            <div class="rounded-2xl bg-info-soft p-5">
              <p class="text-sm font-medium text-text-secondary">
                基礎代謝 BMR
              </p>

              <p class="mt-2 text-2xl font-bold text-text-primary">
                {{ bmr ?? "-" }}

                <span class="text-sm font-normal text-text-muted"> kcal </span>
              </p>

              <p class="mt-2 text-xs leading-5 text-text-muted">
                身體維持基本生命機能所需熱量
              </p>
            </div>

            <!-- TDEE -->
            <div class="rounded-2xl bg-info-medium p-5">
              <p class="text-sm font-medium text-text-secondary">
                每日總消耗 TDEE
              </p>

              <p class="mt-2 text-2xl font-bold text-text-primary">
                {{ tdee ?? "-" }}

                <span class="text-sm font-normal text-text-muted"> kcal </span>
              </p>

              <p class="mt-2 text-xs leading-5 text-text-muted">
                已納入目前活動程度：「{{
                  formatActivityLevel(latestRecord?.activityLevel)
                }}」
              </p>
            </div>

            <!-- Recommended -->
            <div class="rounded-2xl bg-info-strong p-5">
              <p class="text-sm font-medium text-text-secondary">
                建議每日攝取
              </p>

              <p class="mt-2 text-2xl font-bold text-text-primary">
                {{ recommendedCalories ?? "-" }}

                <span class="text-sm font-normal text-text-muted"> kcal </span>
              </p>

              <p
                v-if="activeGoal"
                class="mt-2 text-xs leading-5 text-text-muted"
              >
                依據「{{ activeGoal.goalType }}」目標計算
              </p>

              <p v-else class="mt-2 text-xs leading-5 text-text-muted">
                目前以維持體重需求估算
              </p>
            </div>
          </div>
        </section>

        <!-- =========================
             第三層：歷史紀錄
        ========================== -->
        <section class="mt-8">
          <BodyRecordList
            :records="records"
            @edit="handleEditBodyRecord"
            @delete="handleDeleteBodyRecord"
          />
        </section>
      </div>
    </div>

    <!-- =========================
         User Goal Modal
    ========================== -->
    <div
      v-if="showGoalModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/35 p-4"
      @click.self="closeGoalModal"
    >
      <div
        class="max-h-[calc(100vh-2rem)] w-full max-w-2xl overflow-hidden rounded-3xl bg-modal-content shadow-2xl"
      >
        <!-- Header -->
        <div
          class="flex items-center justify-between border-b border-border bg-modal-header px-7 py-5"
        >
          <div>
            <h2 class="text-xl font-semibold text-text-primary">
              {{ editingGoal ? "編輯體態目標" : "設定體態目標" }}
            </h2>

            <p class="mt-1 text-sm text-text-secondary">
              設定你的體態目標，持續追蹤進度
            </p>
          </div>

          <button
            type="button"
            class="flex h-9 w-9 items-center justify-center rounded-full text-xl text-text-muted transition hover:bg-white/50 hover:text-text-primary"
            @click="closeGoalModal"
          >
            ×
          </button>
        </div>

        <!-- Content -->
        <div
          class="scrollbar-none max-h-[calc(100vh-9rem)] overflow-y-auto bg-modal-content px-7 py-6"
        >
          <UserGoalForm
            :key="editingGoal?.goalId ?? 'new-goal'"
            :goal="editingGoal"
            :current-weight="latestRecord?.weight ?? null"
            @submit="handleGoalSubmit"
            @cancel="closeGoalModal"
          />
        </div>
      </div>
    </div>

    <!-- =========================
         Body Record Modal
    ========================== -->
    <div
      v-if="showBodyRecordModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/35 p-4"
      @click.self="closeBodyRecordModal"
    >
      <div
        class="max-h-[calc(100vh-2rem)] w-full max-w-2xl overflow-hidden rounded-3xl bg-modal-content shadow-2xl"
      >
        <!-- Header -->
        <div
          class="flex items-center justify-between border-b border-border bg-modal-header px-7 py-5"
        >
          <div>
            <h2 class="text-xl font-semibold text-text-primary">
              {{ editingBodyRecord ? "編輯體態紀錄" : "新增體態紀錄" }}
            </h2>

            <p class="mt-1 text-sm text-text-secondary">
              記錄身體數據，掌握體態變化
            </p>
          </div>

          <button
            type="button"
            class="flex h-9 w-9 items-center justify-center rounded-full text-xl text-text-muted transition hover:bg-white/50 hover:text-text-primary"
            @click="closeBodyRecordModal"
          >
            ×
          </button>
        </div>

        <!-- Content -->
        <div
          class="scrollbar-none max-h-[calc(100vh-9rem)] overflow-y-auto bg-modal-content px-7 py-6"
        >
          <BodyRecordForm
            :key="editingBodyRecord?.bodyRecordId ?? 'new-body-record'"
            :record="editingBodyRecord"
            @submit="handleBodyRecordSubmit"
            @cancel="closeBodyRecordModal"
          />
        </div>
      </div>
    </div>
  </main>
</template>
