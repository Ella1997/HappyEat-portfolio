<script setup>
import { computed } from "vue";

const props = defineProps({
  nutrition: {
    type: Object,
    required: true,
  },

  recommendedCalories: {
    type: Number,
    default: null,
  },
});

const calorieProgress = computed(() => {
  if (!props.recommendedCalories) {
    return 0;
  }

  return Math.min(
    Math.round((props.nutrition.calories / props.recommendedCalories) * 100),
    100,
  );
});

const formatNumber = (value) => {
  const number = Number(value ?? 0);

  return Number.isInteger(number)
    ? number.toLocaleString()
    : number.toLocaleString(undefined, {
        maximumFractionDigits: 1,
      });
};
</script>

<template>
  <section class="rounded-3xl border border-border bg-card p-6 shadow-sm">
    <!-- =========================
         Header + Calorie Progress
    ========================== -->
    <div
      class="flex flex-col gap-5 lg:flex-row lg:items-center lg:justify-between"
    >
      <!-- Title -->
      <div class="shrink-0">
        <h2 class="text-xl font-semibold text-text-primary">今日營養攝取</h2>
      </div>

      <!-- Calorie Progress -->
      <div v-if="recommendedCalories" class="w-full lg:max-w-xl">
        <!-- Progress Information -->
        <div
          class="mb-2 flex flex-wrap items-center justify-between gap-x-4 gap-y-1 text-sm"
        >
          <div class="flex items-center gap-2">
            <span class="text-text-secondary">
              建議
              {{ formatNumber(recommendedCalories) }}
              kcal
            </span>

            <span class="text-text-muted"> · </span>

            <span class="font-medium text-primary-dark">
              已攝取 {{ calorieProgress }}%
            </span>
          </div>

          <span class="text-text-muted">
            {{
              Math.max(
                recommendedCalories - nutrition.calories,
                0,
              ).toLocaleString()
            }}
            kcal 可攝取
          </span>
        </div>

        <!-- Progress Bar -->
        <div class="h-2 overflow-hidden rounded-full bg-info-medium">
          <div
            class="h-full rounded-full bg-primary transition-all"
            :style="{
              width: `${calorieProgress}%`,
            }"
          />
        </div>
      </div>
    </div>

    <!-- =========================
         Nutrition Cards
    ========================== -->
    <div
      class="mt-6 grid overflow-hidden rounded-2xl bg-surface-soft sm:grid-cols-2 lg:grid-cols-4"
    >
      <!-- =========================
           Calories
      ========================== -->
      <div class="bg-primary-soft px-7 py-7">
        <!-- Label -->
        <div class="flex items-center gap-3">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-6 w-6 text-primary-dark"
          >
            <path
              d="M12 22c4.97 0 9-3.582 9-8 0-2.5-1-4.5-3-6.5.08 2.5-1.5 4-3 4.5.5-4-2-8-6-10 .5 3-1 5-2.5 7C5 11 3 13 3 16c0 3.314 4.03 6 9 6Z"
            />
          </svg>

          <p class="text-base font-medium text-text-secondary">累計熱量</p>
        </div>

        <!-- Value -->
        <p class="mt-7 text-2xl font-bold text-text-primary">
          {{ formatNumber(nutrition.calories) }}

          <span class="text-sm font-normal text-text-secondary"> kcal </span>
        </p>
      </div>

      <!-- =========================
           Carbs
      ========================== -->
      <div class="border-border px-7 py-7 sm:border-l">
        <!-- Label -->
        <div class="flex items-center gap-3">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-6 w-6 text-accent-hover"
          >
            <path d="M2 22 16 8" />
            <path d="M3.47 12.53 5 11l1.53 1.53a2.5 2.5 0 0 0 3.54 0L11.6 11" />
            <path d="m8 8 1.53-1.53a2.5 2.5 0 0 1 3.54 0L14.6 8" />
            <path d="m15 5 2-2" />
          </svg>

          <p class="text-base font-medium text-text-secondary">碳水</p>
        </div>

        <!-- Value -->
        <p class="mt-7 text-2xl font-bold text-text-primary">
          {{ formatNumber(nutrition.carbs) }}

          <span class="text-sm font-normal text-text-secondary"> g </span>
        </p>
      </div>

      <!-- =========================
           Protein
      ========================== -->
      <div
        class="border-border px-7 py-7 sm:border-t lg:border-t-0 lg:border-l"
      >
        <!-- Label -->
        <div class="flex items-center gap-3">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-6 w-6 text-accent-hover"
          >
            <path
              d="M15.5 4.5c3 1 5 3.5 5 6.5 0 4.5-4 8-8.5 8-4 0-7.5-2.5-7.5-6.5 0-3.5 3-7 7-8.5"
            />

            <circle cx="12" cy="11" r="2.5" />
          </svg>

          <p class="text-base font-medium text-text-secondary">蛋白質</p>
        </div>

        <!-- Value -->
        <p class="mt-7 text-2xl font-bold text-text-primary">
          {{ formatNumber(nutrition.protein) }}

          <span class="text-sm font-normal text-text-secondary"> g </span>
        </p>
      </div>

      <!-- =========================
           Fat
      ========================== -->
      <div
        class="border-border px-7 py-7 sm:border-t sm:border-l lg:border-t-0"
      >
        <!-- Label -->
        <div class="flex items-center gap-3">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-6 w-6 text-accent-hover"
          >
            <path d="M12 2.7S6 9.2 6 14a6 6 0 0 0 12 0c0-4.8-6-11.3-6-11.3Z" />
          </svg>

          <p class="text-base font-medium text-text-secondary">脂肪</p>
        </div>

        <!-- Value -->
        <p class="mt-7 text-2xl font-bold text-text-primary">
          {{ formatNumber(nutrition.fat) }}

          <span class="text-sm font-normal text-text-secondary"> g </span>
        </p>
      </div>
    </div>
  </section>
</template>
